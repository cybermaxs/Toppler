using StackExchange.Redis;
using System;
using System.Linq;
using System.Reflection;

namespace Toppler.Redis
{
    internal class StackExchangeRedisConnection : IRedisConnection, IDisposable
    {
        private ConnectionMultiplexer multiplexer;
        private ConfigurationOptions configurationOptions;

        public StackExchangeRedisConnection(ConfigurationOptions configurationOptions)
        {
            this.configurationOptions = configurationOptions;

            //myope overrides here
            this.configurationOptions.ConnectTimeout = 5000;
            this.configurationOptions.ConnectRetry = 3;
            this.configurationOptions.DefaultVersion = new Version("2.8.0");
            this.configurationOptions.KeepAlive = 90;
            this.configurationOptions.AbortOnConnectFail = false;
            this.configurationOptions.ClientName = "Toppler_" + System.Environment.MachineName + "_" + Assembly.GetCallingAssembly().GetName().Version;

            this.multiplexer = ConnectionMultiplexer.Connect(configurationOptions);
        }

        #region IRedisConnection
        public bool IsConnected
        {
            get { return this.multiplexer != null && this.multiplexer.IsConnected; }
        }

        public IDatabase GetDatabase(int db = 0)
        {
            return this.multiplexer.GetDatabase(db);
        }

        public IServer GetServer(string hostAndPort = "")
        {
            if (string.IsNullOrEmpty(hostAndPort))
                return this.multiplexer.GetServer(this.configurationOptions.EndPoints.First());
            else
                return this.multiplexer.GetServer(hostAndPort);
        }
        #endregion

        public void Dispose()
        {
            if (this.multiplexer != null)
                multiplexer.Close(false);
        }
    }
}
