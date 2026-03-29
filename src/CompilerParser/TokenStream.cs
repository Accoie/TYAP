using CompilerLexer;

namespace CompilerParser;

public class TokenStream
{
    private readonly Lexer _lexer;
    private Token _nextToken;

    public TokenStream(string text)
    {
        TextScanner scanner = new TextScanner(text);
        _lexer = new Lexer(scanner);
        _nextToken = _lexer.ParseToken();
    }

    public Token Peek()
    {
        return _nextToken;
    }

    public void Advance()
    {
        _nextToken = _lexer.ParseToken();
    }
}