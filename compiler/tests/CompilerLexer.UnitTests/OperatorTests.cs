namespace CompilerLexer.UnitTests;

public class OperatorTests
{
    [Theory]
    [MemberData(nameof(ArithmeticOperatorsData))]
    public void Can_tokenize_arithmetic_operators(string code, List<Token> expected)
    {
        List<Token> actual = LexerTest.Tokenize(code);
        LexerTest.AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> ArithmeticOperatorsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "+ - * / % = ^",
                new List<Token>
                {
                    new(TokenType.PlusSign),
                    new(TokenType.MinusSign),
                    new(TokenType.MultiplySign),
                    new(TokenType.DivideSign),
                    new(TokenType.ModuloSign),
                    new(TokenType.Assign),
                    new(TokenType.ExponentiationSign)
                }
            },
        };
    }

    [Theory]
    [MemberData(nameof(LogicalOperatorsData))]
    public void Can_tokenize_logical_operators(string code, List<Token> expected)
    {
        List<Token> actual = LexerTest.Tokenize(code);
        LexerTest.AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> LogicalOperatorsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "|| @ !",
                new List<Token>
                {
                    new(TokenType.LogicalOr),
                    new(TokenType.LogicalAnd),
                    new(TokenType.LogicalNot),
                }
            },
        };
    }

    [Theory]
    [MemberData(nameof(ComparisonOperatorsData))]
    public void Can_tokenize_comparison_operators(string code, List<Token> expected)
    {
        List<Token> actual = LexerTest.Tokenize(code);
        LexerTest.AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> ComparisonOperatorsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "== != < > <= >=",
                new List<Token>
                {
                    new(TokenType.Equal),
                    new(TokenType.NotEqual),
                    new(TokenType.LessThan),
                    new(TokenType.GreaterThan),
                    new(TokenType.LessThanOrEqual),
                    new(TokenType.GreaterThanOrEqual),
                }
            },
        };
    }

    [Theory]
    [MemberData( nameof( DelimetersData ) )]
    public void Can_tokenize_delimeters( string code, List<Token> expected )
    {
        List<Token> actual = LexerTest.Tokenize( code );
        LexerTest.AssertTokensEqual( expected, actual );
    }

    public static TheoryData<string, List<Token>> DelimetersData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "(true) myObject.field myArray[0]",
                new List<Token>
                {
                    new(TokenType.LParen),
                    new(TokenType.True),
                    new(TokenType.RParen),
                    new(TokenType.Identifier, new TokenValue("myObject")),
                    new(TokenType.Dot),
                    new(TokenType.Identifier, new TokenValue("field")),
                    new(TokenType.Identifier, new TokenValue("myArray")),
                    new(TokenType.LBracket),
                    new(TokenType.Integer, new TokenValue(0)),
                    new(TokenType.RBracket),
                }
            },
        };
    }
}