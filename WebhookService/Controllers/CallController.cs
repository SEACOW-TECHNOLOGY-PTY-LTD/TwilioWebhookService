using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using Shared.Middlewares;
using WebhookService.Producers;

namespace WebhookService.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class CallController()
    : ControllerBase
{
    [HttpPost]
    [ValidateTwilioRequest]
    public async Task<IActionResult> ReceiveEvent([FromForm] Dictionary<string, string> data)
    {
        await Task.Delay(1000).WaitAsync(CancellationToken.None);
        
        // var callStatus = data.GetValueOrDefault("CallStatus");
        
        // await _twilioCallStatusProducer.Send(data, callStatus ?? "Unknown");

        // 返回 204 No Content 并设置 Content-Type 头部为 "application/json"
        return this.UnifiedNoContent();
    }
}