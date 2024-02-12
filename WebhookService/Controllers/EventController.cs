using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using Shared.Middlewares;
using WebhookService.Producers;

namespace WebhookService.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class EventController(TwilioEventProducer twilioEventProducer) : ControllerBase
{
    [HttpPost]
    [ValidateTwilioRequest]
    public async Task<IActionResult> ReceiveEvent([FromForm] Dictionary<string, string> data)
    {
        var eventType = data.GetValueOrDefault("EventType");
        
        if (!string.IsNullOrEmpty(eventType))
            await twilioEventProducer.Send(data, eventType, eventType + ".handler");
        else 
            await twilioEventProducer.Send(data, "twilio.unknown.event", "twilio.unknown.event.handler");

        // 返回 204 No Content 并设置 Content-Type 头部为 "application/json"
        return this.UnifiedNoContent();
    }
}