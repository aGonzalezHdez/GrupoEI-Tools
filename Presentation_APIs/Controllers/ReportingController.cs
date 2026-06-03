using Application.DTOs.Reporting;
using Application.Interfaces.Reporting;
using Microsoft.AspNetCore.Mvc;

namespace Presentatio_APIs.Controllers;

[ApiController]
[Route("api/reporting")]
public class ReportingController : ControllerBase
{
    private readonly IReportingUseCase _reportingUseCase;

    public ReportingController(IReportingUseCase reportingUseCase)
    {
        _reportingUseCase = reportingUseCase;
    }

    [HttpPost("/CargarControl")]
    public async Task<IActionResult> CargarControl([FromBody] CargarControlRequest request, CancellationToken ct)
    {
        var resultado = await _reportingUseCase.CargarControlAsync(request, ct);
        return resultado.Success
            ? Ok(resultado)
            : StatusCode(StatusCodes.Status502BadGateway, resultado);
    }
}
