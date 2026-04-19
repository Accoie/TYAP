namespace Semantics.Exceptions;

#pragma warning disable RCS1194
// Конструкторы исключения не нужны, т.к. это не класс общего назначения.

/// <summary>
/// Исключение из-за повторного объявления символа с тем же именем.
/// </summary>
public class DuplicateSymbolException : Exception
{
    public DuplicateSymbolException(string name)
        : base($"Имя {name} уже объявлено в текущей области")
    {
    }
}
#pragma warning restore RCS1194