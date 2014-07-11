using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Toppler.Redis.Scope
{
    internal class BatchScopeProvider : IScopeProvider
    {
        private readonly IConnectionProvider connectionProvider;
        public BatchScopeProvider(IConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
        }

        public Task<bool> Invoke(Action<IDatabaseAsync> commands, int db = 0)
        {
            try
            {
                var database = this.connectionProvider.GetDatabase(db);

                var batch = database.CreateBatch();
                //invoke
                commands(batch);
                batch.Execute();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Trace.TraceError("failed to invoke batch. Reason :" + ex.Message);
                return Task.FromResult(false);
            }
        }
    }
}
