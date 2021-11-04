using System;
using System.Collections.Generic;
using System.Text;
using Genesis.Plugin;
using Microsoft.CodeAnalysis;


namespace Examples.CodeGen.Genesis.Common
{
    public abstract class TypeGeneratorData : CodeGeneratorData
    {
        public readonly string Namespace;
        public readonly string Name;
        public readonly bool IsReadOnly;

        protected abstract string FilePathFormat { get; }

        public INamedTypeSymbol TypeSymbol { get; }

        public IMemoryCache MemoryCache { get; }

        protected readonly List<TypeDefinition> ContainingTypes = new List<TypeDefinition>();
        protected readonly List<FieldDefinition> Fields = new List<FieldDefinition>(0);
        protected readonly List<MethodDefinition> Methods = new List<MethodDefinition>(0);
        protected readonly List<PropertyDefinition> Properties = new List<PropertyDefinition>(0);
        protected readonly List<EventDefinition> Events = new List<EventDefinition>(0);

        public TypeGeneratorData(
              IMemoryCache memoryCache
            , INamedTypeSymbol typeSymbol
        )
        {
            MemoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            TypeSymbol = typeSymbol ?? throw new ArgumentNullException(nameof(typeSymbol));

            Namespace = typeSymbol.ContainingNamespace.GetFullName();
            Name = typeSymbol.Name;
            IsReadOnly = typeSymbol.IsReadOnly;

            GetContainingTypes(typeSymbol);
        }

        public virtual string GetFieldPrefix()
            => IsReadOnly ? "" : Tokens.NAMING_PREFIX;

        public IReadOnlyList<TypeDefinition> GetContainingTypes()
            => ContainingTypes;

        public IReadOnlyList<FieldDefinition> GetFields()
            => Fields;

        public IReadOnlyList<MethodDefinition> GetMethods()
            => Methods;

        public IReadOnlyList<PropertyDefinition> GetProperties()
            => Properties;

        public IReadOnlyList<EventDefinition> GetEvents()
            => Events;

        protected virtual void GetContainingTypes(INamedTypeSymbol namedTypeSymbol)
        {
            foreach (INamedTypeSymbol typeSymbol in namedTypeSymbol.GetContainingTypes())
            {
                var definition = MemoryCache.GetTypeDefinition(typeSymbol);
                ContainingTypes.Add(definition);
            }
        }

        public virtual string ReplaceTemplateTokens(string template)
        {
            return template
                .Replace(Tokens.NAMESPACE_TOKEN, Namespace)
                .Replace(Tokens.TYPE_NAME_TOKEN, Name)
                ;
        }

        public virtual string GetFileName(StringBuilder builder)
            => FileNameHelper.Combine(
                builder
                , FilePathFormat
                , Namespace
                , Name
                , containingTypes: ContainingTypes
            );
    }
}
