using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Shared.Middlewares;

public class CustomAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomAuthenticationMiddleware> _logger;
    private readonly IConfiguration _configuration;

    public CustomAuthenticationMiddleware(RequestDelegate next, ILogger<CustomAuthenticationMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        {
            await _next(context);
            return;
        }

        if (endpoint?.Metadata?.GetMetadata<IAuthorizeData>() != null)
        {
            var accessToken = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogWarning($"No access token provided for request to {context.Request.Path}");
                context.Response.StatusCode = 401;
                return;
            }

            // Here you can add more logic to inspect the access token and log more detailed information

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException()))
                };

                tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out _);
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogWarning($"Expired access token provided for request to {context.Request.Path}");
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = 401;
                throw;
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                _logger.LogWarning($"Invalid signature in access token for request to {context.Request.Path}");
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = 401;
                throw;
            }
            catch (SecurityTokenValidationException ex)
            {
                _logger.LogWarning($"Invalid access token provided for request to {context.Request.Path}");
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = 401;
                throw;
            }
        }

        await _next(context);
    }
}