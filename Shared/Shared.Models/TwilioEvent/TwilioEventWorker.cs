using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using SqlSugar;

namespace Shared.Models.TwilioEvent;

public class TwilioEventWorker : TwilioEventCallbacks
{
    [Description("Worker")]
    [SugarColumn(ColumnDescription = "Worker", IsJson = true, IsNullable = true)] //必填
    public Dictionary<string, string>? Worker
    {
        get
        {
            if (WorkerObj == null)
                return null;

            var dictionary = new Dictionary<string, string>();
            var properties = WorkerObj.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(WorkerObj)?.ToString();
                if (value != null)
                {
                    dictionary[property.Name] = value;
                }
            }

            return dictionary;
        }
        set
        {
            if (value == null)
            {
                WorkerObj = null;
            }
            else
            {
                WorkerObj = new TwilioWorkerField();
                var properties = WorkerObj?.GetType().GetProperties();

                if (properties == null) return;
                
                foreach (var property in properties)
                {
                    if (value.TryGetValue(property.Name, out var propValue))
                    {
                        var convertedValue = Convert.ChangeType(propValue, property.PropertyType);
                        property.SetValue(WorkerObj, convertedValue);
                    }
                }
            }
        }
    }

    [Description("Task Channel")]
    [SugarColumn(ColumnDescription = "Task Channel", IsJson = true, IsNullable = true)] //必填
    public Dictionary<string, string>? TaskChannel
    {
        get
        {
            if (TaskChannelObj == null)
                return null;

            var dictionary = new Dictionary<string, string>();
            var properties = TaskChannelObj.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(TaskChannelObj)?.ToString();
                if (value != null)
                {
                    dictionary[property.Name] = value;
                }
            }

            return dictionary;
        }
        set
        {
            if (value == null)
            {
                TaskChannelObj = null;
            }
            else
            {
                TaskChannelObj = new TwilioTaskChannelField();
                var properties = TaskChannelObj?.GetType().GetProperties();

                if (properties == null) return;
                
                foreach (var property in properties)
                {
                    if (value.TryGetValue(property.Name, out var propValue))
                    {
                        var convertedValue = Convert.ChangeType(propValue, property.PropertyType);
                        property.SetValue(TaskChannelObj, convertedValue);
                    }
                }
            }
        }
    }
    
    [SugarColumn(IsIgnore = true)]
    public TwilioWorkerField? WorkerObj { get; set; }
    
    [SugarColumn(IsIgnore = true)]
    public TwilioTaskChannelField? TaskChannelObj { get; set; }
}