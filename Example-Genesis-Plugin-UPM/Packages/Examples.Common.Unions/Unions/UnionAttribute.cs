using System;

namespace Examples.Common.Unions
{
    [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class UnionAttribute : Attribute
    {
        public Type TupleType { get; }

        public InvalidAccessKind InvalidAccessKind { get; }

        public UnionAttribute(Type tupleType)
        {
            TupleType = tupleType;
        }

        public UnionAttribute(Type tupleType, InvalidAccessKind invalidAccessKind)
        {
            TupleType = tupleType;
            InvalidAccessKind = invalidAccessKind;
        }
    }
}