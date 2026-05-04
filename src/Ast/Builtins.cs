using Ast.Statements;

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

    /// <summary>
    /// Список встроенных функций языка.
    /// Все функции без суффиксов - одна версия для каждой.
    /// Для tostring используется float (double), int будет конвертироваться автоматически.
    /// </summary>
    public static readonly IReadOnlyList<BuiltInFunction> Functions =
    [
        new(
            Abs, // `abs_f(number: float): float` — возвращает модуль числа
            [
                new BuiltInFunctionParameter("number", ValueType.Float),
            ],
            ValueType.Float
        ),

        new(
            Min, // `min_f(a: float, b: float): float` — возвращает минимальное из чисел
            [
                new BuiltInFunctionParameter("a", ValueType.Float),
                new BuiltInFunctionParameter("b", ValueType.Float),
            ],
            ValueType.Float
        ),

        new(
            Max, // `max_f(a: float, b: float): float` — возвращает максимальное из чисел
            [
                new BuiltInFunctionParameter("a", ValueType.Float),
                new BuiltInFunctionParameter("b", ValueType.Float),
            ],
            ValueType.Float
        ),

        new(
            Round, // `round(number: float): float` — округляет число до ближайшего целого
            [
                new BuiltInFunctionParameter("number", ValueType.Float),
            ],
            ValueType.Float
        ),

        new(
            Len, // `len(str: string): int` — возвращает длину строки
            [
                new BuiltInFunctionParameter("str", ValueType.String),
            ],
            ValueType.Integer
        ),

        new(
            Getsymbol, // `getsymbol(str: string, index: int): string` — возвращает символ строки по индексу (1-based)
            [
                new BuiltInFunctionParameter("str", ValueType.String),
                new BuiltInFunctionParameter("index", ValueType.Integer),
            ],
            ValueType.String
        ),

        new(
            TostringI, // `tostring_i(number: int): string` — преобразует целое число в строку
            [
                new BuiltInFunctionParameter("number", ValueType.Integer),
            ],
            ValueType.String
        ),

        new(
            TostringF, // `tostring_f(number: float): string` — преобразует вещественное число в строку
            [
                new BuiltInFunctionParameter("number", ValueType.Float),
            ],
            ValueType.String
        ),
    ];
}