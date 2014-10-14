using System.Threading.Tasks;

namespace Toppler.Api
{
    public interface IAdmin
    {
        /// <summary>
        /// Used to flush one or more dimensions (remove everyhting concerning a dimension)
        /// </summary>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        Task FlushDimensionsAsync(string[] dimensions = null);
    }
}
