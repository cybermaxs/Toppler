using System;
using System.Linq;
using System.Collections.Generic;

namespace Toppler.Core
{
    public class DefaultGranularityProvider : IGranularityProvider
    {
        private List<Granularity> Granularities { get; set; }

        public DefaultGranularityProvider()
        {
            this.Granularities = new List<Granularity>();
            this.Granularities.Add(Granularity.Second);
            this.Granularities.Add(Granularity.Minute);
            this.Granularities.Add(Granularity.Hour);
            this.Granularities.Add(Granularity.Day);
        }

        public Granularity[] GetGranularities()
        {
            return Granularities.ToArray();
        }


        public bool RegisterGranularity(Granularity granularity)
        {
            if (granularity.Size <= 0 || granularity.Ttl <= 0 || granularity.Factor <= 0 || string.IsNullOrEmpty(granularity.Name))
            {
                throw new ArgumentException("granularity is not valid");
            }

            if (this.Granularities.Exists(g => g.Factor == granularity.Factor && g.Size == granularity.Size))
            {
                //throw new InvalidOperationException("a granularity with same factor & size is laready registered.");
                return false;
            }

            this.Granularities.Add(granularity);
            return true;
        }
    }
}
