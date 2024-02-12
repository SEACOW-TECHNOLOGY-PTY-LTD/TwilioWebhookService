using System.Reflection;
using Amazon;
using Amazon.CloudWatchLogs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Formatting.Json;
using Serilog.Sinks.AwsCloudWatch;
using Serilog.Sinks.SystemConsole.Themes;

namespace Shared.Helpers;

public static class CustomLoggerFactory
{
    private static ILogger? _logger;

    public static void Initialize(IWebHostEnvironment environment, string dsn)
    {
        var serviceName = Assembly.GetEntryAssembly()?.GetName().Name ?? "DefaultServiceName";
        var logGroupName = $"/{serviceName}/{environment.EnvironmentName}/serilog";

        _logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Debug()
            .WriteTo.Console(
                theme: AnsiConsoleTheme.Literate,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}"
            )
            .WriteTo.File(
                Path.Combine(Directory.GetCurrentDirectory(), "ServiceLogs", "log-.log"),
                rollingInterval: RollingInterval.Day, // 每天创建一个新的日志文件
                rollOnFileSizeLimit: true, // 当文件大小达到上限时，创建一个新文件
                fileSizeLimitBytes: 100 * 1024 * 1024, // 设定文件大小的上限，例如：10MB
                retainedFileCountLimit: null, // 可以设定保留的日志文件数量，null 表示不限制
                shared: true, // 允许多个进程写入同一文件
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}"
            )
            .WriteTo.AmazonCloudWatch(
                // The name of the log group to log to
                logGroup: logGroupName,
                // A string that our log stream names should be prefixed with. We are just specifying the
                // start timestamp as the log stream prefix
                logStreamPrefix: DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                // (Optional) Maximum number of log events that should be sent in a batch to AWS CloudWatch
                batchSizeLimit: 100,
                // (Optional) The maximum number of log messages that are stored locally before being sent
                // to AWS Cloudwatch
                queueSizeLimit: 10000,
                // (Optional) Similar to above, except the maximum amount of time that should pass before
                // log events must be sent to AWS CloudWatch
                batchUploadPeriodInSeconds: 15,
                // (Optional) If the log group does not exists, should we try create it?
                createLogGroup: true,
                // (Optional) The number of attempts we should make when logging log events that fail
                maxRetryAttempts: 3,
                // (Optional) Specify the time that logs should be kept for in AWS CloudWatch
                logGroupRetentionPolicy: LogGroupRetentionPolicy.OneMonth,
                // (Optional) Specify a custom text formatter for the output message.
                textFormatter: new JsonFormatter(),
                // The AWS CloudWatch client to use
                cloudWatchClient: new AmazonCloudWatchLogsClient("AKIA4I364CG7MEKZB2KT", "U5PtVK7FZjLkF49SFfGYNzIFDADEOQYk4e9OMBiQ", RegionEndpoint.APSoutheast2)
            )
            .WriteTo.Sentry(o =>
            {
                o.MinimumBreadcrumbLevel = LogEventLevel.Debug;
                o.MinimumEventLevel = LogEventLevel.Error;

                // A Sentry Data Source Name (DSN) is required.
                // See https://docs.sentry.io/product/sentry-basics/dsn-explainer/
                // You can set it in the SENTRY_DSN environment variable, or you can set it in code here.
                o.Dsn = dsn;

                // When debug is enabled, the Sentry client will emit detailed debugging information to the console.
                // This might be helpful, or might interfere with the normal operation of your application.
                // We enable it here for demonstration purposes when first trying Sentry.
                // You shouldn't do this in your applications unless you're troubleshooting issues with Sentry.
                o.Debug = true;

                // This option is recommended. It enables Sentry's "Release Health" feature.
                o.AutoSessionTracking = true;

                // Enabling this option is recommended for client applications only. It ensures all threads use the same global scope.
                o.IsGlobalModeEnabled = false;

                // This option will enable Sentry's tracing features. You still need to start transactions and spans.
                o.EnableTracing = true;

                // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
                // We recommend adjusting this value in production.
                o.TracesSampleRate = 1.0;

                o.Environment = environment.EnvironmentName;
                o.CaptureFailedRequests = true;
                o.SendDefaultPii = true;
            })
            .CreateLogger();
    }

    public static ILogger GetLogger() => _logger ?? throw new InvalidOperationException("Logger is not initialized. Call Initialize() first.");
}