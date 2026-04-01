using GrpcAspNet.Contracts.Messages;
using GrpcAspNet.Contracts.Requests;
using GrpcAspNet.Contracts.Responses;
using GrpcAspNet.Contracts.Services;
using ProtoBuf.Grpc;

namespace GrpcAspNet.Server.Services;

public class ChatService : IChatService
{
    public IAsyncEnumerable<ChatMessage> ChatStream(IAsyncEnumerable<ChatMessage> messages, CallContext context = default)
    {
        throw new NotImplementedException();
    }

    public Task<JoinResponse> JoinAsync(JoinRequest request, CallContext context = default)
    {
        throw new NotImplementedException();
    }

    public Task<Ack> SendMessagesAsync(IAsyncEnumerable<ChatMessage> messages, CallContext context = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<ChatMessage> SubscribeAsync(SubscribeRequest message, CallContext context = default)
    {
        throw new NotImplementedException();
    }
}
