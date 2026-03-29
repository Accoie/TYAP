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

    public override void Visit(VariableExpression e)
    {
        base.Visit(e);

        e.ResultType = e.Variable.ResultType;
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
}