using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Toppler.Core;

namespace Toppler.Sample.RateLimiter.App_Start
{
    public class RateLimiterMessageHandler : DelegatingHandler
    {
        private Granularity Granularity { get; set; }
        private int Range { get; set; }
        private long Limit { get; set; }

        public RateLimiterMessageHandler(Granularity granularity, int range, long limit)
        {
            this.Granularity = granularity;
            this.Range = range;
            this.Limit = limit;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var path = request.RequestUri.ToString();
            var IP = ((HttpContextBase)request.Properties["MS_HttpContext"]).Request.UserHostAddress;

            var rate = await Top.Ranking.DetailsAsync(IP, this.Granularity, this.Range, DateTime.UtcNow, new string[] { path });

            if (rate != null && rate.Score > this.Limit)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Rate Limit exceeded  !" + IP + "=>" + rate.Score) };
            }
            else
            {
                await Top.Counter.HitAsync(new string[] { IP }, 1L, new string[] { path });
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Current Rate Limit : " + IP + "=>" + rate.Score) };
            }

            
            return await base.SendAsync(request, cancellationToken);
        }
    }
}