using Microsoft.AspNetCore.Http;
using Shared.Services;

namespace Shared.Middlewares;

public class CustomExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ExceptionService _exceptionService;

    public CustomExceptionHandlerMiddleware(RequestDelegate next, ExceptionService exceptionService)
    {
        _next = next;
        _exceptionService = exceptionService;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            if (httpContext.Response.StatusCode != StatusCodes.Status204NoContent)
            {
                await _next(httpContext);
            }
        }
        catch (Exception ex)
        {
            if (httpContext.Response.StatusCode != StatusCodes.Status204NoContent)
            {
                await _exceptionService.HandleExceptionAsync(httpContext, ex);
            }
            // 如果您希望在HTTP 204响应的情况下也进行某种处理，可以在这里添加代码
        }
    }
}