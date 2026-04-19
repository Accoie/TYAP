using System.Globalization;

using Ast.Statements;

using Runtime;

using ValueType = Runtime.ValueType;

namespace RusMatushkaParser;

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
                [new BuiltInFunctionParameter("number", ValueType.Integer)],
                ValueType.Integer,
                arguments =>
                {
                    int number = arguments[0].AsInteger();
                    return new Value(Math.Abs(number));
                }
            ),

            new BuiltInFunction(
                "abs",
                [new BuiltInFunctionParameter("number", ValueType.Float)],
                ValueType.Float,
                arguments =>
                {
                    double number = arguments[0].AsFloat();
                    return new Value(Math.Abs(number));
                }
            ),

            new BuiltInFunction(
                "min",
                [new BuiltInFunctionParameter("a", ValueType.Float),
                 new BuiltInFunctionParameter("b", ValueType.Float)],
                ValueType.Float,
                arguments =>
                {
                    if (arguments.Count == 0)
                    {
                        throw new ArgumentException("Использование: min(<number1>, <number2>, ...)");
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
                [new BuiltInFunctionParameter("a", ValueType.Float),
                 new BuiltInFunctionParameter("b", ValueType.Float)],
                ValueType.Float,
                arguments =>
                {
                    if (arguments.Count == 0)
                    {
                        throw new ArgumentException("Использование: max(<number1>, <number2>, ...)");
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
                [new BuiltInFunctionParameter("number", ValueType.Float)],
                ValueType.Float,
                arguments =>
                {
                    double number = arguments[0].AsFloat();
                    return new Value(Math.Round(number));
                }
            ),

            new BuiltInFunction(
                "len",
                [new BuiltInFunctionParameter("str", ValueType.String)],
                ValueType.Integer,
                arguments =>
                {
                    string str = arguments[0].AsString();
                    return new Value(str.Length);
                }
            ),

            new BuiltInFunction(
                "getsymbol",
                [
                    new BuiltInFunctionParameter("str", ValueType.String),
                    new BuiltInFunctionParameter("index", ValueType.Integer)
                ],
                ValueType.String,
                arguments =>
                {
                    string str = arguments[0].AsString();
                    int index = arguments[1].AsInteger();

                    if (index < 1 || index > str.Length)
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(index),
                            $"Index {index} is out of range [1, {str.Length}]");
                    }

                    return new Value(str[index - 1].ToString());
                }
            ),

            new BuiltInFunction(
                "tostring",
                [new BuiltInFunctionParameter("number", ValueType.Integer )],
                ValueType.String,
                arguments =>
                {
                    int number = arguments[0].AsInteger();

                    if (number % 1 == 0)
                    {
                        return new Value(number.ToString());
                    }
                    else
                    {
                        return new Value(number.ToString("0.00", CultureInfo.InvariantCulture));
                    }
                }
            ),

            new BuiltInFunction(
                "tostring",
                [new BuiltInFunctionParameter("number", ValueType.Float)],
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