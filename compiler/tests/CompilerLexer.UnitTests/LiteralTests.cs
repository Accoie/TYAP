namespace CompilerLexer.UnitTests;

public class LiteralTests
{
    [Theory]
    [MemberData(nameof(IntegerLiteralsData))]
    public void Can_tokenize_integer_literals(string code, List<Token> expected)
    {
        List<Token> actual = LexerTest.Tokenize(code);
        LexerTest.AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> IntegerLiteralsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "0 100 999 025",
                new List<Token>
                {
                    new(TokenType.Integer, new TokenValue(0)),
                    new(TokenType.Integer, new TokenValue(100)),
                    new(TokenType.Integer, new TokenValue(999)),
                    new(TokenType.Integer, new TokenValue(25)),
                }
            },
        };
    }

    [Theory]
    [MemberData(nameof(FloatLiteralsData))]
    public void Can_tokenize_float_literals(string code, List<Token> expected)
    {
        List<Token> actual = LexerTest.Tokenize(code);
        LexerTest.AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> FloatLiteralsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "3.14 0.5 02.0",
                new List<Token>
                {
                    new(TokenType.Float, new TokenValue(3.14m)),
                    new(TokenType.Float, new TokenValue(0.5m)),
                    new(TokenType.Float, new TokenValue(2.0m)),
                }
            },
            {
                "3.14.14 0.5.0 2.0.3",
                new List<Token>
                {
                    new(TokenType.Float, new TokenValue(3.14m)),
                    new(TokenType.Error, new TokenValue(".")),
                    new(TokenType.Integer, new TokenValue(14)),
                    new(TokenType.Float, new TokenValue(0.5m)),
                    new(TokenType.Error, new TokenValue(".")),
                    new(TokenType.Integer, new TokenValue(0)),
                    new(TokenType.Float, new TokenValue(2.0m)),
                    new(TokenType.Error, new TokenValue(".")),
                    new(TokenType.Integer, new TokenValue(3)),
                }
            },
        };
    }

    [Theory]
    [MemberData(nameof(StringLiteralsData))]
    public void Can_tokenize_string_literals(string code, List<Token> expected)
    {
        List<Token> actual = LexerTest.Tokenize(code);
        LexerTest.AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> StringLiteralsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "\"Count: \" \"Hello\" \"\"",
                new List<Token>
                {
                    new(TokenType.StringLiteral, new TokenValue("Count: ")),
                    new(TokenType.StringLiteral, new TokenValue("Hello")),
                    new(TokenType.StringLiteral, new TokenValue("")),
                }
            },
            {
                "\"path\\\\file\" \"line1\\nline2\" \"tab\\tchar\" \"quote\\\"end\" \"carriage\\r\"",
                new List<Token>
                {
                    new(TokenType.StringLiteral, new TokenValue("path\\file")),
                    new(TokenType.StringLiteral, new TokenValue("line1\nline2")),
                }
            }
        };
    }

    [Theory]
    [MemberData(nameof(BooleanLiteralsData))]
    public void Can_tokenize_boolean_literals(string code, List<Token> expected)
    {
        List<Token> actual = LexerTest.Tokenize(code);
        LexerTest.AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> BooleanLiteralsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "true false",
                new List<Token>
                {
                    new(TokenType.True),
                    new(TokenType.False),
                }
            },
        };
    }

    [Theory]
    [MemberData( nameof( ArrayLiteralsData ) )]
    public void Can_tokenize_array_literals( string code, List<Token> expected )
    {
        List<Token> actual = LexerTest.Tokenize( code );
        LexerTest.AssertTokensEqual( expected, actual );
    }

    public static TheoryData<string, List<Token>> ArrayLiteralsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "[]",
                new List<Token>
                {
                    new(TokenType.LBracket),
                    new(TokenType.RBracket),
                }
            },
        };
    }

    [Theory]
    [MemberData( nameof( StructureLiteralsData ) )]
    public void Can_tokenize_structure_literals( string code, List<Token> expected )
    {
        List<Token> actual = LexerTest.Tokenize( code );
        LexerTest.AssertTokensEqual( expected, actual );
    }

    public static TheoryData<string, List<Token>> StructureLiteralsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "{ x : 10, y : 20 }",
                new List<Token>
                {
                    new(TokenType.LBrace),
                    new(TokenType.Identifier, new TokenValue("x")),
                    new(TokenType.Colon),
                    new(TokenType.Integer, new TokenValue(10)),
                    new(TokenType.Comma),
                    new(TokenType.Identifier, new TokenValue("y")),
                    new(TokenType.Colon),
                    new(TokenType.Integer, new TokenValue(20)),
                    new(TokenType.RBrace),
                }
            },
        };
    }
}