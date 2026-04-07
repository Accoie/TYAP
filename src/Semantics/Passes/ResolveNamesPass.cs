using Ast.Expressions;
using Ast.Statements;

using Semantics.Exceptions;
using Semantics.Symbols;

namespace Semantics.Passes;

public sealed class ResolveNamesPass : AbstractPass
{
    private SymbolsTable _symbols;

    public ResolveNamesPass(SymbolsTable globalSymbols)
    {
        _symbols = globalSymbols;
    }

    public override void Visit(VariableExpression e)
    {
        base.Visit(e);

        try
        {
            DeclarationStatement symbol = _symbols.GetSymbol(e.Name);

            if (!(symbol is AbstractVariableDeclaration))
            {
                throw new InvalidSymbolException(e.Name, "variable");
            }

            e.Variable = (AbstractVariableDeclaration)symbol;
        }
        catch (UnknownSymbolException)
        {
            throw new UnknownSymbolException($"Неизвестная переменная '{e.Name}'");
        }
    }

    public override void Visit(InputStatement s)
    {
        DeclarationStatement symbol = _symbols.GetSymbol(s.VariableName);
        base.Visit(s);
    }

    public override void Visit(BlockStatement s)
    {
        if (s.IsNewScope)
        {
            _symbols = new SymbolsTable(_symbols );
        }

        try
        {
            ProcessDeclarationsAndStatements(s.Statements);
        }
        finally
        {
            if (s.IsNewScope)
            {
                _symbols = _symbols.Parent!;
            }
        }
    }

    public override void Visit(VariableDeclarationStatement d)
    {
        base.Visit(d);

        if (IsBuiltInFunction(d.Name))
        {
            throw DuplicateSymbolException.DuplicateVariableOrFunction(d.Name);
        }

        _symbols.DefineSymbol(d.Name, d);
    }

    public override void Visit(AssignmentStatement s)
    {
        base.Visit(s);
        DeclarationStatement symbol = _symbols.GetSymbol(s.Name);

        if (symbol is not AbstractVariableDeclaration)
        {
            throw new InvalidSymbolException(s.Name, "variable");
        }

        s.Variable = (AbstractVariableDeclaration)symbol;
    }

    private void ProcessDeclarationsAndStatements(IEnumerable<Statement> statements)
    {
        foreach (Statement statement in statements)
        {
            if (statement is VariableDeclarationStatement variable)
            {
                variable.Accept(this);
            }
        }

        foreach (Statement statement in statements)
        {
            if (statement is VariableDeclarationStatement variable)
            {
            }
            else
            {
                statement.Accept(this);
            }
        }
    }

    private bool IsBuiltInFunction(string name)
    {
        string[] builtInFunctions =
        {
            "abs", "min", "max", "round",
            "len", "getsymbol", "tostring",
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
}