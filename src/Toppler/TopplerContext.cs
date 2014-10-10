using System;
using Toppler.Core;

namespace Toppler
{
    /// <summary>
    /// An internal class to keep user-defined settings.
    /// </summary>
    internal class TopplerContext : ITopplerContext
    {
        public string Namespace { get; private set; }
        public int  DbIndex { get; private set; }
        public Granularity[] Granularities { get; private set; }
        public IKeyFactory KeyFactory { get; private set; }
        public TopplerContext(string @namespace,int dbIndex,  Granularity[] granularities)
        {
            this.Namespace = @namespace;
            this.DbIndex = dbIndex;
            this.KeyFactory = new DefaultKeyFactory(this.Namespace);
            this.Granularities = granularities!=null && granularities.Length == 0 ? new DefaultGranularityProvider().GetGranularities() : granularities;
        }
    }
}
