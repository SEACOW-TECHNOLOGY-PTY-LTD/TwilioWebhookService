using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using Shared.Models.Twilio;
using SqlSugar;

namespace WebhookService.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class LogsController(ISqlSugarClient db) : ControllerBase
{
    [HttpGet]
    [TwilioTokenAuthorize]
    public async Task<IActionResult> GetAllLogs()
    {
        var logs = await db.Queryable<CallLog>().ToListAsync();

        return this.UnifiedOk(logs);
    }
}