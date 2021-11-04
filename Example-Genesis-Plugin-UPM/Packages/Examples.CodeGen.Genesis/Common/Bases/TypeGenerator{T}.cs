using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Genesis.Plugin;

namespace Examples.CodeGen.Genesis.Common
{
    public abstract class TypeGenerator<TGenerator, TData> : ICacheable, ICodeGenerator
        where TGenerator : TypeGenerator<TGenerator, TData>
        where TData : TypeGeneratorData
    {
        protected readonly static Type Type = typeof(TGenerator);

        public string Name => Type.Name;

        public int Priority => 0;

        public bool RunInDryMode => false;

        protected IMemoryCache MemoryCache { get; private set; }

        private readonly StringBuilder _builder = new StringBuilder();

        public virtual void SetCache(IMemoryCache memoryCache)
        {
            MemoryCache = memoryCache;
        }

        public virtual CodeGenFile[] Generate(CodeGeneratorData[] data)
        {
            IEnumerable<TData> filteredData = data.OfType<TData>();

            var codeGenFiles = new List<CodeGenFile>();
            codeGenFiles.AddRange(filteredData.Select(CreateCodeGenFile));

            return codeGenFiles.ToArray();
        }

        protected virtual CodeGenFile CreateCodeGenFile(TData data)
        {
            _builder.Clear();
            string fileName = data.GetFileName(_builder);

            _builder.Clear();
            string code = CreateCode(data, _builder);

            return new CodeGenFile(fileName, code, Name);
        }

        protected abstract string CreateCode(TData def, StringBuilder builder);
    }
}
