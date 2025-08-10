using gRPC.Server.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Security.Cryptography.X509Certificates;

namespace gRPC.Server
{
    public class Program
    {
        const int PORT = 50051;

        public static void Main(string[] args)
        {
            // Typically you get IConfiguration injected or built:
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>()  // or your startup class
                .Build();

            string pfxPassword = configuration["ServerCertificatePassword"]
                ?? throw new ArgumentNullException("No value for ServerCertificatePassword");

            // Convert your server.crt + server.key to a .pfx file (outside C#)
            // or load the certificate with private key in some other way.
            var serverCertificate = new X509Certificate2("ssl/server.pfx", pfxPassword);

            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenLocalhost(PORT, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                    listenOptions.UseHttps(serverCertificate);
                });
            });

            // Add services to the container.
            builder.Services.AddGrpc();
            builder.Services.AddGrpcReflection();


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.MapGrpcService<HealthCheckService>();
            app.MapGrpcService<GreetingService>();
            app.MapGrpcService<CalculatorService>();
            app.MapGrpcService<CountDownService>();

            if (app.Environment.IsDevelopment())
            {
                app.MapGrpcReflectionService();
            }
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}