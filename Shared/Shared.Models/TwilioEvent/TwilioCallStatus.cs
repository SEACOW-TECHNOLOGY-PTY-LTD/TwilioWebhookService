using System.ComponentModel;
using SqlSugar;

namespace Shared.Models.TwilioEvent;

public class TwilioCallStatus
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)] //数据库是自增才配自增 
    public int Id { get; set; }

    [Description("Call Sid")]
    [SugarColumn(ColumnDescription = "Call Sid", IsNullable = true)]
    public string? CallSid { get; set; }

    [Description("Account Sid")]
    [SugarColumn(ColumnDescription = "Account Sid", IsNullable = true)]
    public string? AccountSid { get; set; }
    
    [Description("Forward From")]
    [SugarColumn(ColumnDescription = "Forward From", IsNullable = true)]
    public string? ForwardFrom { get; set; }
    
    [Description("From")]
    [SugarColumn(ColumnDescription = "From", IsNullable = true)]
    public string? From { get; set; }
    
    [Description("From Formatted")]
    [SugarColumn(ColumnDescription = "From Formatted", IsNullable = true)]
    public string? FromFormatted { get; set; }
    
    [Description("To")]
    [SugarColumn(ColumnDescription = "To", IsNullable = true)]
    public string? To { get; set; }
    
    [Description("To Formatted")]
    [SugarColumn(ColumnDescription = "To Formatted", IsNullable = true)]
    public string? ToFormatted { get; set; }
    
    [Description("Call Status")]
    [SugarColumn(ColumnDescription = "Call Status", IsNullable = true)]
    public string? CallStatus { get; set; }
    
    [Description("Api Version")]
    [SugarColumn(ColumnDescription = "Api Version", IsNullable = true)]
    public string? ApiVersion { get; set; }
    
    [Description("Direction")]
    [SugarColumn(ColumnDescription = "Direction", IsNullable = true)]
    public string? Direction { get; set; }
    
    [Description("Forwarded From")]
    [SugarColumn(ColumnDescription = "Forwarded From", IsNullable = true)]
    public string? ForwardedFrom { get; set; }
    
    [Description("Caller Name")]
    [SugarColumn(ColumnDescription = "Caller Name", IsNullable = true)]
    public string? CallerName { get; set; }
    
    [Description("Parent Call Sid")]
    [SugarColumn(ColumnDescription = "Parent Call Sid", IsNullable = true)]
    public string? ParentCallSid { get; set; }
    
    [Description("Duration")]
    [SugarColumn(ColumnDescription = "Duration", IsNullable = true)]
    public string? Duration { get; set; }
    
    [Description("Call Duration")]
    [SugarColumn(ColumnDescription = "Call Duration", IsNullable = true)]
    public string? CallDuration { get; set; }
    
    [Description("Sip Response Code")]
    [SugarColumn(ColumnDescription = "Sip Response Code", IsNullable = true)]
    public string? SipResponseCode { get; set; }
    
    [Description("Recording Url")]
    [SugarColumn(ColumnDescription = "Recording Url", IsNullable = true)]
    public string? RecordingUrl { get; set; }
    
    [Description("Recording Sid")]
    [SugarColumn(ColumnDescription = "Recording Sid", IsNullable = true)]
    public string? RecordingSid { get; set; }
    
    [Description("Recording Duration")]
    [SugarColumn(ColumnDescription = "Recording Duration", IsNullable = true)]
    public string? RecordingDuration { get; set; }
    
    [Description("Timestamp")]
    [SugarColumn(ColumnDescription = "Timestamp", IsNullable = true)]
    public string? Timestamp { get; set; }
    
    [Description("Callback Source")]
    [SugarColumn(ColumnDescription = "Callback Source", IsNullable = true)]
    public string? CallbackSource { get; set; }
    
    [Description("Sequence Number")]
    [SugarColumn(ColumnDescription = "Sequence Number", IsNullable = true)]
    public string? SequenceNumber { get; set; }
}