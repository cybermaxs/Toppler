using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Toppler.Redis.Scope
{
    interface IScopeProvider
    {
        Task<bool> Invoke(Action<IDatabaseAsync> commands,int db=0);
    }
}
