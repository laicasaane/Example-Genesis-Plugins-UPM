using System.Collections.Generic;
using System.Text;

namespace Examples.CodeGen.Genesis.Common
{
    public static class FileNameHelper
    {
        public static string Combine(
              StringBuilder builder
            , string format
            , string @namespace
            , string typeName
            , char deliminator = '-'
            , IEnumerable<TypeDefinition> containingTypes = null
        )
        {
            builder.Clear();
            builder.Append(typeName);

            if (containingTypes != null)
            {
                foreach (TypeDefinition type in containingTypes)
                {
                    builder.Append(deliminator).Append(type.TypeSymbol.Name);
                }
            }

            builder.Append(deliminator).Append(@namespace);

            return string.Format(format, builder.ToString());
        }
    }
}
