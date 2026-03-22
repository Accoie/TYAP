namespace CompilerLexer.UnitTests;

public class LexerTest
{
    [Theory]
    [MemberData(nameof(AllTestData))]
    public void Can_tokenize_all_lexemes(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);
        AssertTokensEqual(expected, actual);
    }

    public static TheoryData<string, List<Token>> AllTestData()
    {
        return new TheoryData<string, List<Token>>
        {
            // Разбор идентификаторов
            {
                "variable _value item1", [
                    new Token(TokenType.Identifier, new TokenValue("variable")),
                    new Token(TokenType.Identifier, new TokenValue("_value")),
                    new Token(TokenType.Identifier, new TokenValue("item1"))
                ]
            },

            // Объявление переменных
            {
                "var pi : float = 3.14; var name : string = \"Vladimir\";", [
                    new Token(TokenType.Var),
                    new Token(TokenType.Identifier, new TokenValue("pi")),
                    new Token(TokenType.Colon),
                    new Token(TokenType.FloatType),
                    new Token(TokenType.Assign),
                    new Token(TokenType.Float, new TokenValue(3.14m)),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.Var),
                    new Token(TokenType.Identifier, new TokenValue("name")),
                    new Token(TokenType.Colon),
                    new Token(TokenType.StringType),
                    new Token(TokenType.Assign),
                    new Token(TokenType.StringLiteral, new TokenValue("Vladimir")),
                    new Token(TokenType.Semicolon)
                ]
            },

            // Условный оператор и else
            {
                "if (age >= 18) then begin write(\"Hello\"); end else begin write(\"Hi\"); end", [
                    new Token(TokenType.If),
                    new Token(TokenType.LParen),
                    new Token(TokenType.Identifier, new TokenValue("age")),
                    new Token(TokenType.GreaterThanOrEqual),
                    new Token(TokenType.Integer, new TokenValue(18)),
                    new Token(TokenType.RParen),
                    new Token(TokenType.Then),
                    new Token(TokenType.Begin),
                    new Token(TokenType.Output),
                    new Token(TokenType.LParen),
                    new Token(TokenType.StringLiteral, new TokenValue("Hello")),
                    new Token(TokenType.RParen),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.End),
                    new Token(TokenType.Else),
                    new Token(TokenType.Begin),
                    new Token(TokenType.Output),
                    new Token(TokenType.LParen),
                    new Token(TokenType.StringLiteral, new TokenValue("Hi")),
                    new Token(TokenType.RParen),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.End)
                ]
            },

            // Цикл FOR с break
            {
                "for i from 1 to 10 do begin break; end", [
                    new Token(TokenType.For),
                    new Token(TokenType.Identifier, new TokenValue("i")),
                    new Token(TokenType.From),
                    new Token(TokenType.Integer, new TokenValue(1)),
                    new Token(TokenType.To),
                    new Token(TokenType.Integer, new TokenValue(10)),
                    new Token(TokenType.Do),
                    new Token(TokenType.Begin),
                    new Token(TokenType.Break),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.End)
                ]
            },

            // Цикл WHILE с continue
            {
                "while (1) do begin continue; end", [
                    new Token(TokenType.While),
                    new Token(TokenType.LParen),
                    new Token(TokenType.Integer, new TokenValue(1)),
                    new Token(TokenType.RParen),
                    new Token(TokenType.Do),
                    new Token(TokenType.Begin),
                    new Token(TokenType.Continue),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.End)
                ]
            },

            // Объявление функции с return
            {
                "function add(a : integer, b : integer) : integer begin return a + b; end", [
                    new Token(TokenType.Function),
                    new Token(TokenType.Identifier, new TokenValue("add")),
                    new Token(TokenType.LParen),
                    new Token(TokenType.Identifier, new TokenValue("a")),
                    new Token(TokenType.Colon),
                    new Token(TokenType.IntegerType),
                    new Token(TokenType.Comma),
                    new Token(TokenType.Identifier, new TokenValue("b")),
                    new Token(TokenType.Colon),
                    new Token(TokenType.IntegerType),
                    new Token(TokenType.RParen),
                    new Token(TokenType.Colon),
                    new Token(TokenType.IntegerType),
                    new Token(TokenType.Begin),
                    new Token(TokenType.Return),
                    new Token(TokenType.Identifier, new TokenValue("a")),
                    new Token(TokenType.PlusSign),
                    new Token(TokenType.Identifier, new TokenValue("b")),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.End)
                ]
            },

            // Ввод-вывод
            {
                "write(\"Hello\"); read(input);", [
                    new Token(TokenType.Output),
                    new Token(TokenType.LParen),
                    new Token(TokenType.StringLiteral, new TokenValue("Hello")),
                    new Token(TokenType.RParen),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.Input),
                    new Token(TokenType.LParen),
                    new Token(TokenType.Identifier, new TokenValue("input")),
                    new Token(TokenType.RParen),
                    new Token(TokenType.Semicolon)
                ]
            },

            // Объявления массива
            {
                "var numbers : arr[10] of integer;", [
                    new Token(TokenType.Var),
                    new Token(TokenType.Identifier, new TokenValue("numbers")),
                    new Token(TokenType.Colon),
                    new Token(TokenType.Array),
                    new Token(TokenType.LBracket),
                    new Token(TokenType.Integer, new TokenValue(10)),
                    new Token(TokenType.RBracket),
                    new Token(TokenType.Of),
                    new Token(TokenType.IntegerType),
                    new Token(TokenType.Semicolon)
                ]
            },

            // Объявление структуры
            {
                "var point : struct { x : float, y : float };", [
                    new Token(TokenType.Var),
                    new Token(TokenType.Identifier, new TokenValue("point")),
                    new Token(TokenType.Colon),
                    new Token(TokenType.Structure),
                    new Token(TokenType.LBrace),
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.Colon),
                    new Token(TokenType.FloatType),
                    new Token(TokenType.Comma),
                    new Token(TokenType.Identifier, new TokenValue("y")),
                    new Token(TokenType.Colon),
                    new Token(TokenType.FloatType),
                    new Token(TokenType.RBrace),
                    new Token(TokenType.Semicolon)
                ]
            },

            // Разбор литералов целых чисел
            {
                "0 999 025", [
                    new Token(TokenType.Integer, new TokenValue(0)),
                    new Token(TokenType.Integer, new TokenValue(999)),
                    new Token(TokenType.Integer, new TokenValue(25))
                ]
            },

            // Разбор вещественных литералов
            {
                "3.14 02.0", [
                    new Token(TokenType.Float, new TokenValue(3.14m)),
                    new Token(TokenType.Float, new TokenValue(2.0m))
                ]
            },

            // Разбор строк
            {
                "\"Count: \" \"\"", [
                    new Token(TokenType.StringLiteral, new TokenValue("Count: ")),
                    new Token(TokenType.StringLiteral, new TokenValue(""))
                ]
            },

            // Разбор escape-последовательностей
            {
                "\"path\\\\file\" \"line1\\nline2\" \"tab\\tchar\" \"quote\\\"end\" \"carriage\\r\"", [
                    new Token(TokenType.StringLiteral, new TokenValue("path\\file")),
                    new Token(TokenType.StringLiteral, new TokenValue("line1\nline2")),
                    new Token(TokenType.StringLiteral, new TokenValue("tab\tchar")),
                    new Token(TokenType.StringLiteral, new TokenValue("quote\"end")),
                    new Token(TokenType.StringLiteral, new TokenValue("carriage\r"))
                ]
            },

            // Арифметические операторы
            {
                "a + b - c * d / e % f ^ g", [
                    new Token(TokenType.Identifier, new TokenValue("a")),
                    new Token(TokenType.PlusSign),
                    new Token(TokenType.Identifier, new TokenValue("b")),
                    new Token(TokenType.MinusSign),
                    new Token(TokenType.Identifier, new TokenValue("c")),
                    new Token(TokenType.MultiplySign),
                    new Token(TokenType.Identifier, new TokenValue("d")),
                    new Token(TokenType.DivideSign),
                    new Token(TokenType.Identifier, new TokenValue("e")),
                    new Token(TokenType.ModuloSign),
                    new Token(TokenType.Identifier, new TokenValue("f")),
                    new Token(TokenType.ExponentiationSign),
                    new Token(TokenType.Identifier, new TokenValue("g"))
                ]
            },

            // Логические операторы
            {
                "x || y @ z !flag", [
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.LogicalOr),
                    new Token(TokenType.Identifier, new TokenValue("y")),
                    new Token(TokenType.LogicalAnd),
                    new Token(TokenType.Identifier, new TokenValue("z")),
                    new Token(TokenType.LogicalNot),
                    new Token(TokenType.Identifier, new TokenValue("flag"))
                ]
            },

            // Операторы сравнения
            {
                "a == b != c < d > e <= f >= g", [
                    new Token(TokenType.Identifier, new TokenValue("a")),
                    new Token(TokenType.Equal),
                    new Token(TokenType.Identifier, new TokenValue("b")),
                    new Token(TokenType.NotEqual),
                    new Token(TokenType.Identifier, new TokenValue("c")),
                    new Token(TokenType.LessThan),
                    new Token(TokenType.Identifier, new TokenValue("d")),
                    new Token(TokenType.GreaterThan),
                    new Token(TokenType.Identifier, new TokenValue("e")),
                    new Token(TokenType.LessThanOrEqual),
                    new Token(TokenType.Identifier, new TokenValue("f")),
                    new Token(TokenType.GreaterThanOrEqual),
                    new Token(TokenType.Identifier, new TokenValue("g"))
                ]
            },

            // Оператор присваивания
            {
                "x = y", [
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.Assign),
                    new Token(TokenType.Identifier, new TokenValue("y"))
                ]
            },

            // Выражение в скобках
            {
                "(0)", [
                    new Token(TokenType.LParen),
                    new Token(TokenType.Integer, new TokenValue(0)),
                    new Token(TokenType.RParen)
                ]
            },

            // Доступ к полям структуры
            {
                "myObject.field", [
                    new Token(TokenType.Identifier, new TokenValue("myObject")),
                    new Token(TokenType.Dot),
                    new Token(TokenType.Identifier, new TokenValue("field"))
                ]
            },

            // Индексация массива
            {
                "myArray[0]", [
                    new Token(TokenType.Identifier, new TokenValue("myArray")),
                    new Token(TokenType.LBracket),
                    new Token(TokenType.Integer, new TokenValue(0)),
                    new Token(TokenType.RBracket)
                ]
            },

            // Литералы структур
            {
                "{ x : 10, y : 20 };", [
                    new Token(TokenType.LBrace),
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.Colon),
                    new Token(TokenType.Integer, new TokenValue(10)),
                    new Token(TokenType.Comma),
                    new Token(TokenType.Identifier, new TokenValue("y")),
                    new Token(TokenType.Colon),
                    new Token(TokenType.Integer, new TokenValue(20)),
                    new Token(TokenType.RBrace),
                    new Token(TokenType.Semicolon)
                ]
            },

            // Пропуск пробельных символов
            { "   \t\r\n\f", [] },

            // Однострочные комментарии
            { "## это комментарий", [] },

            // Многострочные комментарии
            { "/# комментарий на несколько строк #/", [] },

            // Идентификатор с цифрой в начале
            {
                "1fdas", [
                    new Token(TokenType.Integer, new TokenValue(1)),
                    new Token(TokenType.Identifier, new TokenValue("fdas"))
                ]
            },

            // Идентификатор и символы из кириллицы
            {
                "sdкоммент", [
                    new Token(TokenType.Error, new TokenValue("sdкоммент"))
                ]
            },
        };
    }

    public static List<Token> Tokenize(string code)
    {
        TextScanner scanner = new(code);
        Lexer lexer = new(scanner);
        List<Token> tokens = [];

        Token token = lexer.ParseToken();
        while (token.Type != TokenType.EndOfFile)
        {
            tokens.Add(token);
            token = lexer.ParseToken();
        }

        return tokens;
    }

    private static void AssertTokensEqual(List<Token> expected, List<Token> actual)
    {
        Assert.Equal(expected.Count, actual.Count);

        for (int i = 0; i < expected.Count; i++)
        {
            Token expectedToken = expected[i];
            Token actualToken = actual[i];

            Assert.Equal(expectedToken.Type, actualToken.Type);
            Assert.Equal(expectedToken.Value, actualToken.Value);
        }
    }
}