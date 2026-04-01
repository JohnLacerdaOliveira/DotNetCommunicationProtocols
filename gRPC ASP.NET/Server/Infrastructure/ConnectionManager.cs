using GrpcAspNet.Contracts.Messages;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace GrpcAspNet.Server.Infrastructure;

public class ConnectionManager
{
    private readonly ConcurrentDictionary<Guid, Channel<ChatMessage>> _connections = new();

    public Guid AddClient(Channel<ChatMessage> channel)
    {
        var id = Guid.NewGuid();
        _connections[id] = channel;
        return id;
    }

    public void RemoveClient(Guid id)
    {
        _connections.TryRemove(id, out _);
    }

    public async Task BroadcastAsync(ChatMessage message)
    {
        foreach (var connection in _connections.Values)
        {
            await connection.Writer.WriteAsync(message);
        }
    }
}