using System;
using Genesis.Plugin;
using Microsoft.CodeAnalysis;

namespace Examples.CodeGen.Genesis.Common
{
    public class PropertyDefinition : MemberDefinition
    {
        public IPropertySymbol PropertySymbol { get; }

        public MethodDefinition GetMethod { get; }

        public MethodDefinition SetMethod { get; }

        public PropertyDefinition(
              IMemoryCache memoryCache
            , IPropertySymbol symbol
        )
            : base(memoryCache, symbol)
        {
            PropertySymbol = symbol ?? throw new ArgumentNullException(nameof(symbol));

            if (PropertySymbol.GetMethod != null)
                GetMethod = new MethodDefinition(MemoryCache, PropertySymbol.GetMethod);

            if (PropertySymbol.SetMethod != null)
                SetMethod = new MethodDefinition(MemoryCache, PropertySymbol.SetMethod);
        }
    }
}
