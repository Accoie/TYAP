namespace CompilerLexer.UnitTests;

public class KeywordTests
{
    [Theory]
    [MemberData(nameof(KeywordCasesData))]
    public void Can_tokenize_keywords(string code, List<Token> expected)
    {
        List<Token> actual = LexerTest.Tokenize(code);
        LexerTest.AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> KeywordCasesData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "begin end var if then else for while do break continue function return write read struct arr",
                new List<Token>
                {
                    new(TokenType.Begin),
                    new(TokenType.End),
                    new(TokenType.Var),
                    new(TokenType.If),
                    new(TokenType.Then),
                    new(TokenType.Else),
                    new(TokenType.For),
                    new(TokenType.While),
                    new(TokenType.Do),
                    new(TokenType.Break),
                    new(TokenType.Continue),
                    new(TokenType.Function),
                    new(TokenType.Return),
                    new(TokenType.Output),
                    new(TokenType.Input),
                    new(TokenType.Structure),
                    new(TokenType.Array),
                }
            }
        };
    }

    [Theory]
    [MemberData(nameof( IdentifierData ) )]
    public void Can_tokenize_identifier( string code, List<Token> expected)
    {
        List<Token> actual = LexerTest.Tokenize(code);
        LexerTest.AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> IdentifierData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "variable _value item1",
                new List<Token>
                {
                    new(TokenType.Identifier, new TokenValue("variable")),
                    new(TokenType.Identifier, new TokenValue("_value")),
                    new(TokenType.Identifier, new TokenValue("item1")),
                }
            }
        };
    }
}