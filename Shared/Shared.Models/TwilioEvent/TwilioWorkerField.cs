using Newtonsoft.Json;

namespace Shared.Models.TwilioEvent;

public class TwilioWorkerField
{
    public string? WorkerSid { get; set; }              // The SID of the worker that changed
    public string? WorkerName { get; set; }             // The Friendly Name of the worker
    public string? WorkerAttributes { get; set; }       // The JSON Attributes of the worker
    public string? WorkerActivitySid { get; set; }      // The new activity SID of the worker
    public string? WorkerActivityName { get; set; }     // The new activity friendly name of the worker
    public string? WorkerVersion { get; set; }          // The current worker version
    public string? OperatingUnitSid { get; set; }       // The SID of the operating unit to which the worker belongs
    
    public string? WorkerTimeInPreviousActivity { get; set; }       // The time spent in the previous activity, in seconds; truncated to zero decimals
    public string? WorkerTimeInPreviousActivityMs { get; set; }     // The time spent in previous activity , in milliseconds
    public string? WorkerPreviousActivitySid { get; set; }          // 	The previous activity sid prior to this state change
    
    public string? WorkerChannelAvailable { get; set; }             // The availability of the channel
    public string? WorkerChannelAvailableCapacity { get; set; }     // The available capacity of the channel
    public string? WorkerChannelPreviousCapacity { get; set; }      // The previous capacity of the channel
    public string? TaskChannelSid { get; set; }                     // The associated channel sid
    public string? TaskChannelUniqueName { get; set; }              // The associated channel unique name
    public string? WorkerChannelTaskCount { get; set; }             // The number of assigned tasks to this worker on this channel
    
    public string? TrackingId { get; set; }                            // The Trace ID of the event
    
    public WorkerAttributes? GetWorkerAttributes()
    {
        try
        {
            return string.IsNullOrEmpty(WorkerAttributes) 
                ? null 
                : JsonConvert.DeserializeObject<WorkerAttributes>(WorkerAttributes);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return default;
        }
    }
}