using StackExchange.Redis;

namespace Toppler.Redis
{
    public interface IConnectionProvider
    {
        bool IsConnected { get; }
        IDatabase GetDatabase(int db = 0);
        IServer GetServer(string localhostAndPort="localhost:6379");
    }
}
