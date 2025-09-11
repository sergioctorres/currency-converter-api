using Application.Constants;
using Application.Dtos.Common;
using Application.Dtos.CurrencyRate;
using Application.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.v1;

[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class CurrencyRatesController(ICurrencyRateProvider currencyProvider) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(LatestResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAsync([FromQuery] LatestRequest request, CancellationToken cancellationToken)
    {
        return Ok(await currencyProvider.GetLatestCurrencyRatesAsync(request, cancellationToken));
    }

    [HttpGet]
    [Route("convert")]
    [ProducesResponseType(typeof(ConvertResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ConvertAsync([FromQuery] ConvertRequest request, CancellationToken cancellationToken)
    {
        return Ok(await currencyProvider.ConvertCurrencyRatesAsync(request, cancellationToken));
    }

    [HttpGet]
    [Route("history")]
    [Authorize(PolicyConstants.RequireAdmin)]
    [ProducesResponseType(typeof(PagedResult<HistoricalResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetHistoricalAsync([FromQuery] HistoricalRequest request, CancellationToken cancellationToken)
    {
        return Ok(await currencyProvider.GetHistoricalCurrencyRatesAsync(request, cancellationToken));
    }
}
