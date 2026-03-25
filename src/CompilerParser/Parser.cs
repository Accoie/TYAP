using Ast.Expressions;
using Ast.Statements;
using CompilerLexer;

using Runtime;
using ValueType = Runtime.ValueType;

namespace CompilerParser;

public class Parser
{
    private readonly TokenStream tokens;

    private readonly Stack<ValueType> returnTypes = new();

    public Parser( string code )
    {
        tokens = new TokenStream( code );
    }

    /// <summary>
    /// Выполняет разбор выражения RusMatushka
    /// Правило: program = "begin", { statement }, "end".
    /// </summary>
    public BlockStatement ParseProgram()
    {
        return ParseBlock( true );
    }

    /// <summary>
    /// Разбирает блок кода.
    /// Правило: block = "begin", { statement }, "end".
    /// </summary>
    private BlockStatement ParseBlock( bool isNew )
    {
        Match( TokenType.Begin );

        List<Statement> statements = [];
        while ( tokens.Peek().Type != TokenType.End && tokens.Peek().Type != TokenType.EndOfFile )
        {
            Statement node = ParseStatement();
            statements.Add( node );
        }

        Match( TokenType.End );

        return new BlockStatement( statements, isNew );
    }

    private Statement ParseStatement()
    {
        TokenType token = tokens.Peek().Type;

        return token switch
        {
            TokenType.Begin => ParseBlock( true ),
            TokenType.Output => ParseOutput(),
            _ => throw new UnexpectedLexemeException( tokens.Peek() )
        };
    }

    /// <summary>
    /// Разбирает оператор вывода.
    /// Правило: output_statement = "write", "(", argument_list, ")", ";".
    /// </summary>
    private OutputStatement ParseOutput()
    {
        Match( TokenType.Output );
        Match( TokenType.LParen );

        List<Expression> arguments = [ ParseExpression() ];

        while ( tokens.Peek().Type == TokenType.Comma )
        {
            tokens.Advance();
            arguments.Add( ParseExpression() );
        }

        Match( TokenType.RParen );
        Match( TokenType.Semicolon );
        return new OutputStatement( arguments );
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
    /// Разбирает первичные выражения (литералы, идентификаторы, вызовы функций, выражения в скобках).
    /// Правило: primary_expression = literal.
    /// </summary>
    private Expression ParsePrimaryExpression()
    {
        Token token = tokens.Peek();

        switch ( token.Type )
        {
            case TokenType.Integer:
                tokens.Advance();
                var intExpr = new LiteralExpression( new Value( token.Value!.ToInteger() ) );
                intExpr.ResultType = ValueType.Integer;
                return intExpr;

            case TokenType.Double:
                tokens.Advance();
                var floatExpr = new LiteralExpression( new Value( token.Value!.ToDouble() ) );
                floatExpr.ResultType = ValueType.Float;
                return floatExpr;

            case TokenType.StringLiteral:
                tokens.Advance();
                var stringExpr = new LiteralExpression( new Value( token.Value!.ToString() ) );
                stringExpr.ResultType = ValueType.String; 
                return stringExpr;

            default:
                throw new UnexpectedLexemeException( token );
        }
    }

    /// <summary>
    /// Проверяет соответствие текущего токена ожидаемому типу и продвигает поток токенов.
    /// </summary>
    private Token Match( TokenType expected )
    {
        Token t = tokens.Peek();
        if ( t.Type != expected )
        {
            throw new UnexpectedLexemeException( expected, t );
        }

        tokens.Advance();
        return t;
    }
}
