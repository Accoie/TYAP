using Ast.Declaration;

using ValueType = Runtime.ValueType;

namespace Ast;

/// <summary>
/// Объект, предоставляющий доступ к встроенным символам языка.
/// </summary>
public static class Builtins
{
    public const string Abs = "abs_f";
    public const string Min = "min_f";
    public const string Max = "max_f";
    public const string Len = "len";
    public const string Round = "round";
    public const string Getsymbol = "getsymbol";
    public const string TostringI = "tostring_i";
    public const string TostringF = "tostring_f";

    public const string Integer = "integer";
    public const string String = "string";
    public const string Float = "float";

    /// <summary>
    /// Список встроенных функций языка.
    /// Все функции без суффиксов - одна версия для каждой.
    /// Для tostring используется float (double), int будет конвертироваться автоматически.
    /// </summary>
    public static readonly IReadOnlyList<BuiltInFunctionDeclaration> Functions =
    [
        new(
            Abs, // `abs_f(number: float): float` — возвращает модуль числа
            [
                new BuiltInFunctionParameterDeclaration("number", ValueType.Float),
            ],
            ValueType.Float
        ),

        new(
            Min, // `min_f(a: float, b: float): float` — возвращает минимальное из чисел
            [
                new BuiltInFunctionParameterDeclaration("a", ValueType.Float),
                new BuiltInFunctionParameterDeclaration("b", ValueType.Float),
            ],
            ValueType.Float
        ),

        new(
            Max, // `max_f(a: float, b: float): float` — возвращает максимальное из чисел
            [
                new BuiltInFunctionParameterDeclaration("a", ValueType.Float),
                new BuiltInFunctionParameterDeclaration("b", ValueType.Float),
            ],
            ValueType.Float
        ),

        new(
            Round, // `round(number: float): float` — округляет число до ближайшего целого
            [
                new BuiltInFunctionParameterDeclaration("number", ValueType.Float),
            ],
            ValueType.Float
        ),

        new(
            Len, // `len(str: string): int` — возвращает длину строки
            [
                new BuiltInFunctionParameterDeclaration("str", ValueType.String),
            ],
            ValueType.Integer
        ),

        new(
            Getsymbol, // `getsymbol(str: string, index: int): string` — возвращает символ строки по индексу (1-based)
            [
                new BuiltInFunctionParameterDeclaration("str", ValueType.String),
                new BuiltInFunctionParameterDeclaration("index", ValueType.Integer),
            ],
            ValueType.String
        ),

        new(
            TostringI, // `tostring_i(number: int): string` — преобразует целое число в строку
            [
                new BuiltInFunctionParameterDeclaration("number", ValueType.Integer),
            ],
            ValueType.String
        ),

        new(
            TostringF, // `tostring_f(number: float): string` — преобразует вещественное число в строку
            [
                new BuiltInFunctionParameterDeclaration("number", ValueType.Float),
            ],
            ValueType.String
        ),
    ];

    public static bool IsBuiltInFunction(string name)
    {
        string[] builtInFunctions =
        {
            Abs, Min, Max, Round, Len, Getsymbol, TostringI, TostringF,
        };

        foreach (string builtIn in builtInFunctions)
        {
            if (string.Equals(builtIn, name))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsBuiltInType(string name)
    {
        string[] builtInTypes = { Integer, Float, String };

        foreach (string type in builtInTypes)
        {
            if (string.Equals(type, name))
            {
                return true;
            }
        }

        return false;
    }
}