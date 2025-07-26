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
        var recycle = await _importService.RecycleDb();

        if (recycle.HasError)
        {
            return Problem(recycle.Error!.Message);
        }

        var collectionImport = await _importService.ImportCollection();

        if (collectionImport.HasError)
        {
            return Problem("Error importing Collection");
        }

        var wantlistImport = await _importService.ImportWantlist();

        if (wantlistImport.HasError)
        {
            return Problem("Error importing Wantlist");
        }

        return Ok("Data imported!");
    }
}