using System;
using Microsoft.CodeAnalysis;
using Genesis.Plugin;

namespace Examples.CodeGen.Genesis.Common
{
    public static partial class MemoryCacheExtensions
    {
        public static TypeDefinition GetTypeDefinition(
              this IMemoryCache memoryCache
            , ITypeSymbol typeSymbol
        )
        {
            if (memoryCache == null)
                throw new ArgumentNullException(nameof(memoryCache));

            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            var name = typeSymbol.ToString();
            var dictionary = memoryCache.GetDictionary<string, TypeDefinition>();

            if (dictionary.TryGetValue(name, out TypeDefinition definition) == false)
            {
                definition = new TypeDefinition(memoryCache, typeSymbol);
                dictionary.Add(name, definition);
            }

            return definition;
        }
    }
}
