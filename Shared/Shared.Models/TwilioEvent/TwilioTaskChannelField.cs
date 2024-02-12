namespace Shared.Models.TwilioEvent;

public class TwilioTaskChannelField
{
    public string? TaskChannelSid { get; set; }                 // The SID of the task-channel
    public string? TaskChannelName { get; set; }                // The Friendly Name of the task-channel
    public string? TaskChannelUniqueName { get; set; }          // An application-defined string that uniquely identifies the Task Channel, such as voice or sms
    public string? TaskChannelOptimizedRouting { get; set; }    // Whether the Task Channel should prioritize Workers that have been idle. If true, Workers that have been idle the longest are prioritized.
    
    public string? TrackingId { get; set; }                        // The Trace ID of the event
}