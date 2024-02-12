using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shared.Models.TwilioEvent;

public class TwilioTaskField
{
    public string? TaskSid { get; set; }                // The SID of the task that changed
    public string? TaskAttributes { get; set; }         // The JSON Attributes of the task
    public string? TaskAge { get; set; }                // The age of the task
    public string? TaskPriority { get; set; }           // The priority of the task
    public string? TaskAssignmentStatus { get; set; }   // The assignment status of the task
    public string? TaskCanceledReason { get; set; }     // The reason that task was canceled
    public string? TaskCompletedReason { get; set; }    // The reason that task was completed
    public string? TaskVersion { get; set; }    // The reason that task was completed
    public string? OperatingUnitSid { get; set; }    // The reason that task was completed
    
    public string? TrackingId { get; set; }                        // The Trace ID of the event
    
    public Dictionary<string, object>? GetTaskAttributes()
    {
        var taskAttributes = string.Empty;
        try
        {
            if (!string.IsNullOrEmpty(TaskAttributes))
            {
                taskAttributes = new string(TaskAttributes.Where(c => !char.IsControl(c)).ToArray());
                taskAttributes = taskAttributes.Replace("\\\"", "\"");
            }
            if (string.IsNullOrEmpty(taskAttributes))
            {
                return null;
            }
        
            var jObject = JObject.Parse(taskAttributes);
            return ConvertJObjectToDictionary(jObject);
        }
        catch (JsonReaderException jsonEx) 
        {
            // Handle the JSON deserialization error specifically
            Console.Error.WriteLineAsync($"Error deserializing message: {jsonEx.Message}");
            Console.Error.WriteLineAsync($"Malformed message: {TaskAttributes}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }

    private Dictionary<string, object> ConvertJObjectToDictionary(JObject jObject)
    {
        var values = new Dictionary<string, object>();

        foreach (var pair in jObject)
        {
            switch (pair.Value)
            {
                case JValue jValue:
                {
                    if (jValue.Value != null) values[pair.Key] = jValue.Value;
                    break;
                }
                case JObject value:
                    values[pair.Key] = ConvertJObjectToDictionary(value);
                    break;
                case JArray:
                    values[pair.Key] = pair.Value;  // Or you can further process JArray as needed
                    break;
            }
        }

        return values;
    }
}