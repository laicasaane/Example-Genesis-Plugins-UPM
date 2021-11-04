using System;
using System.Collections.Generic;
using Genesis.Plugin;
using Genesis.Shared;

namespace Examples.CodeGen.Genesis.Common
{
    public abstract class TypeDataProvider<T>
        : ICacheable
        , IConfigurable
        , IDataProvider
        where T : TypeDataProvider<T>
    {
        protected readonly static Type Type = typeof(T);

        public virtual string Name => Type.Name;

        public virtual int Priority => 0;

        public virtual bool RunInDryMode => true;

        protected IMemoryCache MemoryCache { get; private set; }

        protected AssembliesConfig AssembliesConfig { get; private set; }

        public virtual void SetCache(IMemoryCache memoryCache)
        {
            MemoryCache = memoryCache;
        }

        public virtual void Configure(IGenesisConfig genesisConfig)
        {
            AssembliesConfig = genesisConfig.CreateAndConfigure<AssembliesConfig>();
        }

        public virtual CodeGeneratorData[] GetData()
        {
            var codeGenData = new List<CodeGeneratorData>();

            var namedTypeSymbols = MemoryCache.GetNamedTypeSymbols();
            var filteredTypeSymbols = AssembliesConfig.FilterTypeSymbols(namedTypeSymbols);
            codeGenData.AddRange(GetCodeGeneratorData(filteredTypeSymbols));

            return codeGenData.ToArray();
        }

        protected abstract IEnumerable<CodeGeneratorData> GetCodeGeneratorData(
            IReadOnlyList<ICachedNamedTypeSymbol> typeSymbolInfo
        );
    }
}
