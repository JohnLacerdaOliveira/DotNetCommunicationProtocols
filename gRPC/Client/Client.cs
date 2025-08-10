using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;

namespace gRPC.Client;

    internal class GrpcClient
{
    internal async Task RunClient(
         string serverAddress,
         GrpcChannelOptions channelOptions,
         int deadlineForConnection)
    {
        var channel = await ConnectToServerAsync(serverAddress, channelOptions, deadlineForConnection);

        Console.WriteLine("----------------GREET-----------------");
        GreetClientRequest("John", "Lacerda", channel);

        Console.WriteLine("----------------CALCULATOR-----------------");
        SimpleCalculationClientRequest(10, Calculator.Operation.Multiply, 20, channel);
        SquareRoot(25, channel);

        await PrimeFactorizationClientRequestAsync(120, channel);
        await AverageCalculationAsync(new[] { 10.0, 20.0, 30.0, 40.0, 50.0 }, channel);
        await FindCurrentMaxAsync(new[] { 30.0, 10.0, 20.0, 30.0, 40.0, 50.0 }, channel);

        Console.WriteLine("----------------COUNTDOWN-----------------");
        await ServerCountDownRequestAsync(10, channel);
        await ClientCountDownRequestAsync(20, channel);

        await channel.ShutdownAsync();
    }

    internal async Task<GrpcChannel> ConnectToServerAsync(
         string serverAddress,
         GrpcChannelOptions channelOptions,
         int deadlineForConnection)
    {
        try
        {
            var channel = GrpcChannel.ForAddress(serverAddress, channelOptions);

            var client = new Grpc.Health.V1.HealthCheckService.HealthCheckServiceClient(channel);
            var request = new Grpc.Health.V1.HealthCheckRequest { Service = "" };

            var response = await client.HealthCheckAsync(request, deadline: DateTime.UtcNow.AddSeconds(deadlineForConnection));

            Console.WriteLine($"Successfully connected to server at {serverAddress}");
            return channel;
        }
        catch (RpcException ex)
        {
            Console.WriteLine($"gRPC server connection failed: {ex.Status.Detail}");
            throw;
        }
        catch (UriFormatException ex)
        {
            Console.WriteLine($"Invalid server address '{serverAddress}': {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error creating gRPC channel: {ex.Message}");
            throw;
        }
    }

    private void HealthCheck(GrpcChannel channel)
    {
        var client = new Grpc.Health.V1.HealthCheckService.HealthCheckServiceClient(channel);
        var request = new Grpc.Health.V1.HealthCheckRequest { Service = "" };

        try
        {
            var response = client.HealthCheck(request, deadline: DateTime.UtcNow.AddSeconds(5));

            Console.WriteLine($"Health check response: {response.Status}");
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
        {
            Console.WriteLine($"Health check failed: {ex.StatusCode}");
        }
        catch (RpcException ex)
        {
            Console.WriteLine($"Health check failed: {ex.Status.Detail}");
        }
    }


    private void GreetClientRequest(string firstName, string lastName, GrpcChannel channel)
    {
        Console.WriteLine("Client sending greeting request...");

        var client = new Greet.GreetingService.GreetingServiceClient(channel);
        var request = new Greet.GreetingRequest
        {
            Greeting = new Greet.Greeting { FirstName = firstName, LastName = lastName }
        };

        var response = client.Greet(request);

        Console.WriteLine($"Response from server: {response.Result}");

    }

    private void SimpleCalculationClientRequest(double firstNumber, Calculator.Operation operation, double secondNumber, GrpcChannel channel)
    {
        Console.WriteLine($"Client sending a simple calculation request:" +
             $" {firstNumber}" +
             $" {operation}" +
             $" {secondNumber}");

        var client = new Calculator.CalculatorService.CalculatorServiceClient(channel);
        var request = new Calculator.SimpleCalculationRequest
        {
            Calculation = new Calculator.SimpleCalculation
            {
                FirstNumber = firstNumber,
                Operation = operation,
                SecondNumber = secondNumber,
            }
        };

        var response = client.SimpleCalculation(request);

        Console.WriteLine($"Reponse from Server is {response}");
    }

    private void SquareRoot(double number, GrpcChannel channel)
    {
        Console.WriteLine($"Client sending a square root request:{number}");

        var client = new Calculator.CalculatorService.CalculatorServiceClient(channel);
        var request = new Calculator.SquareRootRequest { Number = number };

        try
        {
            var response = client.SquareRoot(request);
            Console.WriteLine($"Square root of {number} is: {response.Result}");
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
        {
            Console.WriteLine($"Error: {ex.Status.Detail}");
        }

        Console.WriteLine($"Response from server:{number}");
    }

    private async Task PrimeFactorizationClientRequestAsync(int number, GrpcChannel channel)
    {
        Console.WriteLine($"Client sending a prime factorization request:{number}");

        var client = new Calculator.CalculatorService.CalculatorServiceClient(channel);
        var request = new Calculator.PrimeFactorizationRequest { Number = number };

        var response = client.DecomposeIntoPrimes(request);

        await foreach (var prime in response.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine($"Prime factor: {prime}");
        }

        Console.WriteLine($"Response from Server:{number}");
    }

    private async Task AverageCalculationAsync(IEnumerable<double> averageStream, GrpcChannel channel)
    {
        Console.WriteLine($"Client sending a average calculation request stream:{averageStream}");

        var client = new Calculator.CalculatorService.CalculatorServiceClient(channel);
        var stream = client.AverageCalculation();

        foreach (var number in averageStream)

        {
            var request = new Calculator.AverageCalculationRequest { Number = number };
            await stream.RequestStream.WriteAsync(request);
        }

        await stream.RequestStream.CompleteAsync();

        var response = await stream.ResponseAsync;

        Console.WriteLine($"Response from server: {response.AverageResult}");
    }

    private async Task FindCurrentMaxAsync(IEnumerable<double> numbers, GrpcChannel channel)
    {
        Console.WriteLine($"Client sending a find max value request stream:{numbers}");

        var client = new Calculator.CalculatorService.CalculatorServiceClient(channel);

        using var stream = client.FindCurrentMax();

        // Task to read responses from server
        var readTask = Task.Run(async () =>
        {
            await foreach (var response in stream.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"Current max from server: {response.MaxValue}");
            }
        });

        // Write each number to the server's request stream
        foreach (var number in numbers)
        {
            await stream.RequestStream.WriteAsync(new Calculator.FindCurrentMaxRequest() { Number = number });
        }

        // Tell the server we've finished sending requests
        await stream.RequestStream.CompleteAsync();

        // Wait until the server completes sending responses
        await readTask;
    }

    private async Task ServerCountDownRequestAsync(int secondsAmount, GrpcChannel channel)
    {
        Console.WriteLine($"Client sending a countdown request of:{secondsAmount} seconds");

        var countdownClient = new Countdown.CountDownService.CountDownServiceClient(channel);
        var countdownRequest = new Countdown.ServerCountDownRequest { Seconds = secondsAmount };

        var response = countdownClient.StartServerCountDown(countdownRequest);

        await foreach (var message in response.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine($"{message.Message} seconds remaining...");
        }
    }

    private async Task ClientCountDownRequestAsync(int secondsAmount, GrpcChannel channel)
    {
        Console.WriteLine($"Server sending a countdown request of:{secondsAmount} seconds");

        var client = new Countdown.CountDownService.CountDownServiceClient(channel);
        var stream = client.StartClientCountDown();

        var remainingSeconds = secondsAmount;

        while (remainingSeconds >= 0)
        {
            await Task.Delay(1000);
            await stream.RequestStream.WriteAsync(new Countdown.ClientCountDownRequest { Seconds = remainingSeconds });

            remainingSeconds--;
        }

        await stream.RequestStream.CompleteAsync();

        var response = await stream.ResponseAsync;

        Console.WriteLine($"Server response: {response.Message}");
    }
}

