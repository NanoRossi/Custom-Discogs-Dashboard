using DiscogsProxy.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiscogsProxy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportController(IImportService importService) : ControllerBase
{
    private readonly IImportService _importService = importService;

    [HttpGet("")]
    public async Task<ActionResult> ImportDataset()
    {
        var importData = await _importService.ImportData();

        if (importData.HasError)
        {
            return Problem(importData.Error!.Message);
        }

        return Ok("Data imported!");
    }
}