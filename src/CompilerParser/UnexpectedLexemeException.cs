using CompilerLexer;

namespace CompilerParser;

public class UnexpectedLexemeException : Exception
{
    public UnexpectedLexemeException(TokenType expected, Token actual)
        : base($"Unexpected lexeme {actual.Type} where expected {expected}")
    {
    }

    public UnexpectedLexemeException(Token actual)
        : base($"Unexpected lexeme {actual.Type}")
    {
    }
}