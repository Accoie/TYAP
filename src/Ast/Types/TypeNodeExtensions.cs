using ValueType = Runtime.ValueType;

namespace Ast.Types;

public static class TypeNodeExtensions
{
    public static bool TypesMatch(TypeNode declared, TypeNode actual)
    {
        if (declared is ScalarTypeNode declaredScalar && actual is ScalarTypeNode actualScalar)
        {
            return declaredScalar.Type == actualScalar.Type;
        }

        if (declared is ArrayTypeNode declaredArray && actual is ArrayTypeNode actualArray)
        {
            return declaredArray.ElementType == actualArray.ElementType;
        }

        return false;
    }

    public static bool IsScalarType(TypeNode type, ValueType scalarType)
    {
        return type is ScalarTypeNode scalar && scalar.Type == scalarType;
    }
}
