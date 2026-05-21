using Ast.BuiltIn;
using Ast.Declaration;
using Ast.Expressions;
using Ast.Statements;

using Semantics.Exceptions;
using Semantics.Symbols;

namespace Semantics.Passes;

public sealed class ResolveNamesPass : AbstractPass
{
    private readonly Stack<FunctionDeclaration> _functionStack;
    private SymbolsTable _symbols;

    public ResolveNamesPass(SymbolsTable globalSymbols)
    {
        _symbols = globalSymbols;
        _functionStack = new Stack<FunctionDeclaration>();
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
            throw new UnknownSymbolException(e.Name);
        }
    }

    public override void Visit(InputStatement s)
    {
        base.Visit(s);
    }

    public override void Visit(FunctionCallExpression e)
    {
        base.Visit(e);

        if (!IsBuiltInFunction(e.Name))
        {
            DeclarationStatement symbol = _symbols.GetSymbol(e.Name);

            if (symbol is FunctionDeclaration function)
            {
                if (e.Arguments.Count != function.Parameters.Count)
                {
                    throw new InvalidFunctionCallException(
                        $"Function '{e.Name}' expects {function.Parameters.Count} arguments, " +
                        $"but got {e.Arguments.Count}"
                    );
                }

                e.Function = function;
            }
            else
            {
                throw new InvalidSymbolException(e.Name, "variable");
            }
        }
        else
        {
            e.Function = (BuiltInFunction)_symbols.GetSymbol(e.Name);
        }
    }

    public override void Visit(FunctionCallStatement s)
    {
        s.Expression.Accept(this);
    }

    public override void Visit(BlockStatement s)
    {
        if (s.IsNewScope)
        {
            _symbols = new SymbolsTable(_symbols);
        }

        try
        {
            PredeclareFunctions(s.Statements);
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

    public override void Visit(VariableDeclaration d)
    {
        base.Visit(d);

        if (IsBuiltInFunction(d.Name) || IsBuiltInType(d.Name))
        {
            throw DuplicateSymbolException.DuplicateVariableOrFunction(d.Name);
        }

        _symbols.DefineSymbol(d.Name, d);
    }

    public override void Visit(FunctionDeclaration d)
    {
        if (IsBuiltInFunction(d.Name) || IsBuiltInType(d.Name))
        {
            throw DuplicateSymbolException.DuplicateVariableOrFunction(d.Name);
        }

        SymbolsTable outerScope = _symbols;
        _symbols = new SymbolsTable(null);
        try
        {
            _symbols.DefineSymbol(d.Name, d);

            foreach (AbstractParameterDeclaration abstractParameterDeclaration in d.Parameters)
            {
                ParameterDeclaration parameter = (ParameterDeclaration)abstractParameterDeclaration;
                parameter.Accept(this);
            }

            _functionStack.Push(d);
            d.Body.Accept(this);
            _functionStack.Pop();
        }
        finally
        {
            _symbols = outerScope;
        }
    }

    public override void Visit(ParameterDeclaration d)
    {
        base.Visit(d);

        if (IsBuiltInFunction(d.Name) || IsBuiltInType(d.Name))
        {
            throw DuplicateSymbolException.DuplicateVariableOrFunction(d.Name);
        }

        try
        {
            _symbols.DefineSymbol(d.Name, d);
        }
        catch (DuplicateSymbolException)
        {
            throw DuplicateSymbolException.DuplicateVariableOrFunction(d.Name);
        }
    }

    public override void Visit(ForLoopStatement s)
    {
        _symbols = new SymbolsTable(_symbols);
        try
        {
            base.Visit(s);
        }
        finally
        {
            _symbols = _symbols.Parent!;
        }
    }

    public override void Visit(IteratorDeclaration e)
    {
        base.Visit(e);

        _symbols.DefineSymbol(e.Name, e);
    }

    public override void Visit(WhileLoopStatement s)
    {
        _symbols = new SymbolsTable(_symbols );
        try
        {
            base.Visit(s);
        }
        finally
        {
            _symbols = _symbols.Parent!;
        }
    }

    public override void Visit(IfElseStatement s)
    {
        s.Condition.Accept(this);

        _symbols = new SymbolsTable(_symbols);
        try
        {
            s.ThenBranch.Accept(this);
        }
        finally
        {
            _symbols = _symbols.Parent!;
        }

        if (s.ElseBranch != null)
        {
            _symbols = new SymbolsTable(_symbols);
            try
            {
                s.ElseBranch.Accept(this);
            }
            finally
            {
                _symbols = _symbols.Parent!;
            }
        }
    }

    public override void Visit(AssignmentStatement s)
    {
        if (s.Target is VariableExpression variableTarget)
        {
            DeclarationStatement symbol = _symbols.GetSymbol(variableTarget.Name);

            if (symbol is FunctionDeclaration)
            {
                throw new InvalidAssignmentException(
                    $"Cannot assign to function '{variableTarget.Name}'"
                );
            }

            if (symbol is not AbstractVariableDeclaration variable)
            {
                throw new InvalidAssignmentException(
                    $"Invalid assignment to '{variableTarget.Name}'"
                );
            }

            if (variable is IteratorDeclaration)
            {
                throw new InvalidAssignmentException(
                    $"Cannot assign to for loop iterator '{variableTarget.Name}'"
                );
            }

            variableTarget.Variable = variable;
        }
        else
        {
            s.Target.Accept(this);
        }

        s.Value.Accept(this);
    }

    public override void Visit(ReturnStatement s)
    {
        base.Visit(s);

        if (_functionStack.Count == 0)
        {
            throw new InvalidOperationException("Return statement cannot be outside of function");
        }
    }

    public override void Visit(BreakStatement s)
    {
        base.Visit(s);

        if (_functionStack.Count > 0)
        {
            throw new InvalidExpressionException("Break statement cannot be used inside a function");
        }
    }

    public override void Visit(ContinueStatement s)
    {
        base.Visit(s);

        if (_functionStack.Count > 0)
        {
            throw new InvalidExpressionException("Continue statement cannot be used inside a function");
        }
    }

    private void PredeclareFunctions(IEnumerable<Statement> statements)
    {
        foreach (Statement statement in statements)
        {
            if (statement is FunctionDeclaration function)
            {
                try
                {
                    _symbols.DefineSymbol(function.Name, function);
                }
                catch (DuplicateSymbolException)
                {
                    throw DuplicateSymbolException.DuplicateVariableOrFunction(function.Name);
                }
            }
        }
    }

    private void ProcessDeclarationsAndStatements(IEnumerable<Statement> statements)
    {
        foreach (Statement statement in statements)
        {
            if (statement is VariableDeclaration variable)
            {
                variable.Accept(this);
            }
        }

        foreach (Statement statement in statements)
        {
            if (statement is VariableDeclaration variable)
            {
            }
            else if (statement is FunctionDeclaration function)
            {
                function.Accept(this);
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
            "abs_f", "min_f", "max_f", "round", "len", "getsymbol", "tostring_i", "tostring_f",
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

    private bool IsBuiltInType(string name)
    {
        string[] builtInTypes = { "integer", "float", "string", "arr" };

        foreach (string type in builtInTypes)
        {
            if (string.Equals(type, name))
            {
                return true;
            }
        }

        return false;
    }
}