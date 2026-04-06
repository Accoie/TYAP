using System.Globalization;

using Ast.Statements;

using Runtime;

using ValueType = Runtime.ValueType;

namespace Compiler;

/// <summary>
/// Объект, предоставляющий доступ к встроенным символам языка.
/// </summary>
public class BuiltInFunctions
{
    public BuiltInFunctions()
    {
        List<BuiltInFunction> functions =
        [
            new BuiltInFunction(
                "abs",
                [new BuiltInFunctionParameter("число", ValueType.Float)],
                ValueType.Float,
                arguments =>
                {
                    double number = arguments[0].AsFloat();
                    return new Value(Math.Abs(number));
                }
            ),

            new BuiltInFunction(
                "min",
                [new BuiltInFunctionParameter("числа", ValueType.Float)],
                ValueType.Float,
                arguments =>
                {
                    if (arguments.Count == 0)
                    {
                        throw new ArgumentException("Использование: min(<число1>, <число2>, ...)");
                    }

                    double min = arguments[0].AsFloat();
                    for (int i = 1; i < arguments.Count; i++)
                    {
                        double current = arguments[i].AsFloat();
                        if (current < min)
                        {
                            min = current;
                        }
                    }

                    return new Value(min);
                }
            ),

            new BuiltInFunction(
                "max",
                [new BuiltInFunctionParameter("числа", ValueType.Float)],
                ValueType.Float,
                arguments =>
                {
                    if (arguments.Count == 0)
                    {
                        throw new ArgumentException("Использование: max(<число1>, <число2>, ...)");
                    }

                    double max = arguments[0].AsFloat();
                    for (int i = 1; i < arguments.Count; i++)
                    {
                        double current = arguments[i].AsFloat();
                        if (current > max)
                        {
                            max = current;
                        }
                    }

                    return new Value(max);
                }
            ),

            new BuiltInFunction(
                "round",
                [new BuiltInFunctionParameter("число", ValueType.Float)],
                ValueType.Float,
                arguments =>
                {
                    double number = arguments[0].AsFloat();
                    return new Value(Math.Round(number));
                }
            ),

            new BuiltInFunction(
                "len",
                [new BuiltInFunctionParameter("число", ValueType.Float)],
                ValueType.Float,
                arguments =>
                {
                    double number = arguments[0].AsFloat();
                    return new Value(Math.Floor(number));
                }
            ),

            new BuiltInFunction(
                "getsymbol",
                [new BuiltInFunctionParameter("число", ValueType.Float), new BuiltInFunctionParameter("степень", ValueType.Float)],
                ValueType.Float,
                arguments =>
                {
                    double number = arguments[0].AsFloat();
                    double power = arguments[1].AsFloat();
                    return new Value((double)Math.Pow((double)number, (double)power));
                }
            ),

            new BuiltInFunction(
                "tostring",
                [new BuiltInFunctionParameter("число", ValueType.Float)],
                ValueType.String,
                arguments =>
                {
                    double number = arguments[0].AsFloat();

                    if (number % 1 == 0)
                    {
                        return new Value(((int)number).ToString());
                    }
                    else
                    {
                        return new Value(number.ToString("0.00", CultureInfo.InvariantCulture));
                    }
                }
            ),
        ];

        Functions = functions.ToDictionary(function => function.Name);
    }

    /// <summary>
    /// Список встроенных функций языка.
    /// </summary>
    public IReadOnlyDictionary<string, BuiltInFunction> Functions { get; }
}