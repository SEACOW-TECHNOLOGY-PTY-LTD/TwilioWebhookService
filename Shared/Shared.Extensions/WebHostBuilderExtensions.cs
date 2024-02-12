using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Shared.Extensions;

public static class WebHostBuilderExtensions
{
    public static IWebHostBuilder ConfigureServerOptions(this IWebHostBuilder builder, IWebHostEnvironment environment, IConfiguration configuration, int httpPort, int httpsPort)
    {
        builder.ConfigureKestrel(serverOptions =>
        {
            serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
            
            var serverCertificate = LoadCertificateAndPrivateKey(configuration["Certificate:Path"], configuration["Certificate:KeyPath"]);
            
            serverOptions.ListenLocalhost(httpPort, listenOptions =>
            {
                listenOptions.UseConnectionLogging();
                listenOptions.Protocols = HttpProtocols.Http1;
            });

            if (environment.IsDevelopment())
            {
                serverOptions.ListenLocalhost(httpsPort, listenOptions =>
                {
                    listenOptions.UseConnectionLogging();
                    listenOptions.Protocols = HttpProtocols.Http1;
                    listenOptions.UseHttps();
                });
            }
            else
            {
                serverOptions.ListenLocalhost(httpsPort, listenOptions =>
                {
                    listenOptions.UseConnectionLogging();
                    listenOptions.Protocols = HttpProtocols.Http1;
                    listenOptions.UseHttps(serverCertificate ?? throw new InvalidOperationException("Certificate is not found."));
                });
            }
        });

        return builder;
    }
    
    public static IWebHostBuilder ConfigureServerOptions(this IWebHostBuilder builder, IWebHostEnvironment environment, IConfiguration configuration, int httpPort, int httpsPort, int grpcPort)
    {
        if (environment.IsDevelopment())
        {
            builder.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ListenLocalhost(httpPort, listenOptions =>
                {
                    listenOptions.UseConnectionLogging();
                    listenOptions.Protocols = HttpProtocols.Http1;
                });
            
                serverOptions.ListenLocalhost(httpsPort, listenOptions =>
                {
                    listenOptions.UseConnectionLogging();
                    listenOptions.Protocols = HttpProtocols.Http1;
                    listenOptions.UseHttps();
                });
                
                serverOptions.ListenLocalhost(grpcPort, listenOptions =>
                {
                    listenOptions.UseConnectionLogging();
                    listenOptions.Protocols = HttpProtocols.Http2;
                });
            });

            return builder;
        }
        
        builder.ConfigureKestrel(serverOptions =>
        {
            serverOptions.ListenLocalhost(httpPort, listenOptions =>
            {
                listenOptions.UseConnectionLogging();
                listenOptions.Protocols = HttpProtocols.Http1;
            });
            
            var serverCertificate = LoadCertificateAndPrivateKey(configuration["Certificate:Path"], configuration["Certificate:KeyPath"]);
            
            if (environment.IsDevelopment())
            {
                serverOptions.ListenLocalhost(httpsPort, listenOptions =>
                {
                    listenOptions.UseConnectionLogging();
                    listenOptions.Protocols = HttpProtocols.Http1;
                    listenOptions.UseHttps();
                });
                
                serverOptions.ListenLocalhost(grpcPort, listenOptions =>
                {
                    listenOptions.UseConnectionLogging();
                    listenOptions.Protocols = HttpProtocols.Http2;
                });
            }
            else
            {
                serverOptions.ListenLocalhost(httpsPort, listenOptions =>
                {
                    listenOptions.UseConnectionLogging();
                    listenOptions.Protocols = HttpProtocols.Http1;
                    listenOptions.UseHttps(serverCertificate ?? throw new InvalidOperationException("Certificate is not found."));
                });
                
                serverOptions.ListenLocalhost(grpcPort, listenOptions =>
                {
                    listenOptions.UseConnectionLogging();
                    listenOptions.Protocols = HttpProtocols.Http2;
                    listenOptions.UseHttps(serverCertificate ?? throw new InvalidOperationException("Certificate is not found."));
                });
            }
        });

        return builder;
    }
    
    private static X509Certificate2? LoadCertificateAndPrivateKey(string? certPath, string? keyPath)
    {
        if (string.IsNullOrEmpty(certPath) || string.IsNullOrEmpty(keyPath)) return default;
        
        var cert = new X509Certificate2(certPath);

        var privateKeyText = File.ReadAllText(keyPath);
        var privateKeyBlocks = privateKeyText.Split("-", StringSplitOptions.RemoveEmptyEntries);
        var privateKeyBytes = Convert.FromBase64String(privateKeyBlocks[1]);
        var rsa = System.Security.Cryptography.RSA.Create();
        rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);

        var certWithKey = cert.CopyWithPrivateKey(rsa);
        return new X509Certificate2(certWithKey.Export(X509ContentType.Pfx));
    }
}