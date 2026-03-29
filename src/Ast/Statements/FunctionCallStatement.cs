using Ast.Attributes;
using Ast.Expressions;

namespace Ast.Statements;

public class FunctionCallStatement : Statement
{
    private readonly List<Expression> _arguments;
    private AstAttribute<AbstractFunctionDeclaration> _function;

    public FunctionCallStatement(string name, List<Expression> arguments)
    {
        Name = name;
        this._arguments = arguments;
    }

    public string Name { get; }

    public AbstractFunctionDeclaration Function
    {
        get => _function.Get();
        set => _function.Set(value);
    }

    public IReadOnlyList<Expression> Arguments => _arguments;

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}