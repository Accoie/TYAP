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

    public UnexpectedLexemeException()
    {
    }

    public UnexpectedLexemeException(string? message)
        : base(message)
    {
    }

    public UnexpectedLexemeException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}