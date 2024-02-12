using System.ComponentModel;
using SqlSugar;

namespace Shared.Models.TwilioEvent;

public class TwilioEventCallbacks
{
    [SugarColumn(IsPrimaryKey = true)]   //数据库是自增才配自增 
    public string? Sid { get; set; }
    
    [Description("Event Type")]
    [SugarColumn(ColumnDescription = "Event Type", IsNullable = true)]
    public string? EventType { get; set; }          // An identifier for this event
    
    [Description("Account Sid")]
    [SugarColumn(ColumnDescription = "Account Sid", IsNullable = true)]
    public string? AccountSid { get; set; }         // The account owning this event
    
    [Description("Workspace Sid")]
    [SugarColumn(ColumnDescription = "Workspace Sid", IsNullable = true)]
    public string? WorkspaceSid { get; set; }       // The Workspace Sid generating this event
    
    [Description("Workspace Name")]
    [SugarColumn(ColumnDescription = "Workspace Name", IsNullable = true)]
    public string? WorkspaceName { get; set; }      // The Workspace Name generating this event
    
    [Description("Event Description")]
    [SugarColumn(ColumnDescription = "Event Description", IsNullable = true)]
    public string? EventDescription { get; set; }   // A description of the event
    
    [Description("Resource Type")]
    [SugarColumn(ColumnDescription = "Resource Type", IsNullable = true)]
    public string? ResourceType { get; set; }       // The type of object this event is most relevant to (Task, Reservation, Worker, Activity, Workflow, Workspace)
    
    [Description("Resource Sid")]
    [SugarColumn(ColumnDescription = "Resource Sid", IsNullable = true)]
    public string? ResourceSid { get; set; }        // The sid of the object this event is most relevant to (TaskSid, ReservationSid, WorkerSid, ActivitySid, WorkflowSid, WorkspaceSid)
    
    [Description("Timestamp")]
    [SugarColumn(ColumnDescription = "Timestamp", IsNullable = true)]
    public string? Timestamp { get; set; }          // The time this event was sent
    
    [Description("TimestampMs")]
    [SugarColumn(ColumnDescription = "TimestampMs", IsNullable = true)]
    public string? TimestampMs { get; set; }        // The time this event was sent
}