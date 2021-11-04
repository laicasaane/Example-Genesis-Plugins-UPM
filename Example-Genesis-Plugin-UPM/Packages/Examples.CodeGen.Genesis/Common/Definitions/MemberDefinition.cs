using System;
using Genesis.Plugin;
using Microsoft.CodeAnalysis;

namespace Examples.CodeGen.Genesis.Common
{
    public class MemberDefinition : SymbolDefinition
    {
        public ISymbol MemberSymbol { get; }

        public TypeDefinition Type { get; }

        public MemberDefinition(
              IMemoryCache memoryCache
            , ISymbol symbol
        )
            : base(memoryCache, symbol)
        {
            MemberSymbol = symbol ?? throw new ArgumentNullException(nameof(symbol));

            Type = MemoryCache.GetTypeDefinition(symbol.GetMemberType());
        }
    }
}
