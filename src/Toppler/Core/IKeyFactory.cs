
namespace Toppler.Core
{
    internal interface IKeyFactory
    {
        string Namespace { get; }
        string NsKey(params string[] parts);
        string RawKey(params string[] parts);
    }
}
