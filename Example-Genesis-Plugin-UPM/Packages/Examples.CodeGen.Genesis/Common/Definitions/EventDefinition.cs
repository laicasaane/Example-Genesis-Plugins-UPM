using System;
using Genesis.Plugin;
using Microsoft.CodeAnalysis;

namespace Examples.CodeGen.Genesis.Common
{
    public class EventDefinition : MemberDefinition
    {
        public IEventSymbol EventSymbol { get; }

        public EventDefinition(
              IMemoryCache memoryCache
            , IEventSymbol symbol
        )
            : base(memoryCache, symbol)
        {
            EventSymbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        }
    }
}
