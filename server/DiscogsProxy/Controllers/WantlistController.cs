using DiscogsProxy.Services;
using DiscogsProxy.Workers;
using Microsoft.AspNetCore.Mvc;

namespace DiscogsProxy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WantlistController(IWantListService wantListService) : ControllerBase
{
    private IWantListService _wantListService = wantListService;

    [HttpGet("")]
    public ActionResult GetWantlist()
    {
        var wantlist = _wantListService.GetWantlistItems();

        if (wantlist.HasError)
        {
            return Problem(wantlist!.Error!.Message);
        }

        return Ok(wantlist.Result);
    }
}