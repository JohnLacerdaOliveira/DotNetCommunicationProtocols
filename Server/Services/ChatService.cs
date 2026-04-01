using Contracts.Messages;
using Contracts.Requests;
using Contracts.Responses;
using Contracts.Services;
using ProtoBuf.Grpc;

namespace Server.Services;

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
