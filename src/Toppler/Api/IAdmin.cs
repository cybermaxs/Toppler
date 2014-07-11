using System.Threading.Tasks;

namespace Toppler.Api
{
    public interface IAdmin
    {
        Task FlushContexts(string[] dimensions = null);
    }
}
