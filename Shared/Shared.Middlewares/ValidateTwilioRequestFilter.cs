using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Twilio.Security;

namespace Shared.Middlewares;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ValidateTwilioRequestAttribute : TypeFilterAttribute
{
    public ValidateTwilioRequestAttribute() : base(typeof(ValidateTwilioRequestFilter))
    {
    }
}

internal class ValidateTwilioRequestFilter : IAsyncActionFilter
{
    private readonly RequestValidator _requestValidator;
    private readonly ILogger<ValidateTwilioRequestAttribute> _logger;
    private readonly bool _isEnabled;

    public ValidateTwilioRequestFilter(IConfiguration configuration, ILogger<ValidateTwilioRequestAttribute> logger)
    {
        _logger = logger;
        var authToken = configuration["Twilio:Client:AuthToken"] ?? throw new Exception("'Twilio:Client:AuthToken' not configured.");
        _requestValidator = new RequestValidator(authToken);
        _isEnabled = configuration.GetValue("Twilio:RequestValidation:Enabled", true);
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!_isEnabled)
        {
            await next();
            return;
        }
            
        var httpContext = context.HttpContext;
        var request = httpContext.Request;
        
        var requestUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
        
        _logger.LogInformation("Request URL: " + requestUrl);
        
        Dictionary<string, string> parameters = null;
        
        if (request.HasFormContentType)
        {
            var form = await request.ReadFormAsync(httpContext.RequestAborted).ConfigureAwait(false);
            parameters = form.ToDictionary(p => p.Key, p => p.Value.ToString());
        }

        var signature = request.Headers["X-Twilio-Signature"];
        
        _logger.LogInformation("Signature: " + signature);
        
        var isValid = _requestValidator.Validate(requestUrl, parameters, signature);
        
        _logger.LogInformation("Request is valid: " + isValid);
        
        if (!isValid)
        {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        await next();
    }
}