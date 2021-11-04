using System;

namespace Examples.Common.Unions
{
    public interface IUnion
    {
        Type GetUnderlyingType();
    }
}