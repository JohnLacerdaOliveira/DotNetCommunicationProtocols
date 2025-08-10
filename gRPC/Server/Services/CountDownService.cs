using Countdown;
using Grpc.Core;
using static Countdown.CountDownService;

namespace gRPC.Server.Services
{
    internal class CountDownService : CountDownServiceBase
    {
        public override async Task StartServerCountDown(ServerCountDownRequest request, IServerStreamWriter<ServerCountDownResponse> responseStream, ServerCallContext context)
        {
            Console.WriteLine($"Received countdown request for {request} seconds.");

            var remainingSeconds = request.Seconds;
            while (remainingSeconds >= 0)
            {
                await Task.Delay(1000);
                await responseStream.WriteAsync(new ServerCountDownResponse() { Message = remainingSeconds });

                remainingSeconds--;
            }
        }

        public override async Task<ClientCountDownResponse> StartClientCountDown(IAsyncStreamReader<ClientCountDownRequest> requestStream, ServerCallContext context)
        {
            Console.WriteLine("Starting to receive countdown from client...");

            int lastNumber = -1;

            // Read the streamed countdown numbers from the client
            while (await requestStream.MoveNext())
            {
                var request = requestStream.Current;
                Console.WriteLine($"Received number: {request.Seconds}");
                lastNumber = request.Seconds;
            }

            string finalMessage = lastNumber == 0
                ? "Countdown finished!"
                : "Countdown ended prematurely.";

            Console.WriteLine(finalMessage);

            // Return a single response after the client finishes streaming
            return new ClientCountDownResponse() { Message = finalMessage };
        }
    }
}
