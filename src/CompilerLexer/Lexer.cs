namespace CompilerLexer;

public class Lexer
{
    private static readonly IReadOnlyDictionary<string, TokenType> Keywords =
        new Dictionary<string, TokenType>
        {
            { "begin", TokenType.Begin },
            { "end", TokenType.End },
            { "var", TokenType.Var },
            { "integer", TokenType.IntegerType },
            { "float", TokenType.FloatType },
            { "string", TokenType.StringType },
            { "if", TokenType.If },
            { "then", TokenType.Then },
            { "else", TokenType.Else },
            { "for", TokenType.For },
            { "from", TokenType.From },
            { "to", TokenType.To },
            { "do", TokenType.Do },
            { "while", TokenType.While },
            { "write", TokenType.Output },
            { "read", TokenType.Input },
            { "break", TokenType.Break },
            { "continue", TokenType.Continue },
            { "return", TokenType.Return },
            { "function", TokenType.Function },
            { "arr", TokenType.Array },
            { "of", TokenType.Of },
            { "struct", TokenType.Structure },
            { "abs", TokenType.Abs },
            { "min", TokenType.Min },
            { "max", TokenType.Max },
            { "round", TokenType.Round },
            { "len", TokenType.Len },
            { "getsymbol", TokenType.GetSymbol },
            { "tostring", TokenType.ToString },
        };

    private readonly IScanner scanner;

    public Lexer(IScanner scanner)
    {
        this.scanner = scanner;
    }

    public Token ParseToken()
    {
        SkipWhiteSpacesAndComments();

        if (scanner.IsEnd())
        {
            return new Token(TokenType.EndOfFile);
        }

        char currentChar = scanner.Peek();

        if (char.IsAsciiLetter(currentChar) || currentChar == '_')
        {
            return ParseIdentifierOrKeyword();
        }

        if (char.IsAsciiDigit(currentChar))
        {
            return ParseNumericLiteral();
        }

        if (currentChar == '"')
        {
            return ParseStringLiteral();
        }

        return ParseOperatorOrDelimiter();
    }

    private Token ParseOperatorOrDelimiter()
    {
        char currentChar = scanner.Peek();

        switch (currentChar)
        {
            case '{':
                scanner.Advance();
                return new Token(TokenType.LBrace);
            case '}':
                scanner.Advance();
                return new Token(TokenType.RBrace);
            case '(':
                scanner.Advance();
                return new Token(TokenType.LParen);
            case ')':
                scanner.Advance();
                return new Token(TokenType.RParen);
            case '[':
                scanner.Advance();
                return new Token(TokenType.LBracket);
            case ']':
                scanner.Advance();
                return new Token(TokenType.RBracket);

            case ';':
                scanner.Advance();
                return new Token(TokenType.Semicolon);
            case ',':
                scanner.Advance();
                return new Token(TokenType.Comma);
            case ':':
                scanner.Advance();
                return new Token(TokenType.Colon);

            case '+':
                scanner.Advance();
                return new Token(TokenType.PlusSign);
            case '-':
                scanner.Advance();
                return new Token(TokenType.MinusSign);
            case '*':
                scanner.Advance();
                return new Token(TokenType.MultiplySign);
            case '/':
                scanner.Advance();
                return new Token(TokenType.DivideSign);
            case '%':
                scanner.Advance();
                return new Token(TokenType.ModuloSign);
            case '^':
                scanner.Advance();
                return new Token(TokenType.ExponentiationSign);
            case '=':
                return ParseAssignOrEqualsOperator();
            case '!':
                return ParseNotEqualOrLogicalNotOperator();
            case '<':
                return ParseLessThanOperator();
            case '>':
                return ParseGreaterThanOperator();

            case '@':
                scanner.Advance();
                return new Token(TokenType.LogicalAnd);
            case '|':
                return ParseOrOperator();

            case '.':
                scanner.Advance();
                return new Token(TokenType.Dot);
        }

        scanner.Advance();
        return new Token(TokenType.Error, new TokenValue(currentChar.ToString()));
    }

    private Token ParseAssignOrEqualsOperator()
    {
        scanner.Advance();

        if (scanner.Peek() == '=')
        {
            scanner.Advance();
            return new Token(TokenType.Equal);
        }

        return new Token(TokenType.Assign);
    }

    private Token ParseNotEqualOrLogicalNotOperator()
    {
        scanner.Advance();

        if (scanner.Peek() == '=')
        {
            scanner.Advance();
            return new Token(TokenType.NotEqual);
        }

        return new Token(TokenType.LogicalNot);
    }

    private Token ParseLessThanOperator()
    {
        scanner.Advance();

        if (scanner.Peek() == '=')
        {
            scanner.Advance();
            return new Token(TokenType.LessThanOrEqual);
        }

        return new Token(TokenType.LessThan);
    }

    private Token ParseGreaterThanOperator()
    {
        scanner.Advance();

        if (scanner.Peek() == '=')
        {
            scanner.Advance();
            return new Token(TokenType.GreaterThanOrEqual);
        }

        return new Token(TokenType.GreaterThan);
    }

    private Token ParseOrOperator()
    {
        scanner.Advance();
        if (scanner.Peek() == '|')
        {
            scanner.Advance();
            return new Token(TokenType.LogicalOr);
        }

        return new Token(TokenType.Error, new TokenValue("|"));
    }

    private Token ParseIdentifierOrKeyword()
    {
        string value = "";
        bool hasInvalidChar = false;

        while (char.IsLetterOrDigit(scanner.Peek()) || scanner.Peek() == '_')
        {
            char c = scanner.Peek();

            if (!char.IsAsciiLetterOrDigit(c) && c != '_')
            {
                hasInvalidChar = true;
            }

            value += c;
            scanner.Advance();
        }

        if (hasInvalidChar)
        {
            return new Token(TokenType.Error, new TokenValue(value));
        }

        if (Keywords.TryGetValue(value, out TokenType type))
        {
            return new Token(type);
        }

        return new Token(TokenType.Identifier, new TokenValue(value));
    }

    private Token ParseNumericLiteral()
    {
        decimal value = GetDigitValue(scanner.Peek());
        scanner.Advance();

        while (char.IsAsciiDigit(scanner.Peek()))
        {
            value = (value * 10) + GetDigitValue(scanner.Peek());
            scanner.Advance();
        }

        if (scanner.Peek() == '.')
        {
            scanner.Advance();
            return ParseFloatLiteral(value);
        }

        return new Token(TokenType.Integer, new TokenValue(value));
    }

    private Token ParseFloatLiteral(decimal integerPart)
    {
        decimal value = integerPart;
        decimal factor = 0.1m;

        while (char.IsAsciiDigit(scanner.Peek()))
        {
            value += factor * GetDigitValue(scanner.Peek());
            factor *= 0.1m;
            scanner.Advance();
        }

        return new Token(TokenType.Float, new TokenValue(value));
    }

    private static int GetDigitValue(char c)
    {
        return c - '0';
    }

    private Token ParseStringLiteral()
    {
        scanner.Advance();

        string contents = "";
        while (scanner.Peek() != '"' && !scanner.IsEnd())
        {
            if (TryParseEscapeSequence(out char unescaped))
            {
                contents += unescaped;
            }
            else
            {
                contents += scanner.Peek();
                scanner.Advance();
            }
        }

        if (scanner.Peek() == '"')
        {
            scanner.Advance();
            return new Token(TokenType.StringLiteral, new TokenValue(contents));
        }

        return new Token(TokenType.Error, new TokenValue(contents));
    }

    private bool TryParseEscapeSequence(out char unescaped)
    {
        if (scanner.Peek() != '\\')
        {
            unescaped = '\0';
            return false;
        }

        scanner.Advance();

        unescaped = scanner.Peek() switch
        {
            '\\' => '\\',
            'n' => '\n',
            't' => '\t',
            'r' => '\r',
            '\'' => '\'',
            '\"' => '\"',
            _ => '\0',
        };

        if (unescaped != '\0')
        {
            scanner.Advance();
            return true;
        }

        return false;
    }

    private void SkipWhiteSpacesAndComments()
    {
        bool skipped;
        do
        {
            SkipWhiteSpaces();
            skipped = TryParseMultilineComment() || TryParseSingleLineComment();
        } while (skipped);
    }

    private void SkipWhiteSpaces()
    {
        while (char.IsWhiteSpace(scanner.Peek()))
        {
            scanner.Advance();
        }
    }

    private bool TryParseMultilineComment()
    {
        if (scanner.Peek() != '/' || scanner.Peek(1) != '#')
        {
            return false;
        }

        while (!scanner.IsEnd() && !(scanner.Peek() == '#' && scanner.Peek(1) == '/'))
        {
            scanner.Advance();
        }

        if (scanner.Peek() == '#' && scanner.Peek(1) == '/')
        {
            scanner.Advance();
            scanner.Advance();
        }

        return true;
    }

    private bool TryParseSingleLineComment()
    {
        if (scanner.Peek() != '#' || scanner.Peek(1) != '#')
        {
            return false;
        }

        while (!scanner.IsEnd() && scanner.Peek() != '\n' && scanner.Peek() != '\r')
        {
            scanner.Advance();
        }

        return true;
    }
}