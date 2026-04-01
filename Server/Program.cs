using ProtoBuf.Grpc.Server;
using Server.Infrastructure;
using Server.Services;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register gRPC (code-first)
            builder.Services.AddCodeFirstGrpc();

            // Register connection manager (singleton = shared state)
            builder.Services.AddSingleton<ConnectionManager>();

            var app = builder.Build();

            // Map gRPC service
            app.MapGrpcService<ChatService>();

            app.Run();
        }
    }
}
