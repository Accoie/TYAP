using Ast.Statements;

using Semantics.Exceptions;

namespace Semantics.Symbols;

/// <summary>
/// Таблица символов, основанная на лексических областях видимости (областях действия) символов в коде.
/// </summary>
public sealed class SymbolsTable
{
    private readonly SymbolsTable? _parent;

    private readonly Dictionary<string, DeclarationStatement> _symbols;

    public SymbolsTable(SymbolsTable? parent)
    {
        _parent = parent;
        _symbols = [];
    }

    public SymbolsTable? Parent => _parent;

    public DeclarationStatement GetSymbol(string name)
    {
        if (_symbols.TryGetValue(name, out DeclarationStatement? symbol))
        {
            return symbol;
        }

        if (_parent != null)
        {
            return _parent.GetSymbol(name);
        }

        throw new UnknownSymbolException(name);
    }

    public void DefineSymbol(string name, DeclarationStatement symbol)
    {
        if (!_symbols.TryAdd(name, symbol))
        {
            throw new DuplicateSymbolException(name);
        }
    }
}