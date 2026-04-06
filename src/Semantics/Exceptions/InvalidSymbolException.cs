namespace Semantics.Exceptions;

#pragma warning disable RCS1194
// Конструкторы исключения не нужны, т.к. это не класс общего назначения.

/// <summary>
/// Исключение из-за некорректного использования символа (функции, переменной, типа).
/// </summary>
public class InvalidSymbolException : Exception
{
    public InvalidSymbolException(string name, string expectedCategory)
        : base($"Name {name} doesn't refer on {expectedCategory}")
    {
        Name = name;
    }

    public string Name { get; }
}
#pragma warning restore RCS1194