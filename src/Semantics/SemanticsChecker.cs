using Ast;
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

    public SemanticsChecker()
    {
        SymbolsTable globalSymbols = new(parent: null);

        HashSet<string> addedFunctions = new();
        foreach (BuiltInFunctionStatement function in Builtins.Functions)
        {
            if (addedFunctions.Add(function.Name))
            {
                globalSymbols.DefineSymbol(function.Name, function);
            }
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