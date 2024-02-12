using System.ComponentModel;
using SqlSugar;

namespace Shared.Models.Log;

public class TwilioEventMessageLog
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]   //数据库是自增才配自增 
    public int Id { get; set; }
    
    [Description("消息的内容")]
    [SugarColumn(ColumnDescription = "消息的内容", Length = 5000)]
    public string? Message { get; set; }
    
    [Description("消息的类型")]
    [SugarColumn(ColumnDescription = "消息的类型", IsNullable = true)]
    public string? Exchange { get; set; }
    
    [Description("消息的路由键")]
    [SugarColumn(ColumnDescription = "消息的路由键", IsNullable = true)]
    public string? RoutingKey { get; set; }
    
    [Description("消息的队列名称")]
    [SugarColumn(ColumnDescription = "消息的队列名称")]
    public string? Queue { get; set; }
    
    [Description("消息的追踪编号")]
    [SugarColumn(ColumnDescription = "消息的追踪编号")]
    public string? EventSid { get; set; }
    
    [Description("消息的发送时间")]
    [SugarColumn(ColumnDescription = "消息的发送时间", ColumnDataType = "DATETIME(3)")]
    public DateTime PublishAt { get; set; }
    
    [Description("消息的消费时间")]
    [SugarColumn(ColumnDescription = "消息的消费时间", IsNullable = true, ColumnDataType = "DATETIME(3)")]
    public DateTime? ConsumedAt { get; set; }
    
    [Description("处理结果")]
    [SugarColumn(ColumnDescription = "处理结果")]
    public string? ProcessResult { get; set; }
    
    [Description("错误消息")]
    [SugarColumn(ColumnDescription = "错误消息", IsNullable = true, Length = 3000)]
    public string? ErrorMessages { get; set; }
}