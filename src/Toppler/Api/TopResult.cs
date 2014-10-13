
namespace Toppler.Api
{
    public class TopResult
    {
        public string EventSource { get; private set; }
        public double Hits { get; private set; }
        public int Rank { get; private set; }

        public TopResult(string eventSource, double hits, int rank)
        {
            this.EventSource = eventSource;
            this.Hits = hits;
            this.Rank = rank;
        }
    }
}
