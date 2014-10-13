
namespace Toppler.Api
{
    public class TopResult
    {
        public string EventSource { get; private set; }
        public double? Hits { get; private set; }
        public long? Rank { get; private set; }

        public TopResult(string eventSource, double? hits, long? rank)
        {
            this.EventSource = eventSource;
            this.Hits = hits;
            this.Rank = rank;
        }
    }
}
