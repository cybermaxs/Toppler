
namespace Toppler
{
   /// <summary>
   /// Various constants for the API.
   /// </summary>
   public class Constants
    {
       /// <summary>
       /// Default redis DB for storage.
       /// </summary>
       public const int DefaultRedisDb = 0;

       /// <summary>
       /// Default namespace (=prefix) for keys
       /// </summary>
       public const string DefaultNamespace = "toppler";

       /// <summary>
       /// Default name for the global dimension.
       /// </summary>
       public const string DefaultDimension = "default";

       /// <summary>
       /// ?
       /// </summary>
       public const string SetAllDimensions = "dimensions";

       /// <summary>
       /// Default prefix fof the cache key.
       /// </summary>
       public const string CacheKeyPart = "cache";
    }
}
