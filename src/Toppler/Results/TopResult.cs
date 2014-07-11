
namespace Toppler.Results
{
    public class TopResult
    {
        public string EventSource { get; set; }
        public double Hits { get; set; }

        public TopResult(string eventSource, double hits)
        {
            this.EventSource = eventSource;
            this.Hits = hits;
        }
    }
}
