using StackExchange.Redis;
using System;
using Toppler.Api;
using Toppler.Core;
using Toppler.Redis;

namespace Toppler
{
    /// <summary>
    /// Toppler main entry point.
    /// </summary>
    public class Top
    {
        private static Lazy<IRedisConnection> lazyConnector;
        private static ITopplerContext options;

        /// <summary>
        /// Return the state of the Redis Connection.
        /// </summary>
        public static bool IsConnected
        {
            get { return lazyConnector != null && lazyConnector.IsValueCreated && lazyConnector.Value.IsConnected; }
        }

        private static IRedisConnection Connection
        {
            get { return lazyConnector.Value; }
        }

        /// <summary>
        /// Setup code. Required to setup redis persistent connection.
        /// </summary>
        /// <param name="redisConfiguration">StackExchange.Redis configuration settings(Default is localhost:6379)</param>
        /// <param name="dbIndex">Redis DB.</param>
        /// <param name="namespace">Prefix for all keys.</param>
        /// <param name="granularities">Used Granularities (Default is All : Second, Minute, Hour, Day)</param>
        public static void Setup(string redisConfiguration = "localhost:6379", int dbIndex = Constants.DefaultRedisDb, string @namespace = Constants.DefaultNamespace, Granularity[] granularities = null)
        {
            var settings = ConfigurationOptions.Parse(redisConfiguration);
            if (options == null && lazyConnector == null)
            {
                options = new TopplerContext(@namespace, dbIndex, granularities ?? new Granularity[] { Granularity.Second, Granularity.Minute, Granularity.Hour, Granularity.Day });
                lazyConnector = new Lazy<IRedisConnection>(() => { return new StackExchangeRedisConnection(settings); });
            }
        }

        #region Counter Api
        private static Lazy<ICounter> counterInstance = new Lazy<ICounter>(CreateCounter, true);

        /// <summary>
        /// Default Entry point for Clients.
        /// </summary>
        public static ICounter Counter
        {
            get
            {
                return counterInstance.Value;
            }
        }

        internal static ICounter CreateCounter()
        {
            if (options == null)
                throw new InvalidOperationException("Setup hasn't been called yet.");

            return new Counter(Connection, options);
        }
        #endregion

        #region Ranking Api
        private static Lazy<IRanking> rankingInstance = new Lazy<IRanking>(CreateRankingApi, true);

        /// <summary>
        /// Default Entry point for Clients.
        /// Use it to get tops & rankings.
        /// </summary>
        public static IRanking Ranking
        {
            get
            {
                return rankingInstance.Value;
            }
        }

        internal static IRanking CreateRankingApi()
        {
            if (options == null)
                throw new InvalidOperationException("Setup hasn't been called yet.");

            return new Ranking(Connection, options);
        }
        #endregion

        #region Admin Api
        private static Lazy<IAdmin> adminInstance = new Lazy<IAdmin>(CreateAdmin, true);

        /// <summary>
        /// Default Entry point for Clients.
        /// Use it to configure & do admin tasks.
        /// </summary>
        public static IAdmin Admin
        {
            get
            {
                return adminInstance.Value;
            }
        }

        internal static IAdmin CreateAdmin()
        {
            if (options == null)
                throw new InvalidOperationException("Setup hasn't been called yet.");

            return new Admin(Connection, options);
        }
        #endregion
    }
}
