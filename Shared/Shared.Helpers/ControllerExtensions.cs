using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Common;

namespace Shared.Helpers;

public static class ControllerExtensions
{
    // 有数据的正常响应
    public static IActionResult UnifiedOk<T>(this ControllerBase controller, T data, string message = "Success")
    {
        return controller.Ok(new Response<T>
        {
            Data = data,
            Message = message,
            Status = StatusCodes.Status200OK
        });
    }
    
    // 没有数据的正常响应
    public static IActionResult UnifiedOk(this ControllerBase controller, string message = "Success")
    {
        return controller.Ok(new Response
        {
            Message = message,
            Status = StatusCodes.Status200OK
        });
    }

    // 错误响应
    public static IActionResult UnifiedError(this ControllerBase controller, string errorMessage, int statusCode = 500)
    {
        controller.HttpContext.Response.StatusCode = statusCode;
        return new JsonResult(new Response
        {
            Message = errorMessage,
            Status = StatusCodes.Status500InternalServerError
        });
    }
    
    // 没有数据的正常响应
    public static IActionResult UnifiedNoContent(this ControllerBase controller, string message = "No Content")
    {
        controller.HttpContext.Response.StatusCode = StatusCodes.Status204NoContent;
        return new JsonResult(new Response
        {
            Message = message,
            Status = StatusCodes.Status204NoContent
        });
    }

    // NotFound 响应
    public static IActionResult UnifiedNotFound(this ControllerBase controller, string message = "Resource not found")
    {
        return controller.NotFound(new Response
        {
            Message = message,
            Status = StatusCodes.Status404NotFound
        });
    }

    // BadRequest 响应
    public static IActionResult UnifiedBadRequest(this ControllerBase controller, string message = "Bad request")
    {
        return controller.BadRequest(new Response
        {
            Message = message,
            Status = StatusCodes.Status400BadRequest
        });
    }
}