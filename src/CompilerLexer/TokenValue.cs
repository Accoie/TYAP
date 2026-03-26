using System.Globalization;

namespace CompilerLexer;

public class TokenValue
{
    private readonly object _value;

    public TokenValue(string value)
    {
        this._value = value;
    }

    public TokenValue(double value)
    {
        this._value = value;
    }

    public TokenValue(int value)
    {
        this._value = value;
    }

    public override string ToString()
    {
        return _value switch
        {
            string s => s,
            double d => d.ToString(CultureInfo.InvariantCulture),
            int i => i.ToString(CultureInfo.InvariantCulture),
            _ => throw new InvalidOperationException($"Unexpected type: {_value.GetType()}"),
        };
    }

    public double ToFloat()
    {
        return _value switch
        {
            string s => double.Parse(s, CultureInfo.InvariantCulture),
            int i => i,
            double d => d,
            _ => throw new InvalidOperationException($"Cannot convert {_value.GetType()} to double"),
        };
    }

    public int ToInteger()
    {
        return _value switch
        {
            string s => int.Parse(s, CultureInfo.InvariantCulture),
            int i => i,
            double d => (int)d,
            _ => throw new InvalidOperationException($"Cannot convert {_value.GetType()} to integer"),
        };
    }

    public override bool Equals(object? obj)
    {
        if (obj is TokenValue other)
        {
            return _value switch
            {
                string s => (string)other._value == s,
                double d => (double)other._value == d,
                int i => (int)other._value == i,
                _ => throw new NotImplementedException(),
            };
        }

        return false;
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }
}