using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using Shared.Helpers;

namespace Shared.Extensions;

public static class LoggerExtensions
{
    public static void AddCustomLogger(this IServiceCollection services, IWebHostEnvironment environment, string dsn)
    {
        CustomLoggerFactory.Initialize(environment, dsn);
        var loggerProvider = new SerilogLoggerProvider(CustomLoggerFactory.GetLogger());
        services.AddSingleton<ILoggerProvider>(loggerProvider);
    }
}