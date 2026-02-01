using Kardex.Application.Abstractions;
using Kardex.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Kardex.API.Controllers;

[ApiController]
[Route("api/kardex/credit-accounts")]
public sealed class CreditAccountsController : ControllerBase
{
    private readonly ICreditAccountService _creditAccountService;
    private readonly ICreditSaleService _creditSaleService;

    public CreditAccountsController(
        ICreditAccountService creditAccountService,
        ICreditSaleService creditSaleService)
    {
        _creditAccountService = creditAccountService;
        _creditSaleService = creditSaleService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CreditAccountDto>>> GetAll(
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var accounts = await _creditAccountService.GetAllAsync(search, cancellationToken);
        return Ok(accounts);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CreditAccountDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var account = await _creditAccountService.GetByIdAsync(id, cancellationToken);
        if (account is null)
        {
            return NotFound();
        }

        return Ok(account);
    }

    [HttpPost]
    public async Task<ActionResult<CreditAccountDto>> Create(
        CreateCreditAccountRequest request,
        CancellationToken cancellationToken)
    {
        var account = await _creditAccountService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CreditAccountDto>> Update(
        Guid id,
        UpdateCreditAccountRequest request,
        CancellationToken cancellationToken)
    {
        var account = await _creditAccountService.UpdateAsync(id, request, cancellationToken);
        if (account is null)
        {
            return NotFound();
        }

        return Ok(account);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var removed = await _creditAccountService.DeleteAsync(id, cancellationToken);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("{id:guid}/movements")]
    public async Task<ActionResult<IReadOnlyList<CreditMovementDto>>> GetMovements(
        Guid id,
        CancellationToken cancellationToken)
    {
        var movements = await _creditSaleService.GetMovementsAsync(id, cancellationToken);
        return Ok(movements);
    }

    [HttpPost("{id:guid}/credit-sales")]
    public async Task<ActionResult<CreditMovementDto>> RegisterCreditSale(
        Guid id,
        CreateCreditSaleByAccountRequest request,
        CancellationToken cancellationToken)
    {
        var movement = await _creditSaleService.RegisterByAccountAsync(id, request, cancellationToken);
        return Ok(movement);
    }
}
