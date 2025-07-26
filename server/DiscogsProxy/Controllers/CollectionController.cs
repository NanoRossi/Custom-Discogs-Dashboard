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

    [HttpGet("getall/artist/{artistName}")]
    public ActionResult GetAllForArtist(string artistName)
    {
        var artistEntries = _collectionService.GetAllForArtist(artistName.Replace("%20", " "));

        if (artistEntries.HasError)
        {
            return Problem(artistEntries!.Error!.Message);
        }

        return Ok(artistEntries.Result);
    }

    [HttpGet("getall/genre/{genreName}")]
    public ActionResult GetAllForGenre(string genreName)
    {
        var genreEntries = _collectionService.GetAllForGenre(genreName.Replace("%20", " "));

        if (genreEntries.HasError)
        {
            return Problem(genreEntries!.Error!.Message);
        }

        return Ok(genreEntries.Result);
    }

    [HttpGet("getall/style/{styleName}")]
    public ActionResult GetAllForStyle(string styleName)
    {
        var styleEntries = _collectionService.GetAllForStyle(styleName.Replace("%20", " "));

        if (styleEntries.HasError)
        {
            return Problem(styleEntries!.Error!.Message);
        }

        return Ok(styleEntries.Result);
    }

}