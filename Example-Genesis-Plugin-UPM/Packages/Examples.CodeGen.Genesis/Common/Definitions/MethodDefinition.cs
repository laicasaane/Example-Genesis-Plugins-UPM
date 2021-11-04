using System;
using System.Collections.Generic;
using Genesis.Plugin;
using Microsoft.CodeAnalysis;

namespace Examples.CodeGen.Genesis.Common
{
    public class MethodDefinition : MemberDefinition
    {
        public IMethodSymbol MethodSymbol { get; }

        public IReadOnlyList<ParameterDefinition> Parameters => _parameters;

        private readonly List<ParameterDefinition> _parameters;

        public MethodDefinition(
              IMemoryCache memoryCache
            , IMethodSymbol symbol
        )
            : base(memoryCache, symbol)
        {
            MethodSymbol = symbol ?? throw new ArgumentNullException(nameof(symbol));

            var parameters = symbol.Parameters;
            _parameters = new List<ParameterDefinition>(parameters.Length);

            foreach (IParameterSymbol parameter in parameters)
            {
                _parameters.Add(new ParameterDefinition(MemoryCache, parameter));
            }
        }
    }
}
