using Compras.Application.Abstractions;
using Compras.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Compras.API.Controllers;

[ApiController]
[Route("api/purchases")]
public sealed class PurchasesController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    public PurchasesController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PurchaseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var purchases = await _purchaseService.GetAllAsync(cancellationToken);
        return Ok(purchases);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PurchaseDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var purchase = await _purchaseService.GetByIdAsync(id, cancellationToken);
        if (purchase is null)
        {
            return NotFound();
        }

        return Ok(purchase);
    }

    [HttpPost]
    public async Task<ActionResult<PurchaseDto>> Create(CreatePurchaseRequest request, CancellationToken cancellationToken)
    {
        var purchase = await _purchaseService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = purchase.Id }, purchase);
    }

    [HttpPut("{id:guid}/receive")]
    public async Task<ActionResult<PurchaseDto>> MarkReceived(Guid id, CancellationToken cancellationToken)
    {
        var purchase = await _purchaseService.MarkReceivedAsync(id, cancellationToken);
        if (purchase is null)
        {
            return NotFound();
        }

        return Ok(purchase);
    }
}
