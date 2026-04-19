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
                $"Бинарная операция '{e.Operation}' не допустима для типов {e.Left.ResultType} и {e.Right.ResultType}"
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
                        throw new TypeMismatchException($"Унарный минус/плюс не допустим для типа {operandType}");
                }

                break;

            case UnaryOperation.Not:
                if (operandType != ValueType.Integer )
                {
                    throw new TypeMismatchException(
                        $"Логическое НЕ не допустимо для типа {operandType}"
                    );
                }

                e.ResultType = ValueType.Integer;
                break;

            default:
                throw new NotImplementedException($"Неизвестная унарная операция {e.Operation}");
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
                    throw new TypeMismatchException($"Функция '{e.Name}' ожидает аргумент '{e.Function.Parameters[i].Name} с типом '{e.Arguments[i].ResultType}");
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
        base.Visit(s);

        if (IsBuiltInFunction(s.Name))
        {
            CheckBuiltInFunctionTypes(s.Name, s.Arguments);
        }
        else
        {
            for (int i = 0; i < s.Arguments.Count; i++)
            {
                if (s.Function.Parameters[i].ResultType != s.Arguments[i].ResultType)
                {
                    throw new InvalidFunctionCallException($"Функция '{s.Name}' ожидает аргумент '{s.Function.Parameters[i].Name} с типом '{s.Arguments[i].ResultType}");
                }
            }
        }
    }

    /// <summary>
    /// Проверяет тип переменной и тип выражения, которым она инициализируется.
    /// </summary>
    public override void Visit(VariableDeclarationStatement d)
    {
        base.Visit(d);
        if (d.Value != null)
        {
            ValueType valueType = d.Value.ResultType;

            if (d.DeclaredType != valueType)
            {
                throw new TypeMismatchException(
                    $"Нельзя инициализировать переменную типа {d.DeclaredType} значением типа {valueType}"
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
                $"Тип переменной, которой присваивается значение, не совпадает с объявленным"
            );
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
            throw new TypeMismatchException($"Значение в 'return' не соответствует ожидаемому");
        }
    }

    /// <summary>
    /// Проверяет объявление функции.
    /// </summary>
    public override void Visit(FunctionDeclarationStatement s)
    {
        base.Visit(s);

        if (s.ResultType != ValueType.Void)
        {
            if (!ContainsReturnStatement(s.Body))
            {
                throw new TypeMismatchException(
                    $"Функция '{s.Name}' объявлена с типом возвращаемого значения {s.ResultType}, " +
                    $"но не содержит оператора 'return'"
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
                throw new TypeMismatchException("В выводе не может быть пустой тип");
            }
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
                throw new ArgumentException($"Неизвестная бинарная операция {operation}");
        }
    }

    /// <summary>
    /// Проверяет типы аргументов для встроенной функции, вызываемой как оператор.
    /// </summary>
    private void CheckBuiltInFunctionTypes(string name, IReadOnlyList<Expression> arguments)
    {
        switch (name)
        {
            case "abs":
            case "round":
                if (arguments[0].ResultType != ValueType.Float && arguments[0].ResultType != ValueType.Integer)
                {
                    throw new TypeMismatchException($"Функция '{name}' ожидает числовой аргумент");
                }

                break;
            case "min":
            case "max":
                foreach (Expression arg in arguments)
                {
                    if (arg.ResultType != ValueType.Float && arg.ResultType != ValueType.Integer)
                    {
                        throw new TypeMismatchException($"Функция '{name}' ожидает числовые аргументы");
                    }
                }

                break;

            case "tostring":
                if (arguments[0].ResultType != ValueType.Float && arguments[0].ResultType != ValueType.Integer)
                {
                    throw new TypeMismatchException($"Функция '{name}' ожидает числовой аргумент");
                }

                break;

            default:
                throw new ArgumentException($"Неизвестная встроенная функция: {name}");
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
            "abs", "min", "max", "round",
            "len", "getsymbol", "tostring",
        };

        return builtInFunctions.Any(f =>
            string.Equals(f, name, StringComparison.OrdinalIgnoreCase));
    }
}