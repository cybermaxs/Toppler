
namespace Toppler.Api
{
    public class ScoredResult
    {
        public string EventSource { get; set; }
        public double Score { get; set; }

        public ScoredResult(string eventSource, double score)
        {
            this.EventSource = eventSource;
            this.Score = score;
        }
    }
}
