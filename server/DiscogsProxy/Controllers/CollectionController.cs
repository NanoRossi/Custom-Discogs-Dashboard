using DiscogsProxy.Services;
using DiscogsProxy.Workers;
using Microsoft.AspNetCore.Mvc;

namespace DiscogsProxy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollectionController(ICollectionService collectionService) : ControllerBase
{
    private ICollectionService _collectionService = collectionService;

    [HttpGet("random")]
    public async Task<ActionResult> GetRandomCollectionItem()
    {
        var collectionImport = await _collectionService.GetRandomCollectionItem();

        if (collectionImport.HasError)
        {
            return Problem(collectionImport!.Error!.Message);
        }

        return Ok(collectionImport.Result);
    }
}