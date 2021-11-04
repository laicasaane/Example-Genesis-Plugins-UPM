using System;
using System.Collections.Generic;
using Genesis.Plugin;

namespace Examples.CodeGen.Genesis.Common
{
    public static partial class MemoryCacheExtensions
    {
        public static Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(
            this IMemoryCache memoryCache
        )
        {
            if (memoryCache == null)
                throw new ArgumentNullException(nameof(memoryCache));

            var tkey = typeof(TKey).FullName;
            var tvalue = typeof(TValue).FullName;
            var key = $"{typeof(MemoryCacheExtensions).FullName}.{nameof(GetDictionary)}<{tkey},{tvalue}>";

            if (memoryCache.TryGet(key, out Dictionary<TKey, TValue> dictionary) == false)
            {
                dictionary = new Dictionary<TKey, TValue>();
                memoryCache.Add(key, dictionary);
            }

            return dictionary;
        }
    }
}
