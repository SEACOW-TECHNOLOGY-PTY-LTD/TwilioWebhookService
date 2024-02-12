using System.ComponentModel;
using SqlSugar;

namespace Shared.Models.TwilioEvent;

public class TwilioEventTask : TwilioEventCallbacks
{
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
    
    [Description("TaskQueue Sid")]
    [SugarColumn(ColumnDescription = "TaskQueue Sid", IsNullable = true)]
    public string? TaskQueueSid { get; set; }
    
    [Description("TaskQueue Name")]
    [SugarColumn(ColumnDescription = "TaskQueue Name", IsNullable = true)]
    public string? TaskQueueName { get; set; }
    
    [Description("TaskChannel Sid")]
    [SugarColumn(ColumnDescription = "TaskChannel Sid", IsNullable = true)]
    public string? TaskChannelSid { get; set; }    // The reason that task was completed
    
    [Description("TaskChannel UniqueName")]
    [SugarColumn(ColumnDescription = "TaskChannel UniqueName", IsNullable = true)]
    public string? TaskChannelUniqueName { get; set; }    // The reason that task was completed
    
    [Description("Workflow Sid")]
    [SugarColumn(ColumnDescription = "Workflow Sid", IsNullable = true)]
    public string? WorkflowSid { get; set; }
    
    [Description("Workflow Name")]
    [SugarColumn(ColumnDescription = "Workflow Name", IsNullable = true)]
    public string? WorkflowName { get; set; }
    
    [Description("TaskQueue Target Expression")]
    [SugarColumn(ColumnDescription = "TaskQueue Target Expression", IsNullable = true)]
    public string? TaskQueueTargetExpression { get; set; }
    
    [SugarColumn(IsIgnore = true)]
    public TwilioTaskField? TaskObj { get; set; }
}