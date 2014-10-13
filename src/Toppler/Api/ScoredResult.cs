
namespace Toppler.Api
{
    public class ScoredResult
    {
        public string EventSource { get; private set; }
        public double Score { get; private set; }
        public int Rank { get; private set; }


        public ScoredResult(string eventSource, double score, int rank)
        {
            this.EventSource = eventSource;
            this.Score = score;
            this.Rank = rank;
        }
    }
}
