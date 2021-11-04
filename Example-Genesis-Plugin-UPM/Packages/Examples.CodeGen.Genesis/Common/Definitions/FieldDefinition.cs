using System;
using Genesis.Plugin;
using Microsoft.CodeAnalysis;

namespace Examples.CodeGen.Genesis.Common
{
    public class FieldDefinition : MemberDefinition
    {
        public IFieldSymbol FieldSymbol { get; }

        public FieldDefinition(
              IMemoryCache memoryCache
            , IFieldSymbol symbol
        )
            : base(memoryCache, symbol)
        {
            FieldSymbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        }
    }
}
