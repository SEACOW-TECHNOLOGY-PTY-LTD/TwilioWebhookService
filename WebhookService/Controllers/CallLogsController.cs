using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using Shared.Models.Twilio;
using SqlSugar;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;

namespace WebhookService.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class CallLogsController(
    ISqlSugarClient db,
    ILogger<CallLogsController> logger,
    ITwilioRestClient twilioRestClient)
    : ControllerBase
{
    [HttpGet("sync")]
    public async Task<IActionResult> SyncCallLogs()
    {
        logger.LogDebug("Starting FetchCallLogJob at {time}", DateTime.Now.ToString("HH:mm:ss"));
        
        var endTime = DateTime.UtcNow.AddDays(3);
        var startTime = endTime.AddDays(-6);
        
        var calls = await CallResource.ReadAsync(
            startTimeBefore: endTime,
            startTimeAfter: startTime,
            client: twilioRestClient
        );
        
        logger.LogDebug("Start DateTime: {startTime}, End DateTime: {endTime}", startTime, endTime);
        logger.LogDebug("Total records fetched: {count}", calls.Count());

        int existingCount = 0;
        int newRecordsCount = 0;

        foreach (var call in calls)
        {
            if (call.To.StartsWith("client:")) continue;
            
            var callLog = new CallLog()
            {
                CallSid = call.Sid,
                From = call.From,
                To = call.To,
                CallStatus = call.Status.ToString() ?? "Unknown",
                Direction = call.Direction ?? "Unknown",
                StartTime = call.StartTime ?? DateTime.MinValue,
                EndTime = call.EndTime ?? DateTime.MinValue,
                Duration = int.Parse(call.Duration),
                RecordingUrl = string.Empty,
                RecordingSid = string.Empty,
                RecordingDuration = 0,
            };
            
            var regex = new Regex(@"sip:(\+?\d+)@");
            var match = regex.Match(call.To);

            if (match.Success)
            {
                string phoneNumber = match.Groups[1].Value;

                // If the phone number doesn't start with '+', prepend it
                if (!phoneNumber.StartsWith("+"))
                {
                    phoneNumber = "+" + phoneNumber;
                }
                
                if (phoneNumber.StartsWith("+04"))
                {
                    phoneNumber = phoneNumber.Replace("+04", "+614");
                }
                
                if (phoneNumber.StartsWith("+4"))
                {
                    phoneNumber = phoneNumber.Replace("+4", "+614");
                }
                
                callLog.To = phoneNumber;
            }

            if (await RecordExistsInDb(callLog))
            {
                existingCount++;
                await db.CopyNew().Updateable(callLog).Where(x => x.CallSid == callLog.CallSid).ExecuteCommandAsync();
            }
            else
            {
                await db.CopyNew().Insertable(callLog).ExecuteCommandAsync();
                newRecordsCount++;
            }
        }

        var log = new CallLogQueryLog
        {
            StartTime = startTime,
            EndTime = endTime,
            FetchedCount = calls.Count(),
            ExistingCount = existingCount,
            NewRecordsCount = newRecordsCount,
            Completed = true
        };
        logger.LogDebug($"{log.StartTime} - {log.EndTime}: Records fetched - {log.FetchedCount}, Records already in DB - {log.ExistingCount}, New records - {log.NewRecordsCount}");
        await db.Insertable(log).ExecuteCommandAsync();
        
        logger.LogDebug("Ending FetchCallLogJob at {time}", DateTime.Now.ToString("HH:mm:ss"));
        
        return this.UnifiedOk(new
        {
            newRecordsCount,
            existingCount,
            fetchedCount = calls.Count()
        });
    }
    
    private async Task<bool> RecordExistsInDb(CallLog callLog)
    {
        return await db.CopyNew().Queryable<CallLog>().AnyAsync(x => x.CallSid == callLog.CallSid);
    }
    
    public async Task<IActionResult> GetAllLogs()
    {
        var logs = await db.Queryable<CallLog>().ToListAsync();

        return this.UnifiedOk(logs);
    }
}