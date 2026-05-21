using Ast.Types;

using ValueType = Runtime.ValueType;

namespace MsilCodegen;

/// <summary>
/// Отображает типы языка Pascal++ на соответствующие в .NET.
/// </summary>
public class TypeMapper
{
    private readonly Dictionary<string, Type> _typesMap = [];

    public Type MapType(ValueType type)
    {
        string key = $"scalar:{type}";
        if (!_typesMap.TryGetValue(key, out Type? result))
        {
            result = MapScalarType(type);
            _typesMap.Add(key, result);
        }

        return result;
    }

    public Type MapTypeNode(TypeNode type)
    {
        return type switch
        {
            ScalarTypeNode scalar => MapType(scalar.Type),
            ArrayTypeNode array => MapArrayType(array.ElementType),
            _ => throw new NotSupportedException($"Type {type} is not supported"),
        };
    }

    public Type MapArrayType(ValueType elementType)
    {
        string key = $"arr:{elementType}";
        if (!_typesMap.TryGetValue(key, out Type? result))
        {
            result = MapType(elementType).MakeArrayType();
            _typesMap.Add(key, result);
        }

        return result;
    }

    private static Type MapScalarType(ValueType type)
    {
        if (type == ValueType.Void)
        {
            return typeof(void);
        }

        if (type == ValueType.Integer)
        {
            return typeof(int);
        }

        if (type == ValueType.Float)
        {
            return typeof(double);
        }

        if (type == ValueType.String)
        {
            return typeof(string);
        }

        throw new NotSupportedException($"Type of Pascal++ - {type} - can't convert in .NET");
    }
}
