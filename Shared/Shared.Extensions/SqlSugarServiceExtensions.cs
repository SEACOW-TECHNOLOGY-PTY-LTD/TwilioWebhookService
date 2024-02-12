using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data;
using SqlSugar;

namespace Shared.Extensions;

public static class SqlSugarServiceExtensions
{
    public static void AddSqlSugarService(this IServiceCollection services, IConfiguration configuration)
    {
        var dbConnection = configuration.GetConnectionString("Default");
        if (string.IsNullOrEmpty(dbConnection)) throw new Exception("数据库连接字符串为空");

        services.AddSingleton<ISqlSugarClient>(s =>
        {
            var sqlSugar = new SqlSugarScope(new ConnectionConfig()
                {
                    DbType = DbType.MySql,
                    ConnectionString = dbConnection,
                    IsAutoCloseConnection = true,
                },
                db =>
                {
                    db.Ado.CommandTimeOut = 30;

                    db.Aop.DataExecuting = (oldValue, entityInfo) =>
                    {
                        switch (entityInfo.OperationType)
                        {
                            case DataFilterType.InsertByObject:
                                switch (entityInfo.PropertyName)
                                {
                                    case "CreatedAt":
                                    case "ProcessAt":
                                    case "PublishAt":
                                    case "HandledAt":
                                        entityInfo.SetValue(DateTime.Now);
                                        break;

                                    case "UpdatedAt":
                                        entityInfo.SetValue(DateTime.Now);
                                        break;
                                }

                                break;

                            case DataFilterType.UpdateByObject:
                                switch (entityInfo.PropertyName)
                                {
                                    case "UpdatedAt":
                                        entityInfo.SetValue(DateTime.Now);
                                        break;
                                }

                                break;

                            case DataFilterType.DeleteByObject:
                                break;

                            default:
                                Console.WriteLine("未知操作类型");
                                break;
                        }
                    };
                });
            return sqlSugar;
        });

        services.AddScoped(typeof(Repository<>));
    }
}