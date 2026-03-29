using Ast.Expressions;
using Ast.Statements;

using CompilerLexer;

using Runtime;

using ValueType = Runtime.ValueType;

namespace CompilerParser;

public class Parser
{
    private readonly TokenStream _tokens;

    private readonly Stack<ValueType> _returnTypes = new();

    public Parser(string code)
    {
        _tokens = new TokenStream(code);
    }

    /// <summary>
    /// Выполняет разбор выражения RusMatushka
    /// Правило: program = "begin", { statement }, "end".
    /// </summary>
    public BlockStatement ParseProgram()
    {
        return ParseBlock(true);
    }

    /// <summary>
    /// Разбирает блок кода.
    /// Правило: block = "begin", { statement }, "end".
    /// </summary>
    private BlockStatement ParseBlock(bool isNew)
    {
        Match(TokenType.Begin);

        List<Statement> statements = [];
        while (_tokens.Peek().Type != TokenType.End && _tokens.Peek().Type != TokenType.EndOfFile)
        {
            Statement node = ParseStatement();
            statements.Add(node);
        }

        Match(TokenType.End);

        return new BlockStatement(statements, isNew);
    }

    /// <summary>
    /// Разбирает инструкции.
    /// Правило: statement = output_statement.
    /// </summary>
    private Statement ParseStatement()
    {
        TokenType token = _tokens.Peek().Type;

        return token switch
        {
            TokenType.Begin => ParseBlock(true),
            TokenType.Output => ParseOutput(),
            _ => throw new UnexpectedLexemeException(_tokens.Peek()),
        };
    }

    /// <summary>
    /// Разбирает оператор вывода.
    /// Правило: output_statement = "write", "(", argument_list, ")", ";".
    /// </summary>
    private OutputStatement ParseOutput()
    {
        Match(TokenType.Output);
        Match(TokenType.LParen);

        List<Expression> arguments = [ParseExpression()];

        while (_tokens.Peek().Type == TokenType.Comma)
        {
            _tokens.Advance();
            arguments.Add(ParseExpression());
        }

        Match(TokenType.RParen);
        Match(TokenType.Semicolon);
        return new OutputStatement(arguments);
    }

    /// <summary>
    /// Разбирает выражение.
    /// Правило: expression = primary_expression.
    /// </summary>
    private Expression ParseExpression()
    {
        return ParsePrimaryExpression();
    }

    /// <summary>
    /// Разбирает первичные выражения (литералы).
    /// Правило: primary_expression = literal.
    /// </summary>
    private Expression ParsePrimaryExpression()
    {
        Token token = _tokens.Peek();

        switch (token.Type)
        {
            case TokenType.Integer:
                _tokens.Advance();
                LiteralExpression intExpr = new LiteralExpression(new Value(token.Value.ToInteger()));
                intExpr.ResultType = ValueType.Integer;
                return intExpr;

            case TokenType.Float:
                _tokens.Advance();
                LiteralExpression floatExpr = new LiteralExpression(new Value(token.Value.ToFloat()));
                floatExpr.ResultType = ValueType.Float;
                return floatExpr;

            case TokenType.StringLiteral:
                _tokens.Advance();
                LiteralExpression stringExpr = new LiteralExpression(new Value(token.Value.ToString()));
                stringExpr.ResultType = ValueType.String;
                return stringExpr;

            default:
                throw new UnexpectedLexemeException(token);
        }
    }

    /// <summary>
    /// Проверяет соответствие текущего токена ожидаемому типу и продвигает поток токенов.
    /// </summary>
    private Token Match(TokenType expected)
    {
        Token t = _tokens.Peek();
        if (t.Type != expected)
        {
            throw new UnexpectedLexemeException(expected, t);
        }

        _tokens.Advance();
        return t;
    }
}