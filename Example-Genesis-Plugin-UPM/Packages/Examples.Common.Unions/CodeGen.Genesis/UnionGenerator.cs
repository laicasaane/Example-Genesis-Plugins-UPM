using System.Text;
using Microsoft.CodeAnalysis;
using Examples.CodeGen.Genesis.Common;
using Examples.Common;

namespace Examples.CodeGen.Genesis.Plugins.Unions
{
    internal sealed class UnionGenerator : TypeGenerator<UnionGenerator, UnionGeneratorData>
    {
        private const string FIELDS_NAME = "Fields";
        private const string FIELD_NAME = "Field";

        protected override string CreateCode(UnionGeneratorData def, StringBuilder builder)
        {
            builder.Append($@"
using System;
using System.Runtime.InteropServices;
using Examples.Common.Unions;

namespace {def.Namespace}
{{");

            AppendContainingTypes_Begin(def, builder);

            builder.Append($@"
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    partial struct {def.Name} : IUnion, IEquatable<{def.Name}>
    {{");

            AppendFieldsEnum(def, builder);
            AppendFields_Field(def, builder);
            AppendProps(def, builder);
            AppendConstructors(def, builder);
            AppendConstructor_FieldsParam(def, builder);

            if (!def.IsReadOnly)
                AppendMethod_Set(def, builder);

            AppendMethod_TryGet(def, builder);
            AppendMethod_GetUnderlyingType(def, builder);
            AppendMethod_GetHashCode(def, builder);
            AppendMethod_Equals_Object(def, builder);
            AppendMethod_Equals(def, builder, string.Empty);
            AppendMethod_Equals(def, builder, "in ");
            AppendMethod_ToString(def, builder);
            AppendOperator_Implicit(def, builder);

            builder.Append(@"
    }
");

            AppendContainingTypes_End(def, builder);
            builder.Append(@"}
");

            return builder.ToString();
        }

        private static void AppendMethod_TryGet(UnionGeneratorData def, StringBuilder builder)
        {
            var prefix = def.GetFieldPrefix();

            foreach (FieldDefinition field in def.GetFields())
            {
                builder.Append($@"
        public bool TryGet(out {field.Type} value)
        {{
            if ({prefix}{FIELD_NAME} != {FIELDS_NAME}.{field.FieldSymbol.Name})
            {{
                value = default;
                return false;
            }}

            value = {prefix}{field.FieldSymbol.Name};
            return true;
        }}
");
            }
        }

        private static void AppendMethod_GetUnderlyingType(UnionGeneratorData def, StringBuilder builder)
        {
            var prefix = def.GetFieldPrefix();
            var fields = def.GetFields();

            builder.Append($@"
        public System.Type GetUnderlyingType()");

            if (fields.Count < Constants.SWITCH_MIN_CASE_COUNT)
            {
                builder.Append(@"
        {");

                foreach (FieldDefinition field in fields)
                {
                    builder.Append($@"
            if ({prefix}{FIELD_NAME} == {FIELDS_NAME}.{field.FieldSymbol.Name})
                return {prefix}{field.FieldSymbol.Name}.GetType();
");
                }

                builder.Append(@"
            return GetType();
        }
");
            }
            else
            {
                builder.Append($@"
        {{
            switch ({prefix}{FIELD_NAME})
            {{");

                foreach (FieldDefinition field in fields)
                {
                    builder.Append($@"
                case {FIELDS_NAME}.{field.FieldSymbol.Name}: return {prefix}{field.FieldSymbol.Name}.GetType();");
                }

                builder.Append(@"
            }

            return GetType();
        }
");
            }
        }

        private static void AppendMethod_Set(UnionGeneratorData def, StringBuilder builder)
        {
            var prefix = def.GetFieldPrefix();
            var fields = def.GetFields();

            foreach (FieldDefinition field in fields)
            {
                builder.Append($@"
        public void Set({field.Type} value)
        {{
            {prefix}{FIELD_NAME} = {FIELDS_NAME}.{field.FieldSymbol.Name};
            {prefix}{field.FieldSymbol.Name} = value;
        }}
");
            }
        }

        private static void AppendMethod_ToString(UnionGeneratorData def, StringBuilder builder)
        {
            var prefix = def.GetFieldPrefix();
            var fields = def.GetFields();

            builder.Append(@"
        public override string ToString()
        {");

            if (fields.Count < Constants.SWITCH_MIN_CASE_COUNT)
            {
                foreach (FieldDefinition field in fields)
                {
                    builder.Append($@"
            if ({prefix}{FIELD_NAME} == {FIELDS_NAME}.{field.FieldSymbol.Name})
                return {prefix}{field.FieldSymbol.Name}.ToString();
");
                }

                builder.Append(@"
            return string.Empty;
        }");
            }
            else
            {
                builder.Append($@"
            switch ({prefix}{FIELD_NAME})
            {{");

                foreach (FieldDefinition field in fields)
                {
                    builder.Append($@"
                case {FIELDS_NAME}.{field.FieldSymbol.Name}: return {prefix}{field.FieldSymbol.Name}.ToString();");
                }

                builder.Append(@"
            }

            return string.Empty;
        }");
            }
        }

        private static void AppendMethod_GetHashCode(UnionGeneratorData def, StringBuilder builder)
        {
            var prefix = def.GetFieldPrefix();
            var fields = def.GetFields();

            builder.Append($@"
        public override int GetHashCode()
        {{
            var hash = new HashCode();
            hash.Add({prefix}{FIELD_NAME});
");

            if (fields.Count < Constants.SWITCH_MIN_CASE_COUNT)
            {
                foreach (FieldDefinition field in fields)
                {
                    builder.Append($@"
            if ({prefix}{FIELD_NAME} == {FIELDS_NAME}.{field.FieldSymbol.Name})
                hash.Add({prefix}{field.FieldSymbol.Name});
");
                }

                builder.Append(@"
            return hash.ToHashCode();
        }
");
            }
            else
            {
                builder.Append($@"
            switch ({prefix}{FIELD_NAME})
            {{");

                foreach (FieldDefinition field in fields)
                {
                    builder.Append($@"
                case {FIELDS_NAME}.{field.FieldSymbol.Name}: return hash.Add({prefix}{field.FieldSymbol.Name});");
                }

                builder.Append(@"
            }

            return hash.ToHashCode();
        }
");
            }
        }

        private static void AppendMethod_Equals(
              UnionGeneratorData def
            , StringBuilder builder
            , string @in
        )
        {
            var prefix = def.GetFieldPrefix();
            var fields = def.GetFields();

            builder.Append($@"
        public bool Equals({@in}{def.Name} other)
            => Equals({@in}this, {@in}other);
");

            builder.Append($@"
        public static bool Equals({@in}{def.Name} a, {@in}{def.Name} b)");

            if (fields.Count < Constants.SWITCH_MIN_CASE_COUNT)
            {
                builder.Append($@"
        {{
            if (a.{prefix}{FIELD_NAME} != b.{prefix}{FIELD_NAME})
                return false;
");

                foreach (FieldDefinition field in fields)
                {
                    builder.Append($@"
            if (a.{prefix}{FIELD_NAME} == {FIELDS_NAME}.{field.FieldSymbol.Name})");

                    switch (field.Type.Equality)
                    {
                        case EqualityKind.EqualityOperator:
                            builder.Append($@"
                return a.{prefix}{field.FieldSymbol.Name} == b.{prefix}{field.FieldSymbol.Name};
");
                            break;

                        case EqualityKind.MemberEquals:
                            builder.Append($@"
                return a.{prefix}{field.FieldSymbol.Name}.Equals(b.{prefix}{field.FieldSymbol.Name});
");
                            break;

                        case EqualityKind.StaticEquals:
                            builder.Append($@"
                return {field.Type}.Equals(a.{prefix}{field.FieldSymbol.Name}, b.{prefix}{field.FieldSymbol.Name});
");
                            break;

                        default:
                            builder.Append($@"
                return System.Collections.Generic.EqualityComparer<{field.Type}>.Default.Equals(a.{prefix}{field.FieldSymbol.Name}, b.{prefix}{field.FieldSymbol.Name});
");
                            break;
                    }
                }

                builder.Append(@"
            return false;
        }
");
            }
            else
            {
                builder.Append($@"
        {{
            if (a.{prefix}{FIELD_NAME} != b.{prefix}{FIELD_NAME})
                return false;

            switch (a.{prefix}{FIELD_NAME})
            {{");

                foreach (FieldDefinition field in fields)
                {
                    builder.Append($@"
                case {FIELDS_NAME}.{field.FieldSymbol.Name}:
                    System.Collections.Generic.return EqualityComparer<{field.Type}>.Default.Equals(a.{prefix}{field.FieldSymbol.Name}, b.{prefix}{field.FieldSymbol.Name});");
                }

                builder.Append(@"
            }

            return false;
        }
");
            }

            builder.Append($@"
        public static bool operator ==({@in}{def.Name} left, {@in}{def.Name} right)
            => Equals({@in}left, {@in}right);

        public static bool operator !=({@in}{def.Name} left, {@in}{def.Name} right)
            => !Equals({@in}left, {@in}right);
");
        }

        private static void AppendMethod_Equals_Object(UnionGeneratorData def, StringBuilder builder)
        {
            builder.Append($@"
        public override bool Equals(object obj)
            => obj is {def.Name} other && Equals(this, other);
");
        }

        private static void AppendOperator_Implicit(UnionGeneratorData def, StringBuilder builder)
        {
            var prefix = def.GetFieldPrefix();
            var fields = def.GetFields();
            var last = fields.Count - 1;

            switch (def.InvalidAccessKind)
            {
                case InvalidAccessKind.ThrowException:
                case InvalidAccessKind.ThrowExceptionInDebug:
                {
                    var isInDebug = def.InvalidAccessKind == InvalidAccessKind.ThrowExceptionInDebug;
                    var fromSecond = false;
                    var methodName = nameof(SystemTypeExtensions.GetBuiltinOrFullName);

                    for (var i = 0; i < fields.Count; i++)
                    {
                        FieldDefinition field = fields[i];

                        if (field.Type.TypeSymbol.TypeKind == TypeKind.Interface)
                            continue;

                        if (fromSecond == false)
                        {
                            builder.AppendLine();
                            fromSecond = true;
                        }

                        AppendOperator_Implicit_ThrowException(
                              def
                            , builder
                            , prefix
                            , isInDebug
                            , methodName
                            , field.Type
                            , field
                        );

                        if (field.Type.FindNullableUnderlyingType(out var definition))
                        {
                            AppendOperator_Implicit_ThrowException(
                                  def
                                , builder
                                , prefix
                                , isInDebug
                                , methodName
                                , definition
                                , field
                                , false
                            );
                        }

                        if (i < last)
                            builder.AppendLine();
                    }
                    break;
                }

                case InvalidAccessKind.ReturnDefault:
                {
                    var fromSecond = false;

                    for (var i = 0; i < fields.Count; i++)
                    {
                        FieldDefinition field = fields[i];

                        if (field.Type.TypeSymbol.TypeKind == TypeKind.Interface)
                            continue;

                        if (fromSecond == false)
                        {
                            builder.AppendLine();
                            fromSecond = true;
                        }

                        AppendOperator_Implicit_ReturnDefault(
                              def
                            , builder
                            , prefix
                            , field.Type
                            , field
                        );

                        if (field.Type.FindNullableUnderlyingType(out var definition))
                        {
                            AppendOperator_Implicit_ReturnDefault(
                                  def
                                , builder
                                , prefix
                                , definition
                                , field
                                , false
                            );
                        }

                        if (i < last)
                            builder.AppendLine();
                    }
                    break;
                }

                default:
                {
                    for (var i = 0; i < fields.Count; i++)
                    {
                        FieldDefinition field = fields[i];

                        if (i <= 0)
                            builder.AppendLine();

                        AppendOperator_Implicit_Allow(
                              def
                            , builder
                            , prefix
                            , field.Type
                            , field
                        );

                        if (field.Type.FindNullableUnderlyingType(out var definition))
                        {
                            AppendOperator_Implicit_Allow(
                                  def
                                , builder
                                , prefix
                                , definition
                                , field
                                , false
                            );
                        }

                        if (i < last)
                            builder.AppendLine();
                    }
                    break;
                }
            }
        }

        private static void AppendOperator_Implicit_Allow(
              UnionGeneratorData def
            , StringBuilder builder
            , string prefix
            , TypeDefinition type
            , FieldDefinition field
            , bool toBaseType = true
        )
        {
            builder.Append($@"
        public static implicit operator {def.Name}({type} value)
            => new {def.Name}(value);");

            if (toBaseType == false)
                return;

            builder.Append($@"

        public static implicit operator {type}({def.Name} value)
            => value.{prefix}{field.FieldSymbol.Name};");
        }

        private static void AppendOperator_Implicit_ReturnDefault(
              UnionGeneratorData def
            , StringBuilder builder
            , string prefix
            , TypeDefinition type
            , FieldDefinition field
            , bool toBaseType = true
        )
        {
            builder.Append($@"
        public static implicit operator {def.Name}({type} value)
            => new {def.Name}(value);");

            if (toBaseType == false)
                return;

            builder.Append($@"

        public static implicit operator {type}({def.Name} value)
        {{
            if (value.{prefix}{FIELD_NAME} == {FIELDS_NAME}.{field.FieldSymbol.Name})
                return value.{prefix}{field.FieldSymbol.Name};

            return default;
        }}");
        }

        private static void AppendOperator_Implicit_ThrowException(
              UnionGeneratorData def
            , StringBuilder builder
            , string prefix
            , bool isInDebug
            , string methodName
            , TypeDefinition type
            , FieldDefinition field
            , bool toBaseType = true
        )
        {
            builder.Append($@"
        public static implicit operator {def.Name}({type} value)
            => new {def.Name}(value);");

            if (toBaseType == false)
                return;

            builder.Append($@"

        public static implicit operator {type}({def.Name} value)
        {{");
            if (isInDebug)
            {
                builder.Append(@"
#if DEBUG");
            }

            builder.Append($@"
            if (value.{prefix}{FIELD_NAME} != {FIELDS_NAME}.{field.FieldSymbol.Name})
            {{
                var typeName = value.GetUnderlyingType().{methodName}();
                throw new {nameof(InvalidAccessException)}($""Cannot implicitly convert underlying type '{{typeName}}' to '{type}'"");
            }}");

            if (isInDebug)
            {
                builder.Append(@"
#endif");
            }

            builder.Append($@"
            return value.{prefix}{field.FieldSymbol.Name};
        }}
");
        }

        private static void AppendConstructor_FieldsParam(UnionGeneratorData def, StringBuilder builder)
        {
            var prefix = def.GetFieldPrefix();
            var fields = def.GetFields();

            builder.Append($@"
        public {def.Name}({FIELDS_NAME} type)
        {{
            {prefix}{FIELD_NAME} = type;
");

            foreach (FieldDefinition field in fields)
            {
                builder.Append($@"
            {prefix}{field.FieldSymbol.Name} = default;");
            }

            builder.Append(@"
        }
");
        }

        private static void AppendConstructors(UnionGeneratorData def, StringBuilder builder)
        {
            var prefix = def.GetFieldPrefix();
            var fields = def.GetFields();
            var last = fields.Count - 1;

            for (var i = 0; i < fields.Count; i++)
            {
                FieldDefinition field = fields[i];

                if (i > 0)
                    builder.AppendLine();

                AppendConstructor(def, builder, prefix, i, field.Type, field, string.Empty);
                builder.AppendLine();
                AppendConstructor(def, builder, prefix, i, field.Type, field, "in ");

                if (field.Type.FindNullableUnderlyingType(out var definition))
                {
                    builder.AppendLine();
                    AppendConstructor(def, builder, prefix, i, definition, field, string.Empty);
                    builder.AppendLine();
                    AppendConstructor(def, builder, prefix, i, definition, field, "in ");
                }

                if (i == last)
                    builder.AppendLine();
            }
        }

        private static void AppendConstructor(
              UnionGeneratorData def
            , StringBuilder builder
            , string prefix
            , int index
            , TypeDefinition type
            , FieldDefinition field
            , string @in
        )
        {
            var fields = def.GetFields();

            builder.Append($@"
        public {def.Name}({@in}{type} value)
        {{
            {prefix}{FIELD_NAME} = {FIELDS_NAME}.{field.FieldSymbol.Name};
");

            for (var k = 0; k < fields.Count; k++)
            {
                if (k == index)
                    continue;

                FieldDefinition otherField = fields[k];

                builder.Append($@"
            {prefix}{otherField.FieldSymbol.Name} = default;");
            }

            builder.Append($@"

            {prefix}{field.FieldSymbol.Name} = value;
        }}");
        }

        private static void AppendProps(UnionGeneratorData def, StringBuilder builder)
        {
            var fields = def.GetFields();

            if (def.IsReadOnly && def.InvalidAccessKind == InvalidAccessKind.Allow)
            {
                foreach (FieldDefinition field in fields)
                {
                    builder.Append($@"
        [FieldOffset(1)]
        public readonly {field.Type} {field.FieldSymbol.Name};
");
                }

                return;
            }

            var readonlyKeyword = def.IsReadOnly ? "readonly " : string.Empty;
            var prefix = Tokens.NAMING_PREFIX;

            foreach (FieldDefinition field in fields)
            {
                builder.Append($@"
        [FieldOffset(1)]
        private {readonlyKeyword}{field.Type} {prefix}{field.FieldSymbol.Name};
");
            }

            switch (def.InvalidAccessKind)
            {
                case InvalidAccessKind.ThrowException:
                case InvalidAccessKind.ThrowExceptionInDebug:
                {
                    var isInDebug = def.InvalidAccessKind == InvalidAccessKind.ThrowExceptionInDebug;
                    var methodName = nameof(SystemTypeExtensions.GetBuiltinOrFullName);

                    foreach (FieldDefinition field in fields)
                    {
                        builder.Append($@"
        public {field.Type} {field.FieldSymbol.Name}
        {{
            get
            {{");

                        if (isInDebug)
                        {
                            builder.Append(@"
#if DEBUG");
                        }

                        builder.Append($@"
                if ({prefix}{FIELD_NAME} != {FIELDS_NAME}.{field.FieldSymbol.Name})
                {{
                    var typeName = GetUnderlyingType().{methodName}();
                    throw new {nameof(InvalidAccessException)}($""Cannot convert underlying type '{{typeName}}' to '{field.Type}'"");
                }}");

                        if (isInDebug)
                        {
                            builder.Append(@"
#endif");
                        }

                        builder.Append($@"
                return {prefix}{field.FieldSymbol.Name};
            }}
        }}
");
                    }
                    break;
                }

                case InvalidAccessKind.ReturnDefault:
                {
                    foreach (FieldDefinition field in fields)
                    {
                        builder.Append($@"
        public {field.Type} {field.FieldSymbol.Name}
        {{
            get
            {{
                if ({prefix}{FIELD_NAME} == {FIELDS_NAME}.{field.FieldSymbol.Name})
                    return {prefix}{field.FieldSymbol.Name};

                return default;
            }}
        }}
");
                    }
                    break;
                }

                default:
                {
                    foreach (FieldDefinition field in fields)
                    {
                        builder.Append($@"
        public {field.Type} {field.FieldSymbol.Name} => {prefix}{field.FieldSymbol.Name};
");
                    }
                    break;
                }
            }
        }

        private static void AppendFields_Field(UnionGeneratorData def, StringBuilder builder)
        {
            if (def.IsReadOnly && def.InvalidAccessKind == InvalidAccessKind.Allow)
            {
                builder.Append($@"
        [FieldOffset(0)]
        public readonly {FIELDS_NAME} {FIELD_NAME};
");
            }
            else
            {
                var readonlyKeyword = def.IsReadOnly ? "readonly " : string.Empty;
                var prefix = Tokens.NAMING_PREFIX;

                builder.Append($@"
        [FieldOffset(0)]
        private {readonlyKeyword}{FIELDS_NAME} {prefix}{FIELD_NAME};

        public {FIELDS_NAME} {FIELD_NAME} => {prefix}{FIELD_NAME};
");
            }
        }

        private static void AppendFieldsEnum(UnionGeneratorData def, StringBuilder builder)
        {
            var fields = def.GetFields();

            builder.Append($@"
        public enum {FIELDS_NAME} : {def.FieldEnumUnderlyingType}
        {{");

            foreach (FieldDefinition field in fields)
            {
                builder.Append($@"
            {field.FieldSymbol.Name},");

            }

            builder.Append(@"
        }
");
        }

        private static void AppendContainingTypes_Begin(UnionGeneratorData def, StringBuilder builder)
        {
            var types = def.GetContainingTypes();

            if (types.Count <= 0)
                return;

            builder.AppendLine();

            var last = types.Count - 1;

            for (var i = last; i >= 0; i--)
            {
                TypeDefinition type = types[i];
                TypeKind kind = type.TypeSymbol.TypeKind;

                switch (kind)
                {
                    case TypeKind.Class:
                    case TypeKind.Struct:
                    case TypeKind.Interface:
                        builder.AppendLine($"partial {kind.ToString().ToLower()} {type.TypeSymbol.Name} {{");
                        break;
                }
            }
        }

        private static void AppendContainingTypes_End(UnionGeneratorData def, StringBuilder builder)
        {
            var last = def.GetContainingTypes().Count - 1;

            for (var i = last; i >= 0; i--)
            {
                builder.AppendLine(@"}");
            }
        }
    }
}
