
namespace Toppler.Core
{
    public interface IGranularity
    {
        string Name { get; set; }
        int Size { get; set; }
        int Ttl { get; set; }
        int Factor { get; set; }
    }
}
