using System.Globalization;

namespace CompilerLexer;

public class TokenValue
{
    private readonly object value;

    public TokenValue(string value)
    {
        this.value = value;
    }

    public TokenValue(double value)
    {
        this.value = value;
    }

    public TokenValue( int value )
    {
        this.value = value;
    }

    public override string ToString()
    {
        return value switch
        {
            string s => s,
            double d => d.ToString( CultureInfo.InvariantCulture ),
            int i => i.ToString( CultureInfo.InvariantCulture ),
            _ => throw new InvalidOperationException( $"Unexpected type: {value.GetType()}" ),
        };
    }

    public double ToDouble()
    {
        return value switch
        {
            string s => double.Parse(s, CultureInfo.InvariantCulture),
            int i => i,
            double d => d,
            _ => throw new NotImplementedException(),
        };
    }

    public int ToInteger()
    {
        return value switch
        {
            string s => int.Parse( s, CultureInfo.InvariantCulture ),
            int i => i,
            double d => ( int )d,
            _ => throw new InvalidOperationException( $"Cannot convert {value.GetType()} to integer" ),
        };
    }

    public override bool Equals(object? obj)
    {
        if (obj is TokenValue other)
        {
            return value switch
            {
                string s => (string)other.value == s,
                double d => (double)other.value == d,
                int i => (int)other.value == i,
                _ => throw new NotImplementedException(),
            };
        }

        return false;
    }

    public override int GetHashCode()
    {
        return value.GetHashCode();
    }
}