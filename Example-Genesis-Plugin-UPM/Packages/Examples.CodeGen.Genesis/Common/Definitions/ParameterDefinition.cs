using System;
using Genesis.Plugin;
using Microsoft.CodeAnalysis;

namespace Examples.CodeGen.Genesis.Common
{
    public class ParameterDefinition : MemberDefinition
    {
        public IParameterSymbol ParameterSymbol { get; }

        public ParameterDefinition(
              IMemoryCache memoryCache
            , IParameterSymbol symbol
        )
            : base(memoryCache, symbol)
        {
            ParameterSymbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        }
    }
}
