using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Helpers;

public class TwilioTokenAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        
        var accountSid = configuration["Twilio:Client:AccountSid"] ?? throw new Exception("Twilio AccountSid is not configured.");
        var authToken = configuration["Twilio:Client:AuthToken"] ?? throw new Exception("Twilio AuthToken is not configured.");

        var token = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        var validator = new TwilioTokenValidator();

        try
        {
            await validator.ValidateTokenAsync(token, accountSid, authToken, realm: null);
        }
        catch (Exception ex)
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Result = new JsonResult(new { error = ex.Message });
        }
    }
}