using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Toppler.Redis.Scope
{
    internal class TransactionScopeProvider : IScopeProvider
    {
        private readonly IConnectionProvider connectionProvider;
        public TransactionScopeProvider(IConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
        }

        public Task<bool> Invoke(Action<IDatabaseAsync> commands, int db = 0)
        {
            try
            {
                var database = this.connectionProvider.GetDatabase(db);

                var tran = database.CreateTransaction();
                //invoke
                commands(tran);
                return tran.ExecuteAsync();
            }
            catch (Exception ex)
            {
                Trace.TraceError("failed to invoke batch. Reason :" + ex.Message);
                return Task.FromResult(false);
            }
        }
    }
}
