using System;
using System.Linq;
using System.Diagnostics;
using StackExchange.Redis;
using System.Threading;
using Toppler.Core;

namespace Toppler.Tests.Integration.Fixtures
{
    public class RedisServerFixture : IDisposable
    {
        private Process server;
        private ConnectionMultiplexer mux;

        private bool wasStarted = false;

        public RedisServerFixture()
        {
            //todo : elsewahre
            Top.Setup(redisConfiguration: "localhost:6379",
    //  @namespace: "ns",
    granularities: new Granularity[] { Granularity.Second, Granularity.Minute, Granularity.Hour, Granularity.Day, Granularity.AllTime });

            if (!IsRunning)
            {
                this.server = Process.Start(@"..\..\..\..\tools\Redis-64.2.8.2101\redis-server.exe");
                wasStarted = true;
            }
            Thread.Sleep(1000);
            this.mux = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");
        }

        public void Dispose()
        {
            if (this.mux != null && this.mux.IsConnected)
                this.mux.Close(false);

            if (server != null && !server.HasExited && wasStarted)
                server.Kill();
        }

        public IDatabase GetDatabase(int db)
        {
            return this.mux.GetDatabase(db);
        }

        public static bool IsRunning
        {
            get
            {
                return Process.GetProcessesByName("redis-server").Count() > 0;
            }
        }

        public void Reset()
        {
            this.mux.GetServer("localhost:6379").FlushAllDatabases();
        }

        public static void Kill()
        {
            foreach (var p in Process.GetProcessesByName(@"redis-server"))
            {
                p.Kill();
            }
        }

        //public static bool Watch()
        //{
        //    if (IsRunning)
        //    {
        //        ProcessStartInfo info = new ProcessStartInfo();
        //        info.FileName = @"..\..\..\..\packages\Redis-64.2.8.19\redis-cli.exe";
        //        info.Arguments = "MONITOR";
        //        info.CreateNoWindow = true;
        //        info.UseShellExecute = false;
        //        info.RedirectStandardOutput = true;

        //        var cli = new Process();
        //        cli.StartInfo = info;
        //        cli.EnableRaisingEvents = true;
        //        cli.Start();
        //    }
        //    return false;
        //}

        //public static string GetInfo(string key=null)
        //{
        //    if (IsRunning)
        //    {
        //        using (var cnx = new RedisConnection("localhost", allowAdmin: true))
        //        {
        //            cnx.Wait(cnx.Open());
        //            var infos = cnx.Server.GetInfo().Result;
        //            if (infos.ContainsKey(key))
        //                return key+":"+infos[key];
        //            else
        //                return string.Join(System.Environment.NewLine, infos.Select((k, v) => { return k.Key + ":" + k.Value; }));
        //        }
        //    }
        //    return string.Empty;
        //}

    }
}
