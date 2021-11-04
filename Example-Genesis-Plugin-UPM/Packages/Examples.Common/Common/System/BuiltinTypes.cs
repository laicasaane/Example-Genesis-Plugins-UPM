using System;
using System.Collections.Generic;

namespace Examples.Common
{
    public static partial class BuiltinTypes
    {
        public static class Name
        {
            public const string OBJECT  = "object";
            public const string BOOL    = "bool";
            public const string BYTE    = "byte";
            public const string SBYTE   = "sbyte";
            public const string SHORT   = "short";
            public const string USHORT  = "ushort";
            public const string INT     = "int";
            public const string UINT    = "uint";
            public const string LONG    = "long";
            public const string ULONG   = "ulong";
            public const string FLOAT   = "float";
            public const string DOUBLE  = "double";
            public const string CHAR    = "char";
            public const string DECIMAL = "decimal";
            public const string STRING  = "string";
        }

        private static readonly Dictionary<Type, string> s_typeNameMap = new Dictionary<Type, string> {
            { typeof(object) , Name.OBJECT  },
            { typeof(bool)   , Name.BOOL    },
            { typeof(byte)   , Name.BYTE    },
            { typeof(sbyte)  , Name.SBYTE   },
            { typeof(short)  , Name.SHORT   },
            { typeof(ushort) , Name.USHORT  },
            { typeof(int)    , Name.INT     },
            { typeof(uint)   , Name.UINT    },
            { typeof(long)   , Name.LONG    },
            { typeof(ulong)  , Name.ULONG   },
            { typeof(float)  , Name.FLOAT   },
            { typeof(double) , Name.DOUBLE  },
            { typeof(char)   , Name.CHAR    },
            { typeof(string) , Name.STRING  },
            { typeof(decimal), Name.DECIMAL },
        };

        public static bool TryGetBuiltinName(this Type self, out string name)
        {
            if (!s_typeNameMap.TryGetValue(self, out name))
            {
                name = string.Empty;
            }

            return !string.IsNullOrWhiteSpace(name);
        }

        public static string Cast(string value)
            => $"({value})";

        public static bool IsUlong(string value)
            => string.Equals(value, Name.ULONG);

        public static bool IsFloat(string value)
            => string.Equals(value, Name.FLOAT);

        public static bool IsDouble(string value)
            => string.Equals(value, Name.DOUBLE);
    }
}
