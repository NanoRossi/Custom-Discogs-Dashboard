using DiscogsProxy.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiscogsProxy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InfoController(IInfoService infoService) : ControllerBase
{
    private readonly IInfoService _infoService = infoService;

    [HttpGet("artists")]
    public ActionResult GetArtists()
    {
        var styles = _infoService.GetArtists();

        if (styles.HasError)
        {
            return Problem(styles!.Error!.Message);
        }

        return Ok(styles.Result);
    }

    [HttpGet("styles")]
    public ActionResult GetStyles()
    {
        var styles = _infoService.GetStyles();

        if (styles.HasError)
        {
            return Problem(styles!.Error!.Message);
        }

        return Ok(styles.Result);
    }

    [HttpGet("genres")]
    public ActionResult GetGenres()
    {
        var styles = _infoService.GetGenres();

        if (styles.HasError)
        {
            return Problem(styles!.Error!.Message);
        }

        return Ok(styles.Result);
    }

    [HttpGet("fact")]
    public ActionResult GetFact()
    {
        var fact = _infoService.GetFact();

        if (fact.HasError)
        {
            return Problem(fact!.Error!.Message);
        }

        return Ok(fact.Result);
    }
}