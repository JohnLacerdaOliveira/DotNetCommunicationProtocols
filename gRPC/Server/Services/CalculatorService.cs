using Calculator;
using Grpc.Core;
using static Calculator.CalculatorService;

namespace gRPC.Server.Services
{
    public class CalculatorService : CalculatorServiceBase
    {
        public override Task<SimpleCalculationResponse> SimpleCalculation(
            SimpleCalculationRequest request,
            ServerCallContext context)
        {
            double result = 0;

            Console.WriteLine($"Received calculation request:" +
                $" {request.Calculation.FirstNumber}" +
                $" {request.Calculation.Operation}" +
                $" {request.Calculation.SecondNumber}");

            switch (request.Calculation.Operation)
            {
                case Operation.Add:
                    result = request.Calculation.FirstNumber + request.Calculation.SecondNumber;
                    break;
                case Operation.Subtract:
                    result = request.Calculation.FirstNumber - request.Calculation.SecondNumber;
                    break;
                case Operation.Multiply:
                    result = request.Calculation.FirstNumber * request.Calculation.SecondNumber;
                    break;
                case Operation.Divide:

                    if (request.Calculation.SecondNumber < 0)
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Division by zero is not allowed."));

                    result = request.Calculation.FirstNumber / request.Calculation.SecondNumber;
                    break;
            }

            return Task.FromResult(new SimpleCalculationResponse() { Result = result });
        }

        public override Task<SquareRootResponse> SquareRoot(SquareRootRequest request, ServerCallContext context)
        {
            if (request.Number < 0)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Cannot calculate square root of a negative number."));
            }

            var result = Math.Sqrt(request.Number);
            Console.WriteLine($"Calculated square root for {request.Number}: {result}");

            return Task.FromResult(new SquareRootResponse() { Result = result });
        }

        public override async Task DecomposeIntoPrimes(
            PrimeFactorizationRequest request,
            IServerStreamWriter<PrimeFactorizationResponse> responseStream,
            ServerCallContext context)
        {
            int number = request.Number;
            int divisor = 2;

            Console.WriteLine($"Starting prime decomposition for: {number}");

            while (number > 1)
            {
                // Check if the client cancelled the request
                if (context.CancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Cancellation requested by client.");
                    break;  // gracefully exit the loop
                }

                if (number % divisor == 0)
                {
                    number /= divisor;
                    await responseStream.WriteAsync(new PrimeFactorizationResponse { PrimeFactor = divisor });
                    await Task.Delay(500);
                }
                else
                {
                    divisor++;
                }
            }

            Console.WriteLine("Prime decomposition finished or cancelled.");
        }


        public override async Task<AverageCalculationResponse> AverageCalculation(
            IAsyncStreamReader<AverageCalculationRequest> requestStream,
            ServerCallContext context)
        {
            var sum = 0.0;
            var count = 0;

            Console.WriteLine("Starting to receive average request from client...");

            await foreach (var number in requestStream.ReadAllAsync(context.CancellationToken))
            {
                var request = requestStream.Current;

                // Check if the client cancelled the request
                if (context.CancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Cancellation requested by client.");
                    break;
                }

                Console.WriteLine($"Received average number: {request.Number}");

                sum += request.Number;
                count++;
            }

            return new AverageCalculationResponse()
            {
                AverageResult = count == 0 ? 0 : sum / count
            };
        }

        public override async Task FindCurrentMax(
            IAsyncStreamReader<FindCurrentMaxRequest> requestStream,
            IServerStreamWriter<FindCurrentMaxResponse> responseStream,
            ServerCallContext context)
        {
            var currentMax = double.MinValue;

            Console.WriteLine("Starting to receive find max request from client...");

            await foreach(var request in requestStream.ReadAllAsync(context.CancellationToken))
            {
                // Check if the client cancelled the request
                if (context.CancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Cancellation requested by client.");
                    break;
                }

                if (request.Number > currentMax) currentMax = request.Number;

                Console.WriteLine($"Received number: {request.Number}");

                // Send the current maximum number back to the client
                await responseStream.WriteAsync(new FindCurrentMaxResponse { MaxValue = currentMax });
            }
        }
    }
}
