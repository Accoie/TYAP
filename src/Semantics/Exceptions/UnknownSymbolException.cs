namespace Semantics.Exceptions;

#pragma warning disable RCS1194
// Конструкторы исключения не нужны, т.к. это не класс общего назначения.

/// <summary>
/// Исключение из-за отсутствия символа с указанным именем.
/// </summary>
public class UnknownSymbolException : Exception
{
    public UnknownSymbolException(string name)
        : base($"Имя {name} не объявлено в текущем контексте")
    {
        Name = name;
    }

    public string Name { get; }
}
#pragma warning restore RCS1194