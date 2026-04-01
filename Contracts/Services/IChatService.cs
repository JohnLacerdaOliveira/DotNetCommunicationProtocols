using Contracts.Messages;
using Contracts.Requests;
using Contracts.Responses;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

namespace Contracts.Services;

[Service]
public interface IChatService
{
    public Task<JoinResponse> JoinAsync(
        JoinRequest request,
        CallContext context = default);

    public Task<Ack> SendMessagesAsync(
        IAsyncEnumerable<ChatMessage> messages,
        CallContext context = default);

    public IAsyncEnumerable<ChatMessage> SubscribeAsync(
        SubscribeRequest message,
        CallContext context = default);

    public IAsyncEnumerable<ChatMessage> ChatStream(
        IAsyncEnumerable<ChatMessage> messages,
        CallContext context = default);
}
