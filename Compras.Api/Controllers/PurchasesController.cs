using Compras.Application.Interfaces;
using Compras.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Compras.Api.Controllers;

[ApiController]
[Route("api/purchases")]
public sealed class PurchasesController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    public PurchasesController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PurchaseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseRequest request, CancellationToken cancellationToken)
    {
        var result = await _purchaseService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PurchaseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _purchaseService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<PurchaseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] Guid? customerId, CancellationToken cancellationToken)
    {
        var result = await _purchaseService.ListAsync(customerId, cancellationToken);
        return Ok(result);
    }
}
