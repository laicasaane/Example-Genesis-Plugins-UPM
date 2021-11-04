using System;

namespace Examples.Common
{
    public static partial class SystemTypeExtensions
    {
        public static string GetBuiltinOrName(this Type self)
        {
            if (self.TryGetBuiltinName(out var name))
                return name;

            return self.Name;
        }

        public static string GetBuiltinOrFullName(this Type self)
        {
            if (self.TryGetBuiltinName(out var name))
                return name;

            return self.FullName;
        }
    }
}