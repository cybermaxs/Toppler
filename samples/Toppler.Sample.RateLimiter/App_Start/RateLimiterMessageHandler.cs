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
            //var dimension = request.GetActionDescriptor().ControllerDescriptor.ControllerName;
            //var action = request.GetActionDescriptor().ActionName;
            var dimension = "API";
            var action = request.RequestUri.ToString();
            var IP = ((HttpContextBase)request.Properties["MS_HttpContext"]).Request.UserHostAddress;

            var tops = await Topp.Ranking.GetTops(this.Granularity, this.Range, DateTime.UtcNow, new string[] { dimension });
            var v = tops.FirstOrDefault(t => t.EventSource == action);

            if (v != null && v.Hits > this.Limit)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Rate Limit exceeded  !" + IP + "=>" + v.Hits) };
            }

            await Topp.Counter.HitAsync(new string[] { IP }, 1L, new string[] { dimension });
            return await base.SendAsync(request, cancellationToken);
        }
    }
}