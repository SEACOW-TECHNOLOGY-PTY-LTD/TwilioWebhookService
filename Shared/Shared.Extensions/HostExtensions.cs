using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SqlSugar;

namespace Shared.Extensions;

public static class HostExtensions
{
    public static IHost EnsureDbInitialization<T>(this IHost host, string dbName) where T : class
    {
        using var scope = host.Services.CreateScope();
        
        var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();

        var dbList = db.DbMaintenance.GetDataBaseList();
        if (!dbList.Any(d => d.Equals(dbName))) db.DbMaintenance.CreateDatabase(dbName); // 建库：如果不存在创建数据库存在不会重复创建 

        // 检查LeadPushLog表是否存在
        var tableList = db.DbMaintenance.GetTableInfoList();
        var isTableExists = tableList.Any(d => d.Name.Equals(nameof(T)));

        if (!isTableExists) db.CodeFirst.InitTables(typeof(T)); // 不存在则创建

        return host;
    }

    public static IServiceCollection AddHostedService(this IServiceCollection services, Type serviceType)
    {
        var addHostedServiceMethod = typeof(ServiceCollectionHostedServiceExtensions)
            .GetMethods()
            .First(m => m.IsGenericMethod && m.Name == "AddHostedService");
        var genericMethod = addHostedServiceMethod.MakeGenericMethod(serviceType);
        genericMethod.Invoke(null, new object[] { services });
        
        return services;
    }
}