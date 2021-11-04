using System;
using System.Collections.Generic;
using System.Text;
using Genesis.Plugin;

namespace Examples.CodeGen.Genesis.Common
{
    public static partial class MemoryCacheExtensions
    {
        public static StringBuilder GetStringBuilder(this IMemoryCache memoryCache)
        {
            if (memoryCache == null)
                throw new ArgumentNullException(nameof(memoryCache));

            var key = $"{typeof(MemoryCacheExtensions).FullName}.{nameof(GetStringBuilder)}";

            if (memoryCache.TryGet(key, out Queue<StringBuilder> queue) == false)
            {
                queue = new Queue<StringBuilder>();
                memoryCache.Add(key, queue);
            }

            return null;
        }
    }
}
