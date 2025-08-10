using Grpc.Core;
using Grpc.Health.V1;
using static Grpc.Health.V1.HealthCheckService;

namespace gRPC.Server.Services
{
    public class HealthCheckService : HealthCheckServiceBase
    {
        public override Task<HealthCheckResponse> HealthCheck(
            HealthCheckRequest request, 
            ServerCallContext context)
        {
            Console.WriteLine("HealthCheck client request received");

            return Task.FromResult(new HealthCheckResponse
            {
                Status = HealthCheckResponse.Types.ServingStatus.Serving
            });
        }
    }
}
