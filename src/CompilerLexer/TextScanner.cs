namespace CompilerLexer;

public class TextScanner(string str) : IScanner
{
    private int _position;

    public char Peek(int n = 0)
    {
        int position = this._position + n;
        return position >= str.Length ? '\0' : str[position];
    }

    public void Advance()
    {
        _position++;
    }

    public bool IsEnd()
    {
        return _position >= str.Length;
    }
}