using DiscogsProxy.Constants;
using DiscogsProxy.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiscogsProxy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatusController(IStatusService statusService) : ControllerBase
{
    private IStatusService _statusService = statusService;

    [HttpGet("")]
    public ActionResult GetStatus()
    {
        var status = _statusService.GetStatus();

        if (status!.Result!.DatabaseStatus == DbStatus.Disconnected)
        {
            return Problem("Cannot connect to Database");
        }

        return Ok(status.Result);
    }
}