using System.ComponentModel;
using SqlSugar;

namespace Shared.Models.TwilioEvent;

public class TwilioEventTaskQueue : TwilioEventCallbacks
{
    [Description("Task Queue Sid")]
    [SugarColumn(ColumnDescription = "Task Queue Sid", IsNullable = true)]
    public string? TaskQueueSid { get; set; }
    
    [Description("Task Queue Name")]
    [SugarColumn(ColumnDescription = "Task Queue Name", IsNullable = true)]
    public string? TaskQueueName { get; set; }
    
    [Description("Task Queue Target Expression")]
    [SugarColumn(ColumnDescription = "Task Queue Target Expression", IsNullable = true)]
    public string? TaskQueueTargetExpression { get; set; }
    
    [Description("Operating Unit Sid")]
    [SugarColumn(ColumnDescription = "Operating Unit Sid", IsNullable = true)]
    public string? OperatingUnitSid { get; set; }
    
    [Description("Workflow Sid")]
    [SugarColumn(ColumnDescription = "Workflow Sid", IsNullable = true)]
    public string? WorkflowSid { get; set; }
    
    [Description("Task")]
    [SugarColumn(ColumnDescription = "Task", IsJson = true, IsNullable = true)]//必填
    public Dictionary<string, string>? Task
    {
        get
        {
            if (TaskObj == null)
                return null;

            var dictionary = new Dictionary<string, string>();
            var properties = TaskObj.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(TaskObj)?.ToString();
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
                TaskObj = null;
            }
            else
            {
                TaskObj = new TwilioTaskField();
                var properties = TaskObj?.GetType().GetProperties();

                if (properties == null) return;
                
                foreach (var property in properties)
                {
                    if (value.TryGetValue(property.Name, out var propValue))
                    {
                        var convertedValue = Convert.ChangeType(propValue, property.PropertyType);
                        property.SetValue(TaskObj, convertedValue);
                    }
                }
            }
        }
    }
    
    [SugarColumn(IsIgnore = true)]
    public TwilioTaskField? TaskObj { get; set; }
}