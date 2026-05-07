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
    /// Выполняет разбор выражения Pascal++
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
    /// Правило: statement = variable_declaration
    ///                     | function_declaration
    ///                     | assignment_statement
    ///                     | input_statement
    ///                     | output_statement
    ///                     | if_statement
    ///                     | for_statement
    ///                     | while_statement
    ///                     | break_statement
    ///                     | continue_statement
    ///                     | return_statement
    ///                     | expression_statement.
    /// </summary>
    private Statement ParseStatement()
    {
        TokenType token = _tokens.Peek().Type;

        return token switch
        {
            TokenType.Identifier => ParseAssignmentOrFunctionCall(),
            TokenType.Var => ParseVariableDeclaration(),
            TokenType.Begin => ParseBlock(true),
            TokenType.Input => ParseInput(),
            TokenType.Output => ParseOutput(),
            TokenType.Function => ParseFunctionDeclaration(),
            TokenType.If => ParseIfStatement(),
            TokenType.Return => ParseReturnStatement(),
            TokenType.While => ParseWhileLoopStatement(),
            TokenType.For => ParseForLoopStatement(),
            TokenType.Break => ParseBreakStatement(),
            TokenType.Continue => ParseContinueStatement(),

            _ => throw new UnexpectedLexemeException(_tokens.Peek()),
        };
    }

    /// <summary>
    /// Разбирает оператор присваивания.
    /// Правило: assignment_statement = identifier, "=", expression, ";".
    /// </summary>
    private Statement ParseAssignmentOrFunctionCall()
    {
        string name = Match(TokenType.Identifier).Value.ToString();
        Statement result;
        if (_tokens.Peek().Type == TokenType.Assign)
        {
            Match(TokenType.Assign);
            Expression value = ParseExpression();
            result = new AssignmentStatement(name, value);
        }
        else
        {
            result = ParseFunctionCallStatement(name);
        }

        Match(TokenType.Semicolon);

        return result;
    }

    /// <summary>
    /// Разбирает оператор объявления переменной.
    /// Правило: variable_declaration = "var", identifier, ":", type, [ "=", expression ], ";" ;
    /// </summary>
    private VariableDeclarationStatement ParseVariableDeclaration()
    {
        Match(TokenType.Var);
        string name = Match(TokenType.Identifier).Value.ToString();
        Match(TokenType.Colon);

        ValueType type = _tokens.Peek().Type switch
        {
            TokenType.IntegerType => ValueType.Integer,
            TokenType.FloatType => ValueType.Float,
            TokenType.StringType => ValueType.String,
            _ => throw new UnexpectedLexemeException(_tokens.Peek()),
        };
        _tokens.Advance();

        Expression? initialValue = null;
        if (_tokens.Peek().Type == TokenType.Assign)
        {
            _tokens.Advance();
            initialValue = ParseExpression();
        }

        Match(TokenType.Semicolon);

        return new VariableDeclarationStatement(name, type, initialValue);
    }

    /// <summary>
    /// Разбирает оператор ввода.
    /// Правило: input_statement = "read", "(", identifier, ")", ";".
    /// </summary>
    private InputStatement ParseInput()
    {
        Match(TokenType.Input);
        Match(TokenType.LParen);

        string variableName = Match(TokenType.Identifier).Value.ToString();

        Match(TokenType.RParen);
        Match(TokenType.Semicolon);
        return new InputStatement(variableName);
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
    /// Разбирает объявление функции.
    /// Правило: function_declaration = "function", function_name, "(", [ parameter_list ], ")", [ ":", type ], block.
    /// </summary>
    private FunctionDeclarationStatement ParseFunctionDeclaration()
    {
        Match(TokenType.Function);
        string name = Match(TokenType.Identifier).Value.ToString();

        Match(TokenType.LParen);
        List<ParameterDeclarationStatement> parameters = ParseParameterList();

        Match(TokenType.RParen);

        TokenType returnTypetokens = _tokens.Peek().Type;
        ValueType resultType;
        if (returnTypetokens == TokenType.Colon)
        {
            Match(TokenType.Colon);
            resultType = ParseType();
        }
        else
        {
            resultType = ValueType.Void;
        }

        _returnTypes.Push(resultType);

        BlockStatement body = ParseBlock(false);

        _returnTypes.Pop();

        return new FunctionDeclarationStatement(name, parameters, body, resultType);
    }

    /// <summary>
    /// Разбирает вызов функции.
    /// Правило: function_call = function_name, "(", [ argument_list ], ")".
    /// </summary>
    private Expression ParseFunctionCall(string name)
    {
        Match(TokenType.LParen);

        List<Expression> arguments = new();
        if (_tokens.Peek().Type != TokenType.RParen)
        {
            arguments.Add(ParseExpression());
            while (_tokens.Peek().Type == TokenType.Comma)
            {
                _tokens.Advance();
                arguments.Add(ParseExpression());
            }
        }

        Match(TokenType.RParen);

        return new FunctionCallExpression(name, arguments);
    }

    /// <summary>
    /// Разбирает вызов функции.
    /// Правило: function_call = function_name, "(", [ argument_list ], ")".
    /// </summary>
    private Statement ParseFunctionCallStatement(string name)
    {
        Match(TokenType.LParen);

        List<Expression> arguments = new();
        if (_tokens.Peek().Type != TokenType.RParen)
        {
            arguments.Add(ParseExpression());
            while (_tokens.Peek().Type == TokenType.Comma)
            {
                _tokens.Advance();
                arguments.Add(ParseExpression());
            }
        }

        Match(TokenType.RParen);

        return new FunctionCallStatement(new FunctionCallExpression(name, arguments));
    }

    /// <summary>
    /// Разбирает условный оператор if.
    /// Правило: if_statement = "if", "(", expression, ")", "then", statement, [ "else", statement ].
    /// </summary>
    private Statement ParseIfStatement()
    {
        Match(TokenType.If);
        Match(TokenType.LParen);
        Expression condition = ParseExpression();
        Match(TokenType.RParen);
        Match(TokenType.Then);
        BlockStatement thenBranch = ParseBlock(true);
        BlockStatement? elseBranch = null;

        if (_tokens.Peek().Type == TokenType.Else)
        {
            _tokens.Advance();
            elseBranch = ParseBlock(true);
        }

        return new IfElseStatement(condition, thenBranch, elseBranch);
    }

    /// <summary>
    /// Разбирает список параметров функции.
    /// Правило: parameter_list = parameter, { ",", parameter }.
    /// </summary>
    private List<ParameterDeclarationStatement> ParseParameterList()
    {
        List<ParameterDeclarationStatement> parameters = new();

        if (_tokens.Peek().Type == TokenType.RParen)
        {
            return parameters;
        }

        string paramName = Match(TokenType.Identifier).Value.ToString();
        Match(TokenType.Colon);
        parameters.Add(new ParameterDeclarationStatement(paramName, ParseType()));

        while (_tokens.Peek().Type == TokenType.Comma)
        {
            _tokens.Advance();
            paramName = Match(TokenType.Identifier).Value.ToString();
            Match(TokenType.Colon);
            parameters.Add(new ParameterDeclarationStatement(paramName, ParseType()));
        }

        return parameters;
    }

    /// <summary>
    /// Разбирает оператор возврата.
    /// Правило: return_statement = "return", [ expression ], ";".
    /// </summary>
    private Statement ParseReturnStatement()
    {
        Match(TokenType.Return);
        if (_tokens.Peek().Type == TokenType.Semicolon)
        {
            Match(TokenType.Semicolon);
            return new ReturnStatement(null, _returnTypes.Peek());
        }

        Expression returnValue = ParseExpression();

        Match(TokenType.Semicolon);

        return new ReturnStatement(returnValue, _returnTypes.Peek());
    }

    /// <summary>
    /// Разбирает цикл for.
    /// Правило: for_statement = "for", identifier, "from", expression, "to", expression, "do", block ;
    /// </summary>
    private ForLoopStatement ParseForLoopStatement()
    {
        Match(TokenType.For);

        string iteratorName = Match(TokenType.Identifier).Value.ToString();

        Match(TokenType.From);
        Expression startExpression = ParseExpression();

        Match(TokenType.To);
        Expression endExpression = ParseExpression();

        Match(TokenType.Do);

        BlockStatement body = ParseBlock(false);

        return new ForLoopStatement(new IteratorDeclarationStatement(iteratorName, startExpression), endExpression, body);
    }

    /// <summary>
    /// Разбирает цикл while.
    /// Правило: while_statement = "while", "(", expression, ")", "do", block;
    /// </summary>
    private WhileLoopStatement ParseWhileLoopStatement()
    {
        Match(TokenType.While);
        Match(TokenType.LParen);
        Expression condition = ParseExpression();
        Match(TokenType.RParen);
        Match(TokenType.Do);

        BlockStatement body = ParseBlock(false);

        return new WhileLoopStatement(condition, body);
    }

    /// <summary>
    /// Разбирает тип данных.
    /// Правило: type = "integer" | "float" | "string".
    /// </summary>
    private ValueType ParseType()
    {
        ValueType type = _tokens.Peek().Type switch
        {
            TokenType.IntegerType => ValueType.Integer,
            TokenType.FloatType => ValueType.Float,
            TokenType.StringType => ValueType.String,
            _ => throw new UnexpectedLexemeException(_tokens.Peek()),
        };

        _tokens.Advance();
        return type;
    }

    /// <summary>
    /// Разбирает оператор break.
    /// Правило: break_statement = "break", ";".
    /// </summary>
    private BreakStatement ParseBreakStatement()
    {
        Match(TokenType.Break);
        Match(TokenType.Semicolon);

        return new BreakStatement();
    }

    /// <summary>
    /// Разбирает оператор continue.
    /// Правило: continue_statement = "continue", ";".
    /// </summary>
    private ContinueStatement ParseContinueStatement()
    {
        Match(TokenType.Continue);
        Match(TokenType.Semicolon);

        return new ContinueStatement();
    }

    /// <summary>
    /// Разбирает выражение.
    /// Правило: expression = logical_or_expression.
    /// </summary>
    private Expression ParseExpression()
    {
        return ParseLogicalOrExpression();
    }

    /// <summary>
    /// Разбирает выражение логического ИЛИ.
    /// Правило: logical_or_expression = logical_and_expression, { logical_or_operator, logical_and_expression }.
    /// </summary>
    private Expression ParseLogicalOrExpression()
    {
        Expression left = ParseLogicalAndExpression();

        while (_tokens.Peek().Type == TokenType.LogicalOr)
        {
            _tokens.Advance();
            Expression right = ParseLogicalAndExpression();
            left = new BinaryOperationExpression(left, BinaryOperation.Or, right);
        }

        return left;
    }

    /// <summary>
    /// Разбирает выражение логического И.
    /// Правило: logical_and_expression = comparison_expression, { logical_and_operator, comparison_expression }.
    /// </summary>
    private Expression ParseLogicalAndExpression()
    {
        Expression left = ParseComparisonExpression();

        while (_tokens.Peek().Type == TokenType.LogicalAnd)
        {
            _tokens.Advance();
            Expression right = ParseComparisonExpression();
            left = new BinaryOperationExpression(left, BinaryOperation.And, right);
        }

        return left;
    }

    /// <summary>
    /// Разбирает выражение сравнения.
    /// Правило: comparison_expression = additive_expression, [ comparison_operator, additive_expression ].
    /// </summary>
    private Expression ParseComparisonExpression()
    {
        Expression left = ParseAdditiveExpression();

        if (IsComparisonOperator(_tokens.Peek().Type))
        {
            BinaryOperation operation = _tokens.Peek().Type switch
            {
                TokenType.Equal => BinaryOperation.Equal,
                TokenType.NotEqual => BinaryOperation.NotEqual,
                TokenType.LessThan => BinaryOperation.LessThan,
                TokenType.GreaterThan => BinaryOperation.GreaterThan,
                TokenType.LessThanOrEqual => BinaryOperation.LessThanOrEqual,
                TokenType.GreaterThanOrEqual => BinaryOperation.GreaterThanOrEqual,
                _ => throw new UnexpectedLexemeException(_tokens.Peek()),
            };

            _tokens.Advance();
            Expression right = ParseAdditiveExpression();

            return new BinaryOperationExpression(left, operation, right);
        }

        return left;
    }

    /// <summary>
    /// Разбирает сложение/вычитание.
    /// Правило: additive_expression = term_expression, { additive_operator, term_expression }.
    /// </summary>
    private Expression ParseAdditiveExpression()
    {
        Expression left = ParseTermExpression();

        while (true)
        {
            switch (_tokens.Peek().Type)
            {
                case TokenType.PlusSign:
                    _tokens.Advance();
                    Expression plusRight = ParseTermExpression();
                    left = new BinaryOperationExpression(left, BinaryOperation.Add, plusRight);
                    break;
                case TokenType.MinusSign:
                    _tokens.Advance();
                    Expression minusRight = ParseTermExpression();
                    left = new BinaryOperationExpression(left, BinaryOperation.Subtract, minusRight);
                    break;
                default:
                    return left;
            }
        }
    }

    /// <summary>
    /// Разбирает умножение/деление/остаток.
    /// Правило: term_expression = factor_expression, { multiplicative_operator, factor_expression }.
    /// </summary>
    private Expression ParseTermExpression()
    {
        Expression left = ParseFactorExpression();

        while (true)
        {
            switch (_tokens.Peek().Type)
            {
                case TokenType.MultiplySign:
                    _tokens.Advance();
                    Expression multiplyRight = ParseFactorExpression();
                    left = new BinaryOperationExpression(left, BinaryOperation.Multiply, multiplyRight);
                    break;
                case TokenType.DivideSign:
                    _tokens.Advance();
                    Expression divideRight = ParseFactorExpression();
                    left = new BinaryOperationExpression(left, BinaryOperation.Divide, divideRight);
                    break;
                case TokenType.ModuloSign:
                    _tokens.Advance();
                    Expression moduloRight = ParseFactorExpression();
                    left = new BinaryOperationExpression(left, BinaryOperation.Modulo, moduloRight);
                    break;
                default:
                    return left;
            }
        }
    }

    /// <summary>
    /// Разбирает унарные операции.
    /// Правило: factor_expression = [ unary_operator ], exponentiation_expression.
    /// </summary>
    private Expression ParseFactorExpression()
    {
        if (IsUnaryOperator(_tokens.Peek().Type))
        {
            UnaryOperation operation = _tokens.Peek().Type switch
            {
                TokenType.PlusSign => UnaryOperation.Plus,
                TokenType.MinusSign => UnaryOperation.Minus,
                TokenType.LogicalNot => UnaryOperation.Not,
                _ => throw new UnexpectedLexemeException(_tokens.Peek()),
            };

            _tokens.Advance();
            Expression operand = ParseExponentiationExpression();

            return new UnaryOperationExpression(operation, operand);
        }

        return ParseExponentiationExpression();
    }

    /// <summary>
    /// Разбирает выражение возведения в степень.
    /// Правило: exponentiation_expression = primary_expression, [ "^", exponentiation_expression ].
    /// </summary>
    private Expression ParseExponentiationExpression()
    {
        Expression left = ParsePrimaryExpression();

        if (_tokens.Peek().Type == TokenType.ExponentiationSign)
        {
            _tokens.Advance();
            Expression right = ParseExponentiationExpression();
            return new BinaryOperationExpression(left, BinaryOperation.Exponentiate, right);
        }

        return left;
    }

    /// <summary>
    /// Разбирает первичные выражения (литералы, идентификаторы, выражения в скобках).
    /// Правило: primary_expression = literal | variable_access | "(", expression, ")".
    /// </summary>
    private Expression ParsePrimaryExpression()
    {
        Token token = _tokens.Peek();

        switch (token.Type)
        {
            case TokenType.Integer:
                _tokens.Advance();
                return new LiteralExpression(new Value(token.Value.ToInteger()));

            case TokenType.Float:
                _tokens.Advance();
                return new LiteralExpression(new Value(token.Value.ToFloat()));

            case TokenType.StringLiteral:
                _tokens.Advance();
                return new LiteralExpression(new Value(token.Value.ToString()));

            case TokenType.LParen:
                _tokens.Advance();
                Expression expression = ParseExpression();
                Match(TokenType.RParen);
                return expression;

            case TokenType.Identifier:
                string name = Match(TokenType.Identifier).Value.ToString();
                if (_tokens.Peek().Type == TokenType.LParen)
                {
                    return ParseFunctionCall(name);
                }

                return new VariableExpression(name);

            default:
                throw new UnexpectedLexemeException(token);
        }
    }

    /// <summary>
    /// Проверяет, является ли токен оператором сравнения.
    /// Правило: comparison_operator = "==" | "!=" | "<" | ">" | "<=" | ">=".
    /// </summary>
    private bool IsComparisonOperator(TokenType type)
    {
        return type switch
        {
            TokenType.Equal or
            TokenType.NotEqual or
            TokenType.LessThan or
            TokenType.GreaterThan or
            TokenType.LessThanOrEqual or
            TokenType.GreaterThanOrEqual => true,
            _ => false,
        };
    }

    /// <summary>
    /// Проверяет, является ли токен унарным оператором.
    /// Правило: unary_operator = "+" | "-" | "!".
    /// </summary>
    private bool IsUnaryOperator(TokenType type)
    {
        return type switch
        {
            TokenType.PlusSign or
            TokenType.MinusSign or
            TokenType.LogicalNot => true,
            _ => false,
        };
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