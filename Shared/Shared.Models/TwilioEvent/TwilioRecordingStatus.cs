using System.ComponentModel;
using SqlSugar;

namespace Shared.Models.TwilioEvent;

public class TwilioRecordingStatus
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)] //数据库是自增才配自增 
    public int Id { get; set; }

    [Description("Call Sid")]
    [SugarColumn(ColumnDescription = "Call Sid", IsNullable = true)]
    public string? CallSid { get; set; }
    
    [Description("Recording Sid")]
    [SugarColumn(ColumnDescription = "Recording Sid", IsNullable = true)]
    public string? RecordingSid { get; set; }
    
    [Description("Recording Url")]
    [SugarColumn(ColumnDescription = "Recording Url", IsNullable = true)]
    public string? RecordingUrl { get; set; }
    
    [Description("Recording Status")]
    [SugarColumn(ColumnDescription = "Recording Status", IsNullable = true)]
    public string? RecordingStatus { get; set; }
    
    [Description("Recording Duration")]
    [SugarColumn(ColumnDescription = "Recording Duration", IsNullable = true)]
    public string? RecordingDuration { get; set; }
    
    [Description("Recording Channels")]
    [SugarColumn(ColumnDescription = "Recording Channels", IsNullable = true)]
    public string? RecordingChannels { get; set; }
    
    [Description("Recording Start Time")]
    [SugarColumn(ColumnDescription = "Recording Start Time", IsNullable = true)]
    public string? RecordingStartTime { get; set; }
    
    [Description("Recording Source")]
    [SugarColumn(ColumnDescription = "Recording Source", IsNullable = true)]
    public string? RecordingSource { get; set; }
    
    [Description("Recording Track")]
    [SugarColumn(ColumnDescription = "Recording Track", IsNullable = true)]
    public string? RecordingTrack { get; set; }
}