using System.Globalization;

namespace Runtime;

public class Value : IEquatable<Value>
{
    public static readonly Value Void = new(VoidType.Value);

    private readonly object value;

    public Value(string value)
    {
        this.value = value;
    }

    public Value(double value)
    {
        this.value = value;
    }

    public Value(int value)
    {
        this.value = value;
    }

    private Value(VoidType value)
    {
        this.value = value;
    }

    /// <summary>
    /// Возвращает тип значения.
    /// </summary>
    public ValueType GetValueType()
    {
        return value switch
        {
            string => ValueType.String,
            double => ValueType.Float,
            int => ValueType.Integer,
            VoidType => ValueType.Void,
            _ => throw new InvalidOperationException($"Unexpected value {value} of type {value.GetType()}"),
        };
    }

    /// <summary>
    /// Возвращает значение как строку либо бросает исключение.
    /// </summary>
    public string AsString()
    {
        return value switch
        {
            string s => s,
            _ => throw new InvalidOperationException($"Value {value} is not a string"),
        };
    }

    /// <summary>
    /// Возвращает значение как дробное число либо бросает исключение.
    /// </summary>
    public double AsFloat()
    {
        return value switch
        {
            double i => i,
            _ => throw new InvalidOperationException($"Value {value} is not an double"),
        };
    }

    /// <summary>
    /// Возвращает значение как целое число либо бросает исключение.
    /// </summary>
    public int AsInteger()
    {
        return value switch
        {
            int i => i,
            _ => throw new InvalidOperationException($"Value {value} is not an double"),
        };
    }

    /// <summary>
    /// Печатает значение для отладки.
    /// </summary>
    public override string ToString()
    {
        return value switch
        {
            string s => s,
            double d => d.ToString(CultureInfo.InvariantCulture),
            int i => i.ToString(CultureInfo.InvariantCulture),
            VoidType v => "void",
            _ => throw new InvalidOperationException($"Unexpected value {value} of type {value.GetType()}"),
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

        return value switch
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
        return value.GetHashCode();
    }
}