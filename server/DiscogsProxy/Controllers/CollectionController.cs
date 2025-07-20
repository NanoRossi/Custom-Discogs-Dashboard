using DiscogsProxy.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiscogsProxy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollectionController(ICollectionService collectionService) : ControllerBase
{
    private readonly ICollectionService _collectionService = collectionService;

    [HttpGet("random/vinyl")]
    public async Task<ActionResult> GetRandomCollectionVinyl()
    {
        var collectionImport = await _collectionService.GetRandomCollectionVinyl();

        if (collectionImport.HasError)
        {
            return Problem(collectionImport!.Error!.Message);
        }

        return Ok(collectionImport.Result);
    }

    [HttpGet("random/cd")]
    public async Task<ActionResult> GetRandomCollectionCD()
    {
        var collectionImport = await _collectionService.GetRandomCollectionCD();

        if (collectionImport.HasError)
        {
            return Problem(collectionImport!.Error!.Message);
        }

        return Ok(collectionImport.Result);
    }

    [HttpGet("recent/{numOfItems}")]
    public async Task<ActionResult> GetRecentAdditions(int numOfItems)
    {
        var collectionImport = await _collectionService.GetRecentAdditions(numOfItems);

        if (collectionImport.HasError)
        {
            return Problem(collectionImport!.Error!.Message);
        }

        return Ok(collectionImport.Result);
    }
}