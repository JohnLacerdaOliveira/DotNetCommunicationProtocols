using Greet;
using Grpc.Core;
using static Greet.GreetingService;

namespace gRPC.Server.Services
{
    public class GreetingService : GreetingServiceBase
    {
        public override Task<GreetingResponse> Greet(GreetingRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Received greeting request for {request.Greeting.FirstName} {request.Greeting.LastName}");

            string result = $"Hello {request.Greeting.FirstName} {request.Greeting.LastName}";
            return Task.FromResult(new GreetingResponse() { Result = result });
        }
    }
}
