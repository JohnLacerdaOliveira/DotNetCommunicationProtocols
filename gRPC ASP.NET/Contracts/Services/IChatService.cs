using GrpcAspNet.Contracts.Messages;
using GrpcAspNet.Contracts.Requests;
using GrpcAspNet.Contracts.Responses;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

namespace GrpcAspNet.Contracts.Services;

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
