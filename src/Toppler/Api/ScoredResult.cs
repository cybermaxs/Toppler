
namespace Toppler.Api
{
    public class ScoredResult
    {
        public string EventSource { get; private set; }
        public double? Score { get; private set; }
        public long? Rank { get; private set; }


        public ScoredResult(string eventSource, double? score, long? rank)
        {
            this.EventSource = eventSource;
            this.Score = score;
            this.Rank = rank;
        }
    }
}
