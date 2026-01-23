using Inventario.Application.Abstractions;
using Inventario.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.API.Controllers;

[ApiController]
[Route("api/stocks")]
public sealed class StocksController : ControllerBase
{
    private readonly IStockService _stockService;

    public StocksController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StockDto>>> GetAll(CancellationToken cancellationToken)
    {
        var stocks = await _stockService.GetAllAsync(cancellationToken);
        return Ok(stocks);
    }

    [HttpGet("{productId:guid}")]
    public async Task<ActionResult<StockDto>> GetByProductId(Guid productId, CancellationToken cancellationToken)
    {
        var stock = await _stockService.GetByProductIdAsync(productId, cancellationToken);
        if (stock is null)
        {
            return NotFound();
        }

        return Ok(stock);
    }

    [HttpPost("{productId:guid}")]
    public async Task<ActionResult<StockDto>> Create(Guid productId, CancellationToken cancellationToken)
    {
        var stock = await _stockService.CreateForProductAsync(productId, cancellationToken);
        return CreatedAtAction(nameof(GetByProductId), new { productId = stock.ProductId }, stock);
    }

    [HttpPut("{productId:guid}/increase")]
    public async Task<ActionResult<StockDto>> Increase(Guid productId, AdjustStockRequest request, CancellationToken cancellationToken)
    {
        var stock = await _stockService.IncreaseAsync(productId, request.Quantity, cancellationToken);
        if (stock is null)
        {
            return NotFound();
        }

        return Ok(stock);
    }

    [HttpPut("{productId:guid}/decrease")]
    public async Task<ActionResult<StockDto>> Decrease(Guid productId, AdjustStockRequest request, CancellationToken cancellationToken)
    {
        var stock = await _stockService.DecreaseAsync(productId, request.Quantity, cancellationToken);
        if (stock is null)
        {
            return NotFound();
        }

        return Ok(stock);
    }
}
