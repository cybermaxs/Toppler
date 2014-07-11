using System;
using System.Collections.Generic;

namespace Toppler.Extensions
{
    public static class ArgumentCheckExtensions
    {
        public static void ThrowIfNull<T>(this T obj, string name = "") where T : class
        {
            if (obj == null)
                throw new ArgumentNullException(name);
        }
        public static void ThrowIfDefault<T>(this T val, string name = "") where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(val, default(T)))
                throw new ArgumentException(name);
        }
    }
}
