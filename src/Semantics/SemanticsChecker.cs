using Ast.Statements;

using Semantics.Passes;
using Semantics.Symbols;

namespace Semantics;

/// <summary>
/// Фасад для проведения семантических проверок программы.
/// Выполняет три прохода по AST:
/// 1. ResolveNamesPass - разрешение имен и проверка областей видимости
/// 2. ResolveTypesPass - проверка типов данных
/// </summary>
public class SemanticsChecker
{
    private readonly AbstractPass[] _passes;

    public SemanticsChecker(IReadOnlyDictionary<string, BuiltInFunction> builtinFunctions)
    {
        SymbolsTable globalSymbols = new(parent: null);
        foreach ((string name, BuiltInFunction function) in builtinFunctions)
        {
            globalSymbols.DefineSymbol(name, function);
        }

        _passes =
        [
            new ResolveNamesPass(globalSymbols),
            new ResolveTypesPass(),
        ];
    }

    public void Check(BlockStatement program)
    {
        foreach (AbstractPass pass in _passes)
        {
            program.Accept(pass);
        }
    }
}