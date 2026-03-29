namespace CompilerLexer;

public class TextScanner(string str) : IScanner
{
    private readonly string _text = str;
    private int _position;

    public char Peek(int n = 0)
    {
        int position = this._position + n;
        return position >= _text.Length ? '\0' : _text[position];
    }

    public void Advance()
    {
        _position++;
    }

    public bool IsEnd()
    {
        return _position >= _text.Length;
    }
}