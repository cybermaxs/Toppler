using System;
using System.Diagnostics;

namespace Toppler.Tests.Integration.TestHelpers
{
    public class TestBase
    {
        private static Random Generator = new Random();


        protected string TestEventSource { get; private set; }
        protected string TestDimension { get; private set; }
        public TestBase()
        {
            this.TestEventSource = RandomEventSource();      
            this.TestDimension = RandomDimension();
        }

        protected bool Reset()
        {
            return RedisServer.Reset();
        }

        #region Monitor
        private Process RedisCli;
        protected void StartMonitor()
        {
            if (RedisCli == null)
            {
                RedisCli = RedisServer.StartNewMonitor();
                RedisCli.BeginOutputReadLine();
                RedisCli.OutputDataReceived += (e, s) =>
                {
                    Trace.WriteLine(s.Data);
                };
            }
        }
        protected void StopMonitor()
        {
            if (RedisCli != null)
                RedisCli.Kill();
        }
        #endregion"

        public static int RandomHits()
        {
            return Generator.Next(1000);
        }

        public static string RandomEventSource()
        {
            return "es-" + Generator.Next(1000).ToString();
        }


        public static string RandomDimension()
        {
            return "dim-" + Generator.Next(1000).ToString();
        }
    }
}
