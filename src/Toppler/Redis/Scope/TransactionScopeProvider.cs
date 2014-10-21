using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Toppler.Helpers;

namespace Toppler.Redis.Scope
{
    internal class TransactionScopeProvider : IScopeProvider
    {
        private readonly IRedisConnection redisConnection;
        public TransactionScopeProvider(IRedisConnection redisConnection)
        {
            if (redisConnection == null)
                throw new ArgumentNullException("connectionProvider");

            this.redisConnection = redisConnection;
        }

        public Task<bool> Invoke(Action<IDatabaseAsync> commands, int db = 0)
        {
            try
            {
                var redisDb = this.redisConnection.GetDatabase(db);

                var tran = redisDb.CreateTransaction();
                //invoke
                commands(tran);
                return tran.ExecuteAsync();
            }
            catch (Exception ex)
            {
                Trace.TraceError("failed to invoke batch. Reason :" + ex.Message);
                return TaskHelper.AlwaysFalse;
            }
        }
    }
}
