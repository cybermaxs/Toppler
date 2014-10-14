using System.Threading.Tasks;

namespace Toppler.Helpers
{
    internal class TaskHelper
    {
        public static readonly Task<bool> AlwaysTrue = Task.FromResult(true);
        public static readonly Task<bool> AlwaysFalse = Task.FromResult(false);
        public static readonly Task<object> Empty = Task.FromResult<object>(null);
    }
}
