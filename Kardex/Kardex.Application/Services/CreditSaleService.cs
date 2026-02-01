using Kardex.Application.Abstractions;
using Kardex.Application.DTOs;
using Kardex.Domain.Entities;
using Kardex.Domain.Repositories;

namespace Kardex.Application.Services;

public sealed class CreditSaleService : ICreditSaleService
{
    private readonly ICreditAccountRepository _accountRepository;
    private readonly ICreditMovementRepository _movementRepository;

    public CreditSaleService(
        ICreditAccountRepository accountRepository,
        ICreditMovementRepository movementRepository)
    {
        _accountRepository = accountRepository;
        _movementRepository = movementRepository;
    }

    public async Task<CreditMovementDto> RegisterAsync(
        CreateCreditSaleRequest request,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdentificationAsync(
            request.IdentificationType,
            request.IdentificationNumber,
            cancellationToken);

        if (account is null)
        {
            throw new InvalidOperationException("No se encontró un cliente con esa identificación.");
        }

        var movement = account.RegisterCreditSale(request.Amount, request.SaleId, request.OccurredOn);
        await _movementRepository.AddAsync(movement, cancellationToken);
        await _accountRepository.UpdateAsync(account, cancellationToken);

        return Map(movement);
    }

    public async Task<CreditMovementDto> RegisterByAccountAsync(
        Guid accountId,
        CreateCreditSaleByAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account is null)
        {
            throw new InvalidOperationException("No se encontró el cliente.");
        }

        var movement = account.RegisterCreditSale(request.Amount, request.SaleId, request.OccurredOn);
        await _movementRepository.AddAsync(movement, cancellationToken);
        await _accountRepository.UpdateAsync(account, cancellationToken);

        return Map(movement);
    }

    public async Task<IReadOnlyList<CreditMovementDto>> GetMovementsAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        var movements = await _movementRepository.GetByAccountIdAsync(accountId, cancellationToken);
        return movements.Select(Map).ToList();
    }

    private static CreditMovementDto Map(CreditMovement movement)
    {
        return new CreditMovementDto(
            movement.Id,
            movement.CreditAccountId,
            movement.SaleId,
            movement.Amount,
            movement.DueDate,
            movement.Status,
            movement.CreatedAt);
    }
}
