using Ast;
using Ast.BuiltIn;
using Ast.Declaration;
using Ast.Expressions;
using Ast.Statements;

using Semantics.Exceptions;

using ValueType = Runtime.ValueType;

namespace Semantics.Passes;

/// <summary>
/// Проход по AST выполняет две задачи:
///  1. Вычислить типы данных.
///  2. Проверить корректность программы с точки зрения совместимости типов данных.
/// </summary>
/// <exception cref="TypeMismatchException">Бросается при несоответствии типов данных.</exception>
public sealed class ResolveTypesPass : AbstractPass
{
    private int _loopNestingLevel = 0;

    /// <summary>
    /// Литерал всегда имеет определённый тип.
    /// </summary>
    public override void Visit(LiteralExpression e)
    {
        base.Visit(e);
        e.ResultType = e.Value.GetValueType();
    }

    /// <summary>
    /// Выполняет проверки типов для бинарных операций.
    /// </summary>
    public override void Visit(BinaryOperationExpression e)
    {
        base.Visit(e);

        ValueType? resultType = GetBinaryOperationResultType(
            e.Operation,
            e.Left.ResultType,
            e.Right.ResultType
        );

        if (resultType == null)
        {
            throw new TypeMismatchException(
                $"Binary operation '{e.Operation}' is not valid for types {e.Left.ResultType} and {e.Right.ResultType}"
            );
        }

        e.ResultType = resultType.Value;
    }

    /// <summary>
    /// Выполняет проверки типов для унарных операций.
    /// </summary>
    public override void Visit(UnaryOperationExpression e)
    {
        e.Operand.Accept(this);

        ValueType operandType = e.Operand.ResultType;

        switch (e.Operation)
        {
            case UnaryOperation.Minus:
            case UnaryOperation.Plus:
                switch(operandType)
                {
                    case ValueType.Integer:
                        e.ResultType = ValueType.Integer;
                        break;
                    case ValueType.Float:
                        e.ResultType = ValueType.Float;
                        break;
                    default:
                        throw new TypeMismatchException($"Unary minus/plus is not valid for type {operandType}");
                }

                break;

            case UnaryOperation.Not:
                if (operandType != ValueType.Integer )
                {
                    throw new TypeMismatchException(
                        $"Logical NOT is not valid for type {operandType}"
                    );
                }

                e.ResultType = ValueType.Integer;
                break;

            default:
                throw new NotImplementedException($"Unknown unary operation {e.Operation}");
        }
    }

    public override void Visit(VariableExpression e)
    {
        base.Visit(e);

        e.ResultType = e.Variable.ResultType;
    }

    /// <summary>
    /// Проверяет соответствие типов параметров функции и аргументов при вызове этой функции.
    /// </summary>
    public override void Visit(FunctionCallExpression e)
    {
        base.Visit(e);

        if (IsBuiltInFunction(e.Name))
        {
            CheckBuiltInFunctionTypes(e.Name, e.Arguments);
        }
        else
        {
            for (int i = 0; i < e.Arguments.Count; i++)
            {
                if (e.Function.Parameters[i].ResultType != e.Arguments[i].ResultType)
                {
                    throw new TypeMismatchException($"Function '{e.Name}' expects argument '{e.Function.Parameters[i].Name}' with type '{e.Arguments[i].ResultType}'");
                }
            }
        }

        e.ResultType = e.Function.ResultType;
    }

    /// <summary>
    /// Проверяет соответствие типов параметров функции и аргументов при вызове функции как оператора.
    /// </summary>
    public override void Visit(FunctionCallStatement s)
    {
        s.Expression.Accept(this);
    }

    /// <summary>
    /// Проверяет тип переменной и тип выражения, которым она инициализируется.
    /// </summary>
    public override void Visit(VariableDeclaration d)
    {
        base.Visit(d);
        if (d.Value != null)
        {
            ValueType valueType = d.Value.ResultType;

            if (d.DeclaredType != valueType)
            {
                throw new TypeMismatchException(
                    $"Cannot initialize variable of type {d.DeclaredType} with value of type {valueType}"
                );
            }
        }

        d.ResultType = d.DeclaredType;
    }

    /// <summary>
    /// Проверяет тип переменной и тип выражения при присваивании.
    /// </summary>
    public override void Visit(AssignmentStatement s)
    {
        base.Visit(s);

        ValueType valueType = s.Value.ResultType;

        if (s.Value.ResultType != s.Variable.ResultType)
        {
            throw new TypeMismatchException(
                $"Type of variable being assigned does not match the declared type"
            );
        }

        if (s.Variable is IteratorDeclaration)
        {
            throw new InvalidAssignmentException($"Cannot assign to for loop iterator '{s.Name}'");
        }
    }

    /// <summary>
    /// Проверяет блок операторов.
    /// </summary>
    public override void Visit(BlockStatement s)
    {
        base.Visit(s);
    }

    /// <summary>
    /// Проверяет оператор ветвления.
    /// </summary>
    public override void Visit(IfElseStatement s)
    {
        base.Visit(s);

        if (s.Condition.ResultType != ValueType.Integer)
        {
            throw new TypeMismatchException(
                $"If condition must be of type Integer, but got {s.Condition.ResultType}"
            );
        }
    }

    /// <summary>
    /// Проверяет типы в цикле while.
    /// </summary>
    public override void Visit(WhileLoopStatement s)
    {
        _loopNestingLevel++;
        try
        {
            base.Visit(s);

            if (s.Condition.ResultType != ValueType.Integer)
            {
                throw new TypeMismatchException(
                    $"Condition of while loop must be integer, but got {s.Condition.ResultType}"
                );
            }
        }
        finally
        {
            _loopNestingLevel--;
        }
    }

    /// <summary>
    /// Проверяет типы в цикле for.
    /// </summary>
    public override void Visit(ForLoopStatement s)
    {
        _loopNestingLevel++;
        try
        {
            base.Visit(s);

            if (s.Iterator.StartValue.ResultType != ValueType.Integer)
            {
                throw new TypeMismatchException(
                    $"Start value of for loop must be integer, but got {s.Iterator.StartValue.ResultType}"
                );
            }

            if (s.EndValue.ResultType != ValueType.Integer)
            {
                throw new TypeMismatchException(
                    $"End value of for loop must be integer, but got {s.EndValue.ResultType}"
                );
            }
        }
        finally
        {
            _loopNestingLevel--;
        }
    }

    public override void Visit(IteratorDeclaration iteratorDeclarationStatement)
    {
        base.Visit(iteratorDeclarationStatement);
        if (iteratorDeclarationStatement.StartValue.ResultType != ValueType.Integer)
        {
            throw new TypeMismatchException(
                $"Iterator's value in for loop must be integer, but got {iteratorDeclarationStatement.StartValue.ResultType}");
        }

        iteratorDeclarationStatement.ResultType = ValueType.Integer;
    }

    /// <summary>
    /// Проверяет оператор return.
    /// </summary>
    public override void Visit(ReturnStatement s)
    {
        base.Visit(s);

        bool isTypeMismatch = s.Value == null
            ? s.Type != ValueType.Void
            : s.Type != s.Value.ResultType;

        if (isTypeMismatch)
        {
            throw new TypeMismatchException($"Return value does not match the expected type");
        }
    }

    /// <summary>
    /// Проверяет объявление функции.
    /// </summary>
    public override void Visit(FunctionDeclaration s)
    {
        base.Visit(s);

        if (s.ResultType != ValueType.Void)
        {
            if (!ContainsReturnStatement(s.Body))
            {
                throw new TypeMismatchException(
                    $"Function '{s.Name}' is declared with return type {s.ResultType}, " +
                    $"but does not contain a 'return' statement"
                );
            }
        }
    }

    /// <summary>
    /// Проверяет объявление параметра.
    /// </summary>
    public override void Visit(ParameterDeclaration d)
    {
        base.Visit(d);
    }

    /// <summary>
    /// Проверяет оператор ввода.
    /// </summary>
    public override void Visit(InputStatement s)
    {
        base.Visit(s);
    }

    /// <summary>
    /// Проверяет оператор вывода.
    /// </summary>
    public override void Visit(OutputStatement s)
    {
        base.Visit(s);

        foreach (Expression arg in s.Arguments)
        {
            if (arg.ResultType == ValueType.Void)
            {
                throw new TypeMismatchException("Output cannot contain void type");
            }
        }
    }

    /// <summary>
    /// Проверяет оператор break.
    /// </summary>
    public override void Visit(BreakStatement s)
    {
        base.Visit(s);

        if (_loopNestingLevel == 0)
        {
            throw new InvalidExpressionException("Break statement must be inside a loop");
        }
    }

    /// <summary>
    /// Проверяет оператор continue.
    /// </summary>
    public override void Visit(ContinueStatement s)
    {
        base.Visit(s);

        if (_loopNestingLevel == 0)
        {
            throw new InvalidExpressionException("Continue statement must be inside a loop");
        }
    }

    /// <summary>
    /// Вычисляет тип результата бинарной операции.
    /// Возвращает null, если бинарная операция не может быть выполнена с указанными типами.
    /// </summary>
    private static ValueType? GetBinaryOperationResultType(
        BinaryOperation operation,
        ValueType left,
        ValueType right)
    {
        switch (operation)
        {
            case BinaryOperation.Add:
                if (left == ValueType.Float && right == ValueType.Float)
                {
                    return ValueType.Float;
                }

                if (left == ValueType.Integer && right == ValueType.Integer)
                {
                    return ValueType.Integer;
                }

                if (left == ValueType.String && right == ValueType.String)
                {
                    return ValueType.String;
                }

                return null;

            case BinaryOperation.Subtract:
            case BinaryOperation.Multiply:
            case BinaryOperation.Divide:
            case BinaryOperation.Modulo:
            case BinaryOperation.Exponentiate:
                if (left == ValueType.Float && right == ValueType.Float)
                {
                    return ValueType.Float;
                }

                if (left == ValueType.Integer && right == ValueType.Integer)
                {
                    return ValueType.Integer;
                }

                return null;

            case BinaryOperation.LessThan:
            case BinaryOperation.GreaterThan:
            case BinaryOperation.LessThanOrEqual:
            case BinaryOperation.GreaterThanOrEqual:
                if (left == ValueType.Float && right == ValueType.Float)
                {
                    return ValueType.Integer;
                }

                if (left == ValueType.Integer && right == ValueType.Integer)
                {
                    return ValueType.Integer;
                }

                if (left == ValueType.String && right == ValueType.String)
                {
                    return ValueType.Integer;
                }

                return null;

            case BinaryOperation.Equal:
            case BinaryOperation.NotEqual:
                if (left == right && left != ValueType.Void)
                {
                    return ValueType.Integer;
                }

                return null;

            case BinaryOperation.And:
            case BinaryOperation.Or:
                if (left == ValueType.Integer && right == ValueType.Integer )
                {
                    return ValueType.Integer;
                }

                return null;

            default:
                throw new ArgumentException($"Unknown binary operation {operation}");
        }
    }

    /// <summary>
    /// Проверяет типы аргументов для встроенной функции, вызываемой как оператор.
    /// </summary>
    private void CheckBuiltInFunctionTypes(string name, IReadOnlyList<Expression> arguments)
    {
        BuiltInFunction? builtin = Builtins.Functions.FirstOrDefault(f => f.Name == name);
        if (builtin == null)
        {
            throw new ArgumentException($"Unknown built-in function: {name}");
        }

        if (arguments.Count != builtin.Parameters.Count)
        {
            throw new InvalidFunctionCallException(
                $"Function '{name}' expects {builtin.Parameters.Count} arguments, " +
                $"but got {arguments.Count}"
            );
        }

        switch (name)
        {
            case Builtins.Abs:
            case Builtins.Round:
                if (arguments[0].ResultType != ValueType.Float)
                {
                    throw new TypeMismatchException($"Function '{name}' expects a numeric argument");
                }

                break;
            case Builtins.Min:
            case Builtins.Max:
                foreach (Expression arg in arguments)
                {
                    if (arg.ResultType != ValueType.Float)
                    {
                        throw new TypeMismatchException($"Function '{name}' expects numeric arguments");
                    }
                }

                break;

            case Builtins.TostringI:
                if (arguments[0].ResultType != ValueType.Integer)
                {
                    throw new TypeMismatchException($"Function '{name}' expects an integer argument");
                }

                break;
            case Builtins.TostringF:
                if (arguments[0].ResultType != ValueType.Float)
                {
                    throw new TypeMismatchException($"Function '{name}' expects a float argument");
                }

                break;
            case Builtins.Getsymbol:
                if (arguments[0].ResultType != ValueType.String || arguments[1].ResultType != ValueType.Integer)
                {
                    throw new TypeMismatchException($"Function '{name}' expects a string and an integer argument");
                }

                break;
            case Builtins.Len:
                if (arguments[0].ResultType != ValueType.String)
                {
                    throw new TypeMismatchException($"Function '{name}' expects a string argument");
                }

                break;
            default:
                throw new ArgumentException($"Unknown built-in function: {name}");
        }
    }

    /// <summary>
    /// Проверяет, содержит ли блок хотя бы один ReturnStatement.
    /// Рекурсивно проверяет вложенные блоки.
    /// </summary>
    private bool ContainsReturnStatement(BlockStatement block)
    {
        foreach (Statement statement in block.Statements)
        {
            if (statement is ReturnStatement)
            {
                return true;
            }

            if (statement is IfElseStatement ifElse)
            {
                if (ContainsReturnStatement(ifElse.ThenBranch) ||
                    (ifElse.ElseBranch != null && ContainsReturnStatement(ifElse.ElseBranch)))
                {
                    return true;
                }
            }
            else if (statement is BlockStatement nestedBlock)
            {
                if (ContainsReturnStatement(nestedBlock))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Проверяет, является ли функция встроенной.
    /// </summary>
    private bool IsBuiltInFunction(string name)
    {
        string[] builtInFunctions =
        {
            "abs_f", "min_f", "max_f", "round",
            "len", "getsymbol", "tostring_i", "tostring_f",
        };

        return builtInFunctions.Any(f =>
            string.Equals(f, name, StringComparison.OrdinalIgnoreCase));
    }
}