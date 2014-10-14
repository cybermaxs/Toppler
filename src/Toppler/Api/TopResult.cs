
namespace Toppler.Api
{
    public class TopResult
    {
        public string EventSource { get; private set; }
        public double? Score { get; private set; }
        public long? Rank { get; private set; }

        public TopResult(string eventSource, double? score, long? rank)
        {
            this.EventSource = eventSource;
            this.Score = score;
            this.Rank = rank;
        }
    }
}
