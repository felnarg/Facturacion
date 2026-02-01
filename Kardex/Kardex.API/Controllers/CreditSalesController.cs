using Kardex.Application.Abstractions;
using Kardex.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Kardex.API.Controllers;

[ApiController]
[Route("api/kardex/credit-sales")]
public sealed class CreditSalesController : ControllerBase
{
    private readonly ICreditSaleService _creditSaleService;

    public CreditSalesController(ICreditSaleService creditSaleService)
    {
        _creditSaleService = creditSaleService;
    }

    [HttpPost]
    public async Task<ActionResult<CreditMovementDto>> Create(
        CreateCreditSaleRequest request,
        CancellationToken cancellationToken)
    {
        var movement = await _creditSaleService.RegisterAsync(request, cancellationToken);
        return Ok(movement);
    }
}
