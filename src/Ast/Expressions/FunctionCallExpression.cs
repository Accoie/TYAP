using Ast.Attributes;
using Ast.Statements;

namespace Ast.Expressions;

public class FunctionCallExpression : Expression
{
    private readonly List<Expression> _arguments;
    private AstAttribute<AbstractFunctionDeclaration> _function;

    public FunctionCallExpression(string name, List<Expression> arguments)
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