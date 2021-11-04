using System;
using Genesis.Plugin;
using Microsoft.CodeAnalysis;

namespace Examples.CodeGen.Genesis.Common
{
    public class TypeDefinition : SymbolDefinition
    {
        public ITypeSymbol TypeSymbol { get; }

        public string FullName { get; }

        public bool IsNullable { get; }

        public bool IsGeneric { get; }

        public EqualityKind Equality { get; }

        public TypeDefinition(
              IMemoryCache memoryCache
            , ITypeSymbol symbol
        )
            : base(memoryCache, symbol)
        {
            TypeSymbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            FullName = symbol.GetBuiltinOrFullName();

            IsNullable = symbol.IsNullable();
            IsGeneric = symbol.IsGenericType();
            Equality = symbol.GetEqualityKind();
        }

        public override string ToString()
            => FullName;

        public bool FindNullableUnderlyingType(out TypeDefinition definition)
        {
            if (IsNullable == false)
            {
                definition = default;
                return false;
            }

            var typeSymbol = TypeSymbol.GetNullableUnderlyingType();
            definition = MemoryCache.GetTypeDefinition(typeSymbol);
            return true;
        }
    }
}
