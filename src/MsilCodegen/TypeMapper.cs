using ValueType = Runtime.ValueType;

namespace MsilCodegen;

/// <summary>
/// Отображает типы языка Pascal++ на соответствующие в .NET.
/// </summary>
public class TypeMapper
{
    private readonly Dictionary<ValueType, Type> _typesMap = [];

    public Type MapType(ValueType type)
    {
        if (!_typesMap.TryGetValue(type, out Type? result))
        {
            result = MapTypeImpl(type);
            _typesMap.Add(type, result);
        }

        return result;
    }

    private Type MapTypeImpl(ValueType type)
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
            return typeof (double);
        }

        if (type == ValueType.String)
        {
            return typeof(string);
        }

        throw new NotSupportedException($"Type of Pascal++ - {type} - can't convert in .NET");
    }
}