using System.Text.Json.Nodes;
using CliWrap;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Twilio.Clients;
using Twilio.Rest.Taskrouter.V1;

namespace WebhookService.BackgroundServices;

public class TunnelService : BackgroundService
{
    private readonly IServer _server;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IConfiguration _config;
    private readonly ILogger<TunnelService> _logger;
    private readonly ITwilioRestClient _twilioRestClient;

    public TunnelService(IServer server, IHostApplicationLifetime hostApplicationLifetime, IConfiguration config, ILogger<TunnelService> logger, IConfiguration configuration)
    {
        _server = server;
        _hostApplicationLifetime = hostApplicationLifetime;
        _config = config;
        _logger = logger;
        _twilioRestClient = _twilioRestClient = new TwilioRestClient(configuration["Twilio:Client:AccountSid"], configuration["Twilio:Client:AuthToken"]);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await WaitForApplicationStarted();  // 等待应用程序启动完成

        var urls = _server.Features.Get<IServerAddressesFeature>()!.Addresses;  // 获取服务器地址列表
        // 如果已经通过 ngrok 进行了身份验证，则使用 https://，否则只能使用 http://
        var localUrl = urls.Single(u => u.StartsWith("http://"));  // 获取本地地址

        _logger.LogInformation("Starting ngrok tunnel for {LocalUrl}", localUrl);  // 启动 ngrok 隧道，使用本地地址
        var ngrokTask = StartNgrokTunnel(localUrl, stoppingToken);  // 启动 ngrok 隧道任务

        var publicUrl = await GetNgrokPublicUrl();  // 获取 ngrok 的公共 URL
        _logger.LogInformation("Public ngrok URL: {NgrokPublicUrl}", publicUrl);  // 输出 ngrok 的公共 URL
        
        await ConfigureTwilioWebhook(publicUrl);

        await ngrokTask;  // 等待 ngrok 隧道任务完成

        _logger.LogInformation("Ngrok tunnel stopped");  // 输出 ngrok 隧道已停止
    }
    
    private async Task ConfigureTwilioWebhook(string publicUrl)
    {
        try
        {
            var workspace = await WorkspaceResource.UpdateAsync(
                eventCallbackUrl: new Uri($"{publicUrl}/api/v1/event"),
                pathSid: _config["Twilio:WorkspaceSid"],
                client: _twilioRestClient
            );
            
            _logger.LogInformation(
                "Twilio TaskRouter Workspace Event callback URL updated to {EventCallbackUrl}",
                workspace.EventCallbackUrl
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Twilio TaskRouter Workspace Event callback URL: {ErrorMessage}", ex.Message);
        }
    }

    private Task WaitForApplicationStarted()
    {
        var completionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);  // 创建一个任务完成源，用于异步任务
        _hostApplicationLifetime.ApplicationStarted.Register(() => completionSource.TrySetResult());  // 在应用程序启动时注册回调函数，当应用程序启动时设置任务完成
        return completionSource.Task;  // 返回任务
    }

    private CommandTask<CommandResult> StartNgrokTunnel(string localUrl, CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Local URL: {localUrl}");
        var ngrokTask = Cli.Wrap("ngrok")  // 创建一个 ngrok 命令行任务
            .WithArguments(args => args
                .Add("http")  // 添加参数 "http"
                .Add(localUrl)  // 添加本地 URL 参数
                .Add("--log")  // 添加参数 "--log"
                .Add("stdout"))  // 将日志输出到标准输出
            .WithStandardOutputPipe(PipeTarget.ToDelegate(s => _logger.LogInformation(s)))  // 将标准输出管道连接到日志记录器，记录调试日志
            .WithStandardErrorPipe(PipeTarget.ToDelegate(s => _logger.LogError(s)))  // 将标准错误输出管道连接到日志记录器，记录错误日志
            .ExecuteAsync(stoppingToken);  // 异步执行 ngrok 命令行任务
        return ngrokTask;  // 返回 ngrok 任务
    }

    private async Task<string> GetNgrokPublicUrl()
    {
        using var httpClient = new HttpClient();  // 创建一个 HttpClient 实例

        for (var ngrokRetryCount = 0; ngrokRetryCount < 10; ngrokRetryCount++)  // 进行 10 次重试
        {
            _logger.LogDebug("Get ngrok tunnels attempt: {RetryCount}", ngrokRetryCount + 1);  // 记录获取 ngrok 隧道的尝试次数

            try
            {
                var json = await httpClient.GetFromJsonAsync<JsonNode>("http://127.0.0.1:4040/api/tunnels");  // 发送 GET 请求获取 ngrok 隧道信息的 JSON 数据
                var publicUrl = json?["tunnels"]
                    ?.AsArray()  // 从 JSON 数据中选择 "tunnels" 数组
                    .Select(e => e?["public_url"]?.GetValue<string>())  // 提取每个元素的 "public_url" 属性值
                    .SingleOrDefault(u => u != null && u.StartsWith("https://"));  // 选择以 "https://" 开头的第一个 URL
                if (!string.IsNullOrEmpty(publicUrl)) return publicUrl;  // 如果获取到有效的公共 URL，则返回它
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);  // 记录异常消息到日志
            }

            await Task.Delay(200);  // 延迟 200 毫秒后进行下一次尝试
        }

        throw new Exception("Ngrok dashboard did not start in 10 tries");  // 在 10 次重试后仍未获取到有效的公共 URL，抛出异常
    }
}