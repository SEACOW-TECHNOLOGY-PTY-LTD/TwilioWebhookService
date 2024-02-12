using System.ComponentModel;
using SqlSugar;

namespace Shared.Models.TwilioEvent;

public class TwilioEventActivity : TwilioEventCallbacks
{
    [Description("Activity Sid")]
    [SugarColumn(ColumnDescription = "Activity Sid", IsNullable = true)]
    public string? ActivitySid { get; set; }
    
    [Description("Activity Name")]
    [SugarColumn(ColumnDescription = "Activity Name", IsNullable = true)]
    public string? ActivityName { get; set; }
    
    [Description("Activity Available")]
    [SugarColumn(ColumnDescription = "Activity Available", IsNullable = true)]
    public string? ActivityAvailable { get; set; }
}