using System.ComponentModel;
using SqlSugar;

namespace Shared.Models.Twilio;

public class CallbackLog
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]   //数据库是自增才配自增 
    public int Id { get; set; }
    
    [Description("TaskSid")]
    [SugarColumn(ColumnDescription = "TaskSid")]
    public string TaskSid { get; set; }
    
    [Description("From")]
    [SugarColumn(ColumnDescription = "From")]
    public string From { get; set; }
    
    [Description("To")]
    [SugarColumn(ColumnDescription = "To")]
    public string To { get; set; }
    
    [Description("CreatedTime")]
    [SugarColumn(ColumnDescription = "CreatedTime")]
    public DateTime CreatedTime { get; set; }
}