namespace CompilerLexer;

public class Lexer
{
    private readonly IScanner _scanner;

    private static readonly IReadOnlyDictionary<string, TokenType> Keywords =
        new Dictionary<string, TokenType>()
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

    public Lexer(IScanner scanner)
    {
        this._scanner = scanner;
    }

    public Token ParseToken()
    {
        SkipWhiteSpacesAndComments();

        if (_scanner.IsEnd())
        {
            return new Token(TokenType.EndOfFile);
        }

        char currentChar = _scanner.Peek();

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
        char currentChar = _scanner.Peek();

        switch (currentChar)
        {
            case '{':
                _scanner.Advance();
                return new Token(TokenType.LBrace);
            case '}':
                _scanner.Advance();
                return new Token(TokenType.RBrace);
            case '(':
                _scanner.Advance();
                return new Token(TokenType.LParen);
            case ')':
                _scanner.Advance();
                return new Token(TokenType.RParen);
            case '[':
                _scanner.Advance();
                return new Token(TokenType.LBracket);
            case ']':
                _scanner.Advance();
                return new Token(TokenType.RBracket);

            case ';':
                _scanner.Advance();
                return new Token(TokenType.Semicolon);
            case ',':
                _scanner.Advance();
                return new Token(TokenType.Comma);
            case ':':
                _scanner.Advance();
                return new Token(TokenType.Colon);

            case '+':
                _scanner.Advance();
                return new Token(TokenType.PlusSign);
            case '-':
                _scanner.Advance();
                return new Token(TokenType.MinusSign);
            case '*':
                _scanner.Advance();
                return new Token(TokenType.MultiplySign);
            case '/':
                _scanner.Advance();
                return new Token(TokenType.DivideSign);
            case '%':
                _scanner.Advance();
                return new Token(TokenType.ModuloSign);
            case '^':
                _scanner.Advance();
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
                _scanner.Advance();
                return new Token(TokenType.LogicalAnd);
            case '|':
                return ParseOrOperator();

            case '.':
                _scanner.Advance();
                return new Token(TokenType.Dot);
        }

        _scanner.Advance();
        return new Token(TokenType.Error, new TokenValue(currentChar.ToString()));
    }

    private Token ParseAssignOrEqualsOperator()
    {
        _scanner.Advance();

        if (_scanner.Peek() == '=')
        {
            _scanner.Advance();
            return new Token(TokenType.Equal);
        }

        return new Token(TokenType.Assign);
    }

    private Token ParseNotEqualOrLogicalNotOperator()
    {
        _scanner.Advance();

        if (_scanner.Peek() == '=')
        {
            _scanner.Advance();
            return new Token(TokenType.NotEqual);
        }

        return new Token(TokenType.LogicalNot);
    }

    private Token ParseLessThanOperator()
    {
        _scanner.Advance();

        if (_scanner.Peek() == '=')
        {
            _scanner.Advance();
            return new Token(TokenType.LessThanOrEqual);
        }

        return new Token(TokenType.LessThan);
    }

    private Token ParseGreaterThanOperator()
    {
        _scanner.Advance();

        if (_scanner.Peek() == '=')
        {
            _scanner.Advance();
            return new Token(TokenType.GreaterThanOrEqual);
        }

        return new Token(TokenType.GreaterThan);
    }

    private Token ParseOrOperator()
    {
        _scanner.Advance();
        if (_scanner.Peek() == '|')
        {
            _scanner.Advance();
            return new Token(TokenType.LogicalOr);
        }

        return new Token(TokenType.Error, new TokenValue("|"));
    }

    private Token ParseIdentifierOrKeyword()
    {
        string value = "";
        bool hasInvalidChar = false;

        while (char.IsLetterOrDigit(_scanner.Peek()) || _scanner.Peek() == '_')
        {
            char c = _scanner.Peek();

            if (!char.IsAsciiLetterOrDigit(c) && c != '_')
            {
                hasInvalidChar = true;
            }

            value += c;
            _scanner.Advance();
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
        int value = GetDigitValue(_scanner.Peek());
        _scanner.Advance();

        while (char.IsAsciiDigit(_scanner.Peek()))
        {
            value = value * 10 + GetDigitValue(_scanner.Peek());
            _scanner.Advance();
        }

        if (_scanner.Peek() == '.')
        {
            _scanner.Advance();
            return ParseFloatLiteral(value);
        }

        return new Token(TokenType.Integer, new TokenValue(value));
    }

    private Token ParseFloatLiteral(double integerPart)
    {
        double value = integerPart;
        double factor = 0.1d;

        while (char.IsAsciiDigit(_scanner.Peek()))
        {
            value += factor * GetDigitValue(_scanner.Peek());
            factor *= 0.1d;
            _scanner.Advance();
        }

        return new Token(TokenType.Float, new TokenValue(value));
    }

    private static int GetDigitValue(char c) => c - '0';

    private Token ParseStringLiteral()
    {
        _scanner.Advance();

        string contents = "";
        while (_scanner.Peek() != '"' && !_scanner.IsEnd())
        {
            if (TryParseEscapeSequence(out char unescaped))
            {
                contents += unescaped;
            }
            else
            {
                contents += _scanner.Peek();
                _scanner.Advance();
            }
        }

        if (_scanner.Peek() == '"')
        {
            _scanner.Advance();
            return new Token(TokenType.StringLiteral, new TokenValue(contents));
        }

        return new Token(TokenType.Error, new TokenValue(contents));
    }

    private bool TryParseEscapeSequence(out char unescaped)
    {
        if (_scanner.Peek() != '\\')
        {
            unescaped = '\0';
            return false;
        }

        _scanner.Advance();

        unescaped = _scanner.Peek() switch
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
            _scanner.Advance();
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
        }
        while (skipped);
    }

    private void SkipWhiteSpaces()
    {
        while (char.IsWhiteSpace(_scanner.Peek()))
        {
            _scanner.Advance();
        }
    }

    private bool TryParseMultilineComment()
    {
        if (_scanner.Peek() != '/' || _scanner.Peek(1) != '#')
        {
            return false;
        }

        while (!_scanner.IsEnd() && !(_scanner.Peek() == '#' && _scanner.Peek(1) == '/'))
        {
            _scanner.Advance();
        }

        if (_scanner.Peek() == '#' && _scanner.Peek(1) == '/')
        {
            _scanner.Advance();
            _scanner.Advance();
        }

        return true;
    }

    private bool TryParseSingleLineComment()
    {
        if (_scanner.Peek() != '#' || _scanner.Peek(1) != '#')
        {
            return false;
        }

        while (!_scanner.IsEnd() && _scanner.Peek() != '\n' && _scanner.Peek() != '\r')
        {
            _scanner.Advance();
        }

        return true;
    }
}