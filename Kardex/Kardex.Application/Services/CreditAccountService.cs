using Kardex.Application.Abstractions;
using Kardex.Application.DTOs;
using Kardex.Domain.Entities;
using Kardex.Domain.Repositories;

namespace Kardex.Application.Services;

public sealed class CreditAccountService : ICreditAccountService
{
    private readonly ICreditAccountRepository _repository;

    public CreditAccountService(ICreditAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<CreditAccountDto>> GetAllAsync(string? search, CancellationToken cancellationToken = default)
    {
        var accounts = await _repository.GetAllAsync(search, cancellationToken);
        return accounts.Select(Map).ToList();
    }

    public async Task<CreditAccountDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var account = await _repository.GetByIdAsync(id, cancellationToken);
        return account is null ? null : Map(account);
    }

    public async Task<CreditAccountDto> CreateAsync(CreateCreditAccountRequest request, CancellationToken cancellationToken = default)
    {
        var account = new CreditAccount(
            request.CustomerName,
            request.IdentificationType,
            request.IdentificationNumber,
            request.CreditLimit,
            request.PaymentTermDays);

        await _repository.AddAsync(account, cancellationToken);
        return Map(account);
    }

    public async Task<CreditAccountDto?> UpdateAsync(Guid id, UpdateCreditAccountRequest request, CancellationToken cancellationToken = default)
    {
        var account = await _repository.GetByIdAsync(id, cancellationToken);
        if (account is null)
        {
            return null;
        }

        account.UpdateDetails(request.CustomerName, request.CreditLimit, request.PaymentTermDays);
        await _repository.UpdateAsync(account, cancellationToken);
        return Map(account);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var account = await _repository.GetByIdAsync(id, cancellationToken);
        if (account is null)
        {
            return false;
        }

        await _repository.DeleteAsync(account, cancellationToken);
        return true;
    }

    private static CreditAccountDto Map(CreditAccount account)
    {
        return new CreditAccountDto(
            account.Id,
            account.CustomerName,
            account.IdentificationType,
            account.IdentificationNumber,
            account.CreditLimit,
            account.PaymentTermDays,
            account.CurrentBalance,
            account.AvailableCredit,
            account.CreatedAt,
            account.UpdatedAt);
    }
}
