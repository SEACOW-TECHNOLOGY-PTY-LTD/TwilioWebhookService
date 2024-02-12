using System.ComponentModel;
using SqlSugar;

namespace Shared.Models.TwilioEvent;

public class TwilioEventWorkspace : TwilioEventCallbacks
{
    [Description("Workspace Sid")]
    [SugarColumn(ColumnDescription = "Workspace Sid", IsNullable = true)]
    public new string? WorkspaceSid { get; set; } 
    
    [Description("Workspace Name")]
    [SugarColumn(ColumnDescription = "Workspace Name", IsNullable = true)]
    public new string? WorkspaceName { get; set; } 
}