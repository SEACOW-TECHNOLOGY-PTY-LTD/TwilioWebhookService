using System.ComponentModel;
using SqlSugar;

namespace Shared.Models.TwilioEvent;

public class TwilioEventTaskChannel : TwilioEventCallbacks
{
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
    public TwilioTaskChannelField? TaskChannelObj { get; set; }
}