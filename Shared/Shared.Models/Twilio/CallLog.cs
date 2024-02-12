using System.ComponentModel;
using SqlSugar;

namespace Shared.Models.Twilio;

public class CallLog
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]   //数据库是自增才配自增 
    public int Id { get; set; }
    
    [Description("CallSid")]
    [SugarColumn(ColumnDescription = "CallSid")]
    public string CallSid { get; set; }
    
    [Description("From")]
    [SugarColumn(ColumnDescription = "From")]
    public string From { get; set; }
    
    [Description("To")]
    [SugarColumn(ColumnDescription = "To")]
    public string To { get; set; }
    
    [Description("CallStatus")]
    [SugarColumn(ColumnDescription = "CallStatus")]
    public string CallStatus { get; set; }
    
    [Description("Direction")]
    [SugarColumn(ColumnDescription = "Direction")]
    public string Direction { get; set; }
    
    [Description("StartTime")]
    [SugarColumn(ColumnDescription = "StartTime")]
    public DateTime StartTime { get; set; }
    
    [Description("EndTime")]
    [SugarColumn(ColumnDescription = "EndTime")]
    public DateTime EndTime { get; set; }
    
    [Description("Duration")]
    [SugarColumn(ColumnDescription = "Duration")]
    public int Duration { get; set; }
    
    [Description("RecordingUrl")]
    [SugarColumn(ColumnDescription = "RecordingUrl")]
    public string RecordingUrl { get; set; }
    
    [Description("RecordingSid")]
    [SugarColumn(ColumnDescription = "RecordingSid")]
    public string RecordingSid { get; set; }
    
    [Description("RecordingDuration")]
    [SugarColumn(ColumnDescription = "RecordingDuration")]
    public int RecordingDuration { get; set; }
}