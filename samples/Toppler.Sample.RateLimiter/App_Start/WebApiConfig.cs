using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Toppler.Core;
using Toppler.Extensions;

namespace Toppler.Sample.RateLimiter
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.MessageHandlers.Add(new RateLimiterMessageHandler(Granularity.Second, 10));
        }

        public class RateLimiterMessageHandler : DelegatingHandler
        {
            private Granularity granularity;
            private int count;

            public RateLimiterMessageHandler(Granularity granularity, int count)
            {
                this.granularity = granularity;
                this.count = count;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                //var dimension = request.GetActionDescriptor().ControllerDescriptor.ControllerName;
                //var action = request.GetActionDescriptor().ActionName;
                var dimension = "API";
                var action = request.RequestUri.AbsolutePath;

                var tops = await Topp.Ranking.GetTops(this.granularity, this.count, DateTime.UtcNow, new string[] { dimension });
                var v = tops.FirstOrDefault(t => t.EventSource == action);

                if (v != null && v.Hits > 5)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent( "Rate Limit exceeded  !") };
                }
                else
                {
                    await Topp.Counter.HitAsync(eventSource: action, dimension: dimension);
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(v.Hits.ToString()) };
                }

                // await Topp.Counter.HitAsync(eventSource: action, dimension: dimension);
                //return await base.SendAsync(request, cancellationToken);
            }
        }
    }
}
