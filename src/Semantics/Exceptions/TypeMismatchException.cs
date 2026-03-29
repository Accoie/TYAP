namespace Semantics.Exceptions;

#pragma warning disable RCS1194
// Конструкторы исключения не нужны, т.к. это не класс общего назначения.

/// <summary>
/// Исключение из-за несовместимых типов данных в программе.
/// </summary>
public class TypeMismatchException : Exception
{
    public TypeMismatchException(string message)
        : base(message)
    {
    }

    public TypeMismatchException(string category, ValueType expected, ValueType actual)
        : base($"Несовместимость типов: {category} должно быть типа {expected}, а получено {actual}")
    {
    }
}
#pragma warning restore RCS1194