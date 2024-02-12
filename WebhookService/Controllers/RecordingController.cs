using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using Shared.Middlewares;
using WebhookService.Producers;

namespace WebhookService.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class RecordingController(TwilioEventProducer twilioRecordingStatusProducer) : ControllerBase
{
    [HttpPost]
    [ValidateTwilioRequest]
    public async Task<IActionResult> ReceiveEvent([FromForm] Dictionary<string, string> data)
    {
        var recordingStatus = data.GetValueOrDefault("RecordingStatus");
        
        if (!string.IsNullOrEmpty(recordingStatus))
            await twilioRecordingStatusProducer.Send(data, "recording." + recordingStatus, "recording." + recordingStatus + ".handler");
        else
            await twilioRecordingStatusProducer.Send(data, "unknown.recording" , "unknown.recording.handler");

        // 返回 204 No Content 并设置 Content-Type 头部为 "application/json"
        return this.UnifiedNoContent();
    }
}