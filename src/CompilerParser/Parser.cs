using Ast.Declaration;
using Ast.Expressions;
using Ast.Statements;
using Ast.Types;

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
    /// Разбирает оператор присваивания или вызов функции.
    /// </summary>
    private Statement ParseAssignmentOrFunctionCall()
    {
        Expression target = ParseLeftHandSide();

        if (_tokens.Peek().Type == TokenType.Assign)
        {
            _tokens.Advance();
            Expression value = ParseExpression();
            Match(TokenType.Semicolon);
            return new AssignmentStatement(target, value);
        }

        if (target is not VariableExpression variableExpression)
        {
            throw new UnexpectedLexemeException(_tokens.Peek());
        }

        Statement result = ParseFunctionCallStatement(variableExpression.Name);
        Match(TokenType.Semicolon);
        return result;
    }

    /// <summary>
    /// Разбирает lvalue: identifier, { index_access }.
    /// </summary>
    private Expression ParseLeftHandSide()
    {
        string name = Match(TokenType.Identifier).Value.ToString();
        Expression target = new VariableExpression(name);

        while (_tokens.Peek().Type == TokenType.LBracket)
        {
            _tokens.Advance();
            Expression index = ParseExpression();
            Match(TokenType.RBracket);
            target = new ArrayAccessExpression(target, index);
        }

        return target;
    }

    /// <summary>
    /// Разбирает оператор объявления переменной.
    /// </summary>
    private VariableDeclaration ParseVariableDeclaration()
    {
        Match(TokenType.Var);
        string name = Match(TokenType.Identifier).Value.ToString();
        Match(TokenType.Colon);

        TypeNode type = ParseType();

        Expression? initialValue = null;
        if (_tokens.Peek().Type == TokenType.Assign)
        {
            _tokens.Advance();
            initialValue = ParseExpression();
        }

        Match(TokenType.Semicolon);

        return new VariableDeclaration(name, type, initialValue);
    }

    /// <summary>
    /// Разбирает оператор ввода.
    /// </summary>
    private InputStatement ParseInput()
    {
        Match(TokenType.Input);
        Match(TokenType.LParen);

        Expression target = ParseLeftHandSide();

        Match(TokenType.RParen);
        Match(TokenType.Semicolon);
        return new InputStatement(target);
    }

    /// <summary>
    /// Разбирает оператор вывода.
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
    /// </summary>
    private FunctionDeclaration ParseFunctionDeclaration()
    {
        Match(TokenType.Function);
        string name = Match(TokenType.Identifier).Value.ToString();

        Match(TokenType.LParen);
        List<ParameterDeclaration> parameters = ParseParameterList();

        Match(TokenType.RParen);

        TokenType returnTypetokens = _tokens.Peek().Type;
        ValueType resultType;
        if (returnTypetokens == TokenType.Colon)
        {
            Match(TokenType.Colon);
            resultType = ParseScalarType();
        }
        else
        {
            resultType = ValueType.Void;
        }

        _returnTypes.Push(resultType);

        BlockStatement body = ParseBlock(false);

        _returnTypes.Pop();

        return new FunctionDeclaration(name, parameters, body, resultType);
    }

    /// <summary>
    /// Разбирает вызов функции.
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
    /// Разбирает вызов функции как оператор.
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
    /// </summary>
    private List<ParameterDeclaration> ParseParameterList()
    {
        List<ParameterDeclaration> parameters = new();

        if (_tokens.Peek().Type == TokenType.RParen)
        {
            return parameters;
        }

        string paramName = Match(TokenType.Identifier).Value.ToString();
        Match(TokenType.Colon);
        parameters.Add(new ParameterDeclaration(paramName, ParseType()));

        while (_tokens.Peek().Type == TokenType.Comma)
        {
            _tokens.Advance();
            paramName = Match(TokenType.Identifier).Value.ToString();
            Match(TokenType.Colon);
            parameters.Add(new ParameterDeclaration(paramName, ParseType()));
        }

        return parameters;
    }

    /// <summary>
    /// Разбирает оператор возврата.
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

        return new ForLoopStatement(new IteratorDeclaration(iteratorName, startExpression), endExpression, body);
    }

    /// <summary>
    /// Разбирает цикл while.
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
    /// Правило: type = base_type | array_type.
    /// </summary>
    private TypeNode ParseType()
    {
        if (_tokens.Peek().Type == TokenType.Array)
        {
            return ParseArrayType();
        }

        return new ScalarTypeNode(ParseScalarType());
    }

    /// <summary>
    /// Разбирает скалярный тип для возвращаемого значения функции.
    /// </summary>
    private ValueType ParseScalarType()
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
    /// Разбирает тип массива: arr "[", expression, "]", "of", base_type.
    /// </summary>
    private ArrayTypeNode ParseArrayType()
    {
        Match(TokenType.Array);
        Match(TokenType.LBracket);
        Expression size = ParseExpression();
        Match(TokenType.RBracket);
        Match(TokenType.Of);
        ValueType elementType = ParseScalarType();
        return new ArrayTypeNode(size, elementType);
    }

    /// <summary>
    /// Разбирает оператор break.
    /// </summary>
    private BreakStatement ParseBreakStatement()
    {
        Match(TokenType.Break);
        Match(TokenType.Semicolon);

        return new BreakStatement();
    }

    /// <summary>
    /// Разбирает оператор continue.
    /// </summary>
    private ContinueStatement ParseContinueStatement()
    {
        Match(TokenType.Continue);
        Match(TokenType.Semicolon);

        return new ContinueStatement();
    }

    /// <summary>
    /// Разбирает выражение.
    /// </summary>
    private Expression ParseExpression()
    {
        return ParseLogicalOrExpression();
    }

    /// <summary>
    /// Разбирает выражение логического ИЛИ.
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
    /// </summary>
    private Expression ParseTermExpression()
    {
        Expression left = ParseExponentiationExpression();

        while (true)
        {
            switch (_tokens.Peek().Type)
            {
                case TokenType.MultiplySign:
                    _tokens.Advance();
                    Expression multiplyRight = ParseExponentiationExpression();
                    left = new BinaryOperationExpression(left, BinaryOperation.Multiply, multiplyRight);
                    break;
                case TokenType.DivideSign:
                    _tokens.Advance();
                    Expression divideRight = ParseExponentiationExpression();
                    left = new BinaryOperationExpression(left, BinaryOperation.Divide, divideRight);
                    break;
                case TokenType.ModuloSign:
                    _tokens.Advance();
                    Expression moduloRight = ParseExponentiationExpression();
                    left = new BinaryOperationExpression(left, BinaryOperation.Modulo, moduloRight);
                    break;
                default:
                    return left;
            }
        }
    }

    /// <summary>
    /// Разбирает унарные операции.
    /// </summary>
    private Expression ParseUnaryExpression()
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
            Expression operand = ParsePostfixExpression();

            return new UnaryOperationExpression(operation, operand);
        }

        return ParsePostfixExpression();
    }

    /// <summary>
    /// Разбирает выражение возведения в степень.
    /// </summary>
    private Expression ParseExponentiationExpression()
    {
        Expression left = ParseUnaryExpression();

        if (_tokens.Peek().Type == TokenType.ExponentiationSign)
        {
            _tokens.Advance();
            Expression right = ParseExponentiationExpression();
            return new BinaryOperationExpression(left, BinaryOperation.Exponentiate, right);
        }

        return left;
    }

    /// <summary>
    /// Разбирает постфиксные операции (индексация массива).
    /// </summary>
    private Expression ParsePostfixExpression()
    {
        Expression expression = ParsePrimaryExpression();

        while (_tokens.Peek().Type == TokenType.LBracket)
        {
            _tokens.Advance();
            Expression index = ParseExpression();
            Match(TokenType.RBracket);
            expression = new ArrayAccessExpression(expression, index);
        }

        return expression;
    }

    /// <summary>
    /// Разбирает первичные выражения.
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

            case TokenType.LBracket:
                return ParseArrayLiteral();

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
    /// Разбирает литерал массива: "[", [ expression, { ",", expression } ], "]".
    /// </summary>
    private ArrayLiteralExpression ParseArrayLiteral()
    {
        Match(TokenType.LBracket);

        List<Expression> elements = new();
        if (_tokens.Peek().Type != TokenType.RBracket)
        {
            elements.Add(ParseExpression());
            while (_tokens.Peek().Type == TokenType.Comma)
            {
                _tokens.Advance();
                elements.Add(ParseExpression());
            }
        }

        Match(TokenType.RBracket);
        return new ArrayLiteralExpression(elements);
    }

    /// <summary>
    /// Проверяет, является ли токен оператором сравнения.
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
