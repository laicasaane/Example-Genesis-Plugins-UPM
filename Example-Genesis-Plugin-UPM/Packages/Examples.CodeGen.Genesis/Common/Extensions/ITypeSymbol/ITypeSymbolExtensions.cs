using Microsoft.CodeAnalysis;

namespace Examples.CodeGen.Genesis.Common
{
    public static class ITypeSymbolExtensions
    {
        private const string EQUALS = nameof(Equals);
        private const string OP_EQUALITY = "op_Equality";
        private const int SIZE_OF_REF = sizeof(int);

        public static int SizeOf(this ITypeSymbol typeSymbol)
        {
            var size = SizeOfPrimitive(typeSymbol);

            if (size >= 0)
                return size;

            if (typeSymbol.IsValueType == false)
                return SIZE_OF_REF;

            size = 0;

            foreach (ISymbol member in typeSymbol.GetMembers())
            {
                if (member.IsStatic)
                    continue;

                var memberSize = 0;

                if (member.Kind == SymbolKind.Field)
                    memberSize = SizeOf(((IFieldSymbol)member).Type);
                else if (member.Kind == SymbolKind.Event)
                    memberSize = SIZE_OF_REF;

                if (memberSize > 0)
                    size += memberSize;
            }

            return size;
        }

        private static int SizeOfPrimitive(ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                return 0;

            switch (typeSymbol.SpecialType)
            {
                case SpecialType.System_Boolean:
                    return sizeof(bool);

                case SpecialType.System_Byte:
                    return sizeof(byte);

                case SpecialType.System_Char:
                    return sizeof(char);

                case SpecialType.System_Decimal:
                    return sizeof(decimal);

                case SpecialType.System_Double:
                    return sizeof(double);

                case SpecialType.System_Int16:
                    return sizeof(short);

                case SpecialType.System_Int32:
                    return sizeof(int);

                case SpecialType.System_Int64:
                    return sizeof(long);

                case SpecialType.System_SByte:
                    return sizeof(sbyte);

                case SpecialType.System_Single:
                    return sizeof(float);

                case SpecialType.System_UInt16:
                    return sizeof(ushort);

                case SpecialType.System_UInt32:
                    return sizeof(uint);

                case SpecialType.System_UInt64:
                    return sizeof(ulong);
            }

            if (typeSymbol.SpecialType == SpecialType.System_Enum
                && typeSymbol is INamedTypeSymbol namedTypeSymbol)
            {
                return SizeOfPrimitive(namedTypeSymbol.EnumUnderlyingType);
            }

            return -1;
        }

        public static EqualityKind GetEqualityKind(this ITypeSymbol type)
        {
            if (type.HasEqualityOperator())
                return EqualityKind.EqualityOperator;

            if (type.HasMemberEqualsMethod())
                return EqualityKind.MemberEquals;

            if (type.HasStaticEqualsMethod())
                return EqualityKind.StaticEquals;

            return EqualityKind.Undefined;
        }

        public static bool HasMemberEqualsMethod(this ITypeSymbol type)
        {
            switch (type.SpecialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Decimal:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_String:
                case SpecialType.System_IntPtr:
                case SpecialType.System_UIntPtr:
                case SpecialType.System_DateTime:
                    return true;
            }

            foreach (ISymbol op in type.GetMembers(EQUALS))
            {
                if (op.IsStatic == true || !(op is IMethodSymbol opMethod))
                    continue;

                var comparer = SymbolEqualityComparer.Default;

                if (opMethod.Parameters.Length == 1
                    && comparer.Equals(type, opMethod.Parameters[0].Type)
                )
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasStaticEqualsMethod(this ITypeSymbol type)
        {
            switch (type.SpecialType)
            {
                case SpecialType.System_Decimal:
                case SpecialType.System_String:
                case SpecialType.System_DateTime:
                    return true;
            }

            foreach (ISymbol op in type.GetMembers(EQUALS))
            {
                if (op.IsStatic == false || !(op is IMethodSymbol opMethod))
                    continue;

                var comparer = SymbolEqualityComparer.Default;

                if (opMethod.Parameters.Length == 2
                    && comparer.Equals(type, opMethod.Parameters[0].Type)
                    && comparer.Equals(type, opMethod.Parameters[1].Type)
                )
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasEqualityOperator(this ITypeSymbol type)
        {
            switch (type.SpecialType)
            {
                case SpecialType.System_Enum:
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Decimal:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_String:
                case SpecialType.System_IntPtr:
                case SpecialType.System_UIntPtr:
                case SpecialType.System_DateTime:
                    return true;
            }

            if (type.TypeKind == TypeKind.Enum)
            {
                return true;
            }

            foreach (ISymbol op in type.GetMembers(OP_EQUALITY))
            {
                if (!(op is IMethodSymbol opMethod))
                    continue;

                var comparer = SymbolEqualityComparer.Default;

                if (opMethod.Parameters.Length == 2
                    && comparer.Equals(type, opMethod.Parameters[0].Type)
                    && comparer.Equals(type, opMethod.Parameters[1].Type)
                )
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TryGetBuiltinName(ITypeSymbol type, out string name)
        {
            switch (type.SpecialType)
            {
                case SpecialType.System_Object:
                    name = "object";
                    return true;

                case SpecialType.System_Boolean:
                    name = "bool";
                    return true;

                case SpecialType.System_Char:
                    name = "char";
                    return true;

                case SpecialType.System_SByte:
                    name = "sbyte";
                    return true;

                case SpecialType.System_Byte:
                    name = "byte";
                    return true;

                case SpecialType.System_Int16:
                    name = "short";
                    return true;

                case SpecialType.System_UInt16:
                    name = "ushort";
                    return true;

                case SpecialType.System_Int32:
                    name = "int";
                    return true;

                case SpecialType.System_UInt32:
                    name = "uint";
                    return true;

                case SpecialType.System_Int64:
                    name = "long";
                    return true;

                case SpecialType.System_UInt64:
                    name = "ulong";
                    return true;

                case SpecialType.System_Decimal:
                    name = "decimal";
                    return true;

                case SpecialType.System_Single:
                    name = "float";
                    return true;

                case SpecialType.System_Double:
                    name = "double";
                    return true;

                case SpecialType.System_String:
                    name = "string";
                    return true;
            }

            name = string.Empty;
            return false;
        }

        public static string GetBuiltinOrName(this ITypeSymbol type)
        {
            if (TryGetBuiltinName(type, out string name))
                return name;

            return type.Name;
        }

        public static string GetBuiltinOrFullName(this ITypeSymbol type)
        {
            if (TryGetBuiltinName(type, out string name))
                return name;

            return type.ToString();
        }
    }
}