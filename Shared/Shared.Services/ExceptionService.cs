using System.Net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Shared.Models.Common;

namespace Shared.Services;

public class ExceptionService
{
    public async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            // 如果响应已经开始，只记录错误然后返回。
            // 这里您可以添加自己的日志记录机制。
            Console.WriteLine(exception);
            Console.WriteLine("无法修改头部，响应已经开始。");
            return;
        }
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception switch
        {
            KeyNotFoundException e => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        var response = new Response
        {
            Status = context.Response.StatusCode,
            Message = exception.Message
        };

        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
    }
}