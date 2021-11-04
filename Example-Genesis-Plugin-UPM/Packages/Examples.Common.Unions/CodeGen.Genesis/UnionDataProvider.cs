using System;
using System.Linq;
using System.Collections.Generic;
using Genesis.Plugin;
using Microsoft.CodeAnalysis;
using Examples.Common;
using Examples.Common.Unions;
using Examples.CodeGen.Genesis.Common;

namespace Examples.CodeGen.Genesis.Plugins.Unions
{
    internal sealed class UnionDataProvider : TypeDataProvider<UnionDataProvider>
    {
        protected override IEnumerable<CodeGeneratorData> GetCodeGeneratorData(
            IReadOnlyList<ICachedNamedTypeSymbol> typeSymbolInfo
        )
        {
            foreach (ICachedNamedTypeSymbol type in typeSymbolInfo)
            {
                INamedTypeSymbol unionType = type?.NamedTypeSymbol;

                if (unionType == null
                    || unionType.IsValueType == false
                    || unionType.IsGenericType
                )
                {
                    continue;
                }

                var gotData = TryGetDataFromAttribute(
                      unionType
                    , out INamedTypeSymbol tupleType
                    , out TypedConstant? invalidAccessKind
                );

                if (gotData == false)
                    continue;

                yield return new UnionGeneratorData(
                        MemoryCache
                    , unionType
                    , tupleType
                    , GetInvalidAccessKind(invalidAccessKind)
                );
            }
        }

        private static bool TryGetDataFromAttribute(
              INamedTypeSymbol unionType
            , out INamedTypeSymbol tupleType
            , out TypedConstant? invalidAccessKind
        )
        {
            tupleType = null;
            invalidAccessKind = null;

            var attributes = unionType.GetAttributes(nameof(UnionAttribute));

            if (attributes.Count() <= 0)
                return false;

            AttributeData attribute = attributes.First();
            var arguments = attribute.ConstructorArguments;

            if (arguments.Length <= 0)
                return false;

            if (!(arguments[0].Value is INamedTypeSymbol namedTypeSymbol)
                || namedTypeSymbol.IsTupleType == false
                || namedTypeSymbol.TupleElements.Length <= 0
            )
            {
                return false;
            }

            if (arguments.Length > 1)
                invalidAccessKind = arguments[1];

            tupleType = namedTypeSymbol;
            return true;
        }

        private static InvalidAccessKind GetInvalidAccessKind(TypedConstant? typeCconstant)
        {
            if (typeCconstant.HasValue == false)
                return default;

            var typeName = typeCconstant.Value.Type.Name;
            var value = typeCconstant.Value.Value?.ToString();

            if (!string.Equals(typeName, nameof(InvalidAccessKind))
                || string.IsNullOrWhiteSpace(value) != false
            )
            {
                return default;
            }

            if (Enum.TryParse(value, true, out InvalidAccessKind result))
                return result;

            return default;
        }
    }
}
