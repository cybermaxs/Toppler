using System;
using System.Linq;
using Toppler.Extensions;

namespace Toppler.Core
{
    internal class DefaultKeyFactory : IKeyFactory
    {
        public const string separator = ":";
        private string @namespace = string.Empty;

        public string Namespace
        {
            get { return this.@namespace; }
        }

        public DefaultKeyFactory(string @namespace = "")
        {
            if (!@namespace.AlphaNumericString())
                throw new ArgumentException("Only alphanumeric characters may be used. Invalid namespace :" + @namespace);

            this.@namespace = @namespace;
        }

        /// <summary>
        /// Generate a key by prefixing namespace.
        /// </summary>
        /// <param name="parts"></param>
        /// <returns></returns>
        public string NsKey(params string[] parts)
        {
            if (!string.IsNullOrWhiteSpace(@namespace))
                return @namespace + separator + string.Join(separator, parts);
            else
                return string.Join(separator, parts);
        }

        /// <summary>
        /// Generate a raw key without namespace.
        /// </summary>
        /// <param name="parts"></param>
        /// <returns></returns>
        public string RawKey(params string[] parts)
        {
            return string.Join(separator, parts);
        }
    }
}
