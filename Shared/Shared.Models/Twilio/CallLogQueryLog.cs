using System.ComponentModel;
using SqlSugar;

namespace Shared.Models.Twilio;

public class CallLogQueryLog
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]   //数据库是自增才配自增 
    public int Id { get; set; }
    
    [Description("StartTime")]
    [SugarColumn(ColumnDescription = "StartTime")]
    public DateTime StartTime { get; set; }
    
    [Description("EndTime")]
    [SugarColumn(ColumnDescription = "EndTime")]
    public DateTime EndTime { get; set; }
    
    [Description("Fetched Count")]
    [SugarColumn(ColumnDescription = "Fetched Count")]
    public int FetchedCount { get; set; }
    
    [Description("Existing Count")]
    [SugarColumn(ColumnDescription = "Existing Count")]
    public int ExistingCount { get; set; }
    
    [Description("NewRecordsCount")]
    [SugarColumn(ColumnDescription = "NewRecordsCount")]
    public int NewRecordsCount { get; set; }
    
    [Description("Completed")]
    [SugarColumn(ColumnDescription = "Completed")]
    public bool Completed { get; set; }
}