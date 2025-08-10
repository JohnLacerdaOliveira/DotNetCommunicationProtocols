using Grpc.Core;
using Grpc.Net.Client;

namespace gRPC.Client
{
    internal class Program
    {
        const string SERVER_ADDRESS = "https://localhost:50051";
        const int DEADLINE_FOR_CONNECTION = 5;

        static async Task Main(string[] args)
        {
            try
            {
                // Using Core approach for now, as it is simpler for basic gRPC client functionality.

                var caCert = File.ReadAllText("ssl/ca.crt");

                var channelCredentials = new SslCredentials(caCert);

                var channelOptions = new GrpcChannelOptions
                {
                    Credentials = channelCredentials
                };

            
                await new GrpcClient().RunClient(SERVER_ADDRESS, new GrpcChannelOptions(), DEADLINE_FOR_CONNECTION);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Certificate file not found: {ex.FileName}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied reading certificate file: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO error reading certificate file: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }

            Console.ReadLine();
        }
    }
}