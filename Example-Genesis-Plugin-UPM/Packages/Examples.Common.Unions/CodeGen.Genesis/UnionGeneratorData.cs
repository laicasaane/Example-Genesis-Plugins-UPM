using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Examples.Common;
using Examples.CodeGen.Genesis.Common;
using Genesis.Plugin;

namespace Examples.CodeGen.Genesis.Plugins.Unions
{
    internal sealed class UnionGeneratorData : TypeGeneratorData
    {
        private const string INVALID_TOKEN = "${INVALID_VALUE_ACCESS}";

        public readonly InvalidAccessKind InvalidAccessKind;
        public readonly string FieldEnumUnderlyingType;

        protected override string FilePathFormat => @"Unions\{0}.cs";

        public UnionGeneratorData(
              IMemoryCache memoryCache
            , INamedTypeSymbol unionType
            , INamedTypeSymbol tupleType
            , InvalidAccessKind invalidAccessKind
        )
            : base(memoryCache, unionType)
        {
            InvalidAccessKind = invalidAccessKind;

            MakeMembers(tupleType);
            GetFieldEnumUnderlyingType(out FieldEnumUnderlyingType);
        }

        private void MakeMembers(INamedTypeSymbol tupleType)
        {
            ImmutableArray<IFieldSymbol> fields = tupleType.TupleElements;
            Fields.Capacity = fields.Length;

            foreach (IFieldSymbol field in fields)
            {
                Fields.Add(new FieldDefinition(MemoryCache, field));
            }
        }

        private void GetFieldEnumUnderlyingType(out string typeName)
        {
            var memberCount = (ulong)Fields.Count;

            if (memberCount <= byte.MaxValue)
            {
                typeName = "byte";
            }
            else if (memberCount <= ushort.MaxValue)
            {
                typeName = "ushort";
            }
            else if (memberCount <= uint.MaxValue)
            {
                typeName = "uint";
            }
            else
            {
                typeName = "ulong";
            }
        }

        public override string GetFieldPrefix()
            => (IsReadOnly && InvalidAccessKind == InvalidAccessKind.Allow) ? "" : Tokens.NAMING_PREFIX;

        public override string ReplaceTemplateTokens(string template)
        {
            return template
                .Replace(Tokens.NAMESPACE_TOKEN, Namespace)
                .Replace(Tokens.TYPE_NAME_TOKEN, Name)
                .Replace(INVALID_TOKEN, InvalidAccessKind.ToString())
                ;
        }
    }
}
