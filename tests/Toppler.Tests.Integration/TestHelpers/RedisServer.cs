using StackExchange.Redis;
using System.Diagnostics;
using System.Linq;

namespace Toppler.Tests.Integration.TestHelpers
{
    public class RedisServer
    {
        #region Server Management
        public static bool IsRunning
        {
            get
            {
                return Process.GetProcessesByName("redis-server").Count() > 0;
            }
        }

        public static bool Start()
        {
            if (!IsRunning)
                return Process.Start(@"..\..\..\..\packages\Redis-64.2.8.17\redis-server.exe") != null;
            return false;
        }

        public static bool Reset()
        {
            if (IsRunning)
            {
                using (var mpx = ConnectionMultiplexer.Connect(new ConfigurationOptions() { AllowAdmin = true, EndPoints = { { "localhost", 6379 } } }))
                {
                    mpx.GetServer("localhost:6379").FlushAllDatabases();
                }
            }
            return false;
        }

        public static void Kill()
        {
            foreach (var p in Process.GetProcessesByName(@"redis-server"))
            {
                p.Kill();
            }
        }
        #endregion

        #region DB management
        public static Process StartNewMonitor()
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = @"..\..\..\..\packages\Redis-64.2.8.17\redis-cli.exe";
            info.Arguments = "MONITOR";
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;

            var cli = new Process();
            cli.StartInfo = info;
            cli.EnableRaisingEvents = true;
            cli.Start();

            return cli;
        }

        #endregion
    }
}
