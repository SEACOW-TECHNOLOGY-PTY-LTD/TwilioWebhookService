using Microsoft.Extensions.Configuration;
using SqlSugar;

namespace Shared.Data;

public class SqlSugarHelper
{
    private static string? _connectionString;
    //如果是固定多库可以传 new SqlSugarScope(List<ConnectionConfig>,db=>{}) 文档：多租户
    //如果是不固定多库 可以看文档Saas分库
    public SqlSugarHelper(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default");
        if (string.IsNullOrEmpty(_connectionString)) throw new Exception("数据库连接字符串为空");
    }

    //用单例模式
    public static readonly SqlSugarScope Db = new SqlSugarScope(new ConnectionConfig()
        {
            ConnectionString = _connectionString,//连接符字串
            DbType = DbType.Sqlite,//数据库类型
            IsAutoCloseConnection = true //不设成true要手动close
        },
        db => {
            //(A)全局生效配置点
            //调试SQL事件，可以删掉
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(sql);//输出sql,查看执行sql
                //5.0.8.2 获取无参数化 SQL 
                //UtilMethods.GetSqlString(DbType.SqlServer,sql,pars)
            };
        });
}