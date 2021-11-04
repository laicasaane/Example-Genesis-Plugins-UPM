using System;
using Genesis.Plugin;
using Microsoft.CodeAnalysis;

namespace Examples.CodeGen.Genesis.Common
{
    public class SymbolDefinition
    {
        public IMemoryCache MemoryCache { get; }

        public ISymbol Symbol { get; }

        public SymbolDefinition(
              IMemoryCache memoryCache
            , ISymbol symbol
        )
        {
            MemoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        }

        public override string ToString()
            => Symbol.Name;
    }
}
