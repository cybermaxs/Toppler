using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Toppler.Redis
{
    internal class DefaultConnectionProvider : IConnectionProvider, IDisposable
    {
        /// <summary>
        /// Internal Connection State.
        /// </summary>
        private static class State
        {
            public const int Closed = 0;
            public const int Connected = 1;
            public const int Disposing = 2;
            public const int Disposed = 3;
        }

        private ConfigurationOptions redisConfiguration;
        private ConnectionMultiplexer multiplexer;

        private static readonly object connectionLock = new object();
        private int state;
        private long attemptsCount;

        public TimeSpan ReconnectDelay = TimeSpan.FromSeconds(2000);
        public TimeSpan GetConnectionTimeOut = TimeSpan.FromSeconds(10000);
        public int MaxConnectionAttempts = int.MaxValue;

        public DefaultConnectionProvider(string redisConfigurationString)
        {
            this.Configure(redisConfigurationString);

            if (this.redisConfiguration != null)
            {
                this.ConnectAsyncWithRetry();
            }
            else
                throw new InvalidOperationException("Redis connection info not set. Impossible to connect to a Redis DB.");
        }

        private void Configure(string redisConfigurationString)
        {
            ConfigurationOptions options = ConfigurationOptions.Parse(redisConfigurationString);
            options.ClientName = "Toppler.Client";
            this.redisConfiguration = options;
        }

        private void ConnectAsyncWithRetry()
        {
            Task connectTask = ConnectAsync();
            connectTask.ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Trace.TraceError("Error:" + task.Exception.GetBaseException(), "Error connecting to Redis");

                    if (this.state == State.Disposing)
                    {
                        Shutdown();
                        return;
                    }

                    if (this.attemptsCount >= this.MaxConnectionAttempts)
                    {
                        Trace.TraceWarning("Max Retries reached");
                        Shutdown();
                        return;
                    }

                    Task.Delay(ReconnectDelay).ContinueWith(t => { this.ConnectAsyncWithRetry(); });
                }
                else
                {
                    Trace.TraceInformation("Connected");

                    var oldState = Interlocked.CompareExchange(ref this.state,
                                                               State.Connected,
                                                               State.Closed);

                    Interlocked.Exchange(ref this.attemptsCount, 0);

                    if (oldState == State.Disposing)
                    {
                        Shutdown();
                    }
                }
            },
            TaskContinuationOptions.ExecuteSynchronously);
        }

        private void Shutdown()
        {
            Trace.TraceInformation("Shutdown()");

            if (this.multiplexer != null && this.multiplexer.IsConnected)
            {
                this.multiplexer.Close(true);
            }

            Interlocked.Exchange(ref this.state, State.Disposed);
        }

        private Task ConnectAsync()
        {
            //multiplexer already set
            if (this.multiplexer != null)
            {
                this.multiplexer.ConnectionFailed -= multiplexer_ConnectionFailed;
                this.multiplexer.Dispose();
                this.multiplexer = null;
            }

            try
            {

                Trace.TraceInformation("Connecting...");
                Interlocked.Increment(ref this.attemptsCount);

                var options = this.redisConfiguration;
                options.ClientName = "Toppler.Client";

                return ConnectionMultiplexer.ConnectAsync(options).ContinueWith(t =>
                {
                    if (!t.IsFaulted)
                    {
                        lock (connectionLock)
                        {
                            //re-check connection
                            if (this.multiplexer == null)
                            {
                                var mpx = t.Result;
                                this.multiplexer = mpx;
                                this.multiplexer.ConnectionFailed += multiplexer_ConnectionFailed;
                                Monitor.Pulse(connectionLock);
                            }
                            else
                            {
                                //dispose this temp connection if one is already set
                                multiplexer.Close();
                                multiplexer.Dispose();
                            }
                        }


                    }
                    return t;
                });
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.GetBaseException() + "Error connecting to Redis");
                return Task.FromResult(ex);
            }
        }

        void multiplexer_ConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Trace.TraceError("Connection Failed");

            var oldState = Interlocked.CompareExchange(ref this.state,
                                           State.Closed,
                                           State.Connected);
            if (oldState == State.Connected)
            {
                Trace.TraceInformation("Attempting reconnect...");

                // Retry until the connection reconnects
                ConnectAsyncWithRetry();
            }
        }

        private ConnectionMultiplexer GetMultiplexer()
        {
            lock (connectionLock)
            {
                while (this.multiplexer == null && this.attemptsCount <= MaxConnectionAttempts)
                    if (!Monitor.Wait(connectionLock, GetConnectionTimeOut))
                        throw new TimeoutException("Could not connect to Redis DB");

                return this.multiplexer;
            }
        }

        #region IConnectionProvider
        public IServer GetServer(string localhostAndPort)
        {
            var mpx = this.GetMultiplexer();
            return mpx.GetServer(localhostAndPort);
        }

        public IDatabase GetDatabase(int db = 0)
        {
            var mpx = this.GetMultiplexer();
            return mpx.GetDatabase(db);
        }

        public bool IsConnected
        {
            get { return this.state == State.Connected; }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            var oldState = Interlocked.Exchange(ref this.state, State.Disposing);

            switch (oldState)
            {
                case State.Connected:
                    Shutdown();
                    break;
                case State.Closed:
                case State.Disposing:
                    // No-op
                    break;
                case State.Disposed:
                    Interlocked.Exchange(ref this.state, State.Disposed);
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
