using System.Globalization;

namespace Runtime;

public class Value : IEquatable<Value>
{
    public static readonly Value Void = new(VoidType.Value);

    private readonly object _value;

    public Value(string value)
    {
        this._value = value;
    }

    public Value(double value)
    {
        this._value = value;
    }

    public Value(int value)
    {
        this._value = value;
    }

    private Value(VoidType value)
    {
        this._value = value;
    }

    /// <summary>
    /// Возвращает тип значения.
    /// </summary>
    public ValueType GetValueType()
    {
        return _value switch
        {
            string => ValueType.String,
            double => ValueType.Float,
            int => ValueType.Integer,
            VoidType => ValueType.Void,
            _ => throw new InvalidOperationException($"Unexpected value {_value} of type {_value.GetType()}"),
        };
    }

    /// <summary>
    /// Возвращает значение как строку либо бросает исключение.
    /// </summary>
    public string AsString()
    {
        return _value switch
        {
            string s => s,
            _ => throw new InvalidOperationException($"Value {_value} is not a string"),
        };
    }

    /// <summary>
    /// Возвращает значение как дробное число либо бросает исключение.
    /// </summary>
    public double AsFloat()
    {
        return _value switch
        {
            double i => i,
            _ => throw new InvalidOperationException($"Value {_value} is not an double"),
        };
    }

    /// <summary>
    /// Возвращает значение как целое число либо бросает исключение.
    /// </summary>
    public int AsInteger()
    {
        return _value switch
        {
            int i => i,
            _ => throw new InvalidOperationException($"Value {_value} is not an double"),
        };
    }

    /// <summary>
    /// Печатает значение для отладки.
    /// </summary>
    public override string ToString()
    {
        return _value switch
        {
            string s => s,
            double d => d.ToString(CultureInfo.InvariantCulture),
            int i => i.ToString(CultureInfo.InvariantCulture),
            VoidType v => "void",
            _ => throw new InvalidOperationException($"Unexpected value {_value} of type {_value.GetType()}"),
        };
    }

    /// <summary>
    /// Сравнивает на равенство два значения.
    /// </summary>
    public bool Equals(Value? other)
    {
        if (other is null)
        {
            return false;
        }

        if (GetValueType() != other.GetValueType())
        {
            return false;
        }

        return _value switch
        {
            string s => other.AsString() == s,
            double d => Numbers.AreEqual(d, other.AsFloat()),
            int i => Numbers.AreEqual(i, other.AsInteger()),
            VoidType => true,
            _ => throw new NotImplementedException(),
        };
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Value);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }
}