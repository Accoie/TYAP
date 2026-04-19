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
        : base($"Type mismatch: {category} expected {expected}, but got {actual}")
    {
    }
}
#pragma warning restore RCS1194