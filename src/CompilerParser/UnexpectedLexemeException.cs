using CompilerLexer;

namespace CompilerParser;

#pragma warning disable RCS1194 // Конструкторы исключения не нужны, т.к. это не класс общего назначения.

public class UnexpectedLexemeException : Exception
{
    public UnexpectedLexemeException(TokenType expected, Token actual)
        : base($"Unexpected lexeme {actual.Type} where expected {expected}")
    {
        Actual = actual.Type;
    }

    public UnexpectedLexemeException(Token actual)
        : base($"Unexpected lexeme {actual.Type}")
    {
        Actual = actual.Type;
    }

    public TokenType Actual { get; }
}

#pragma warning restore RCS1194