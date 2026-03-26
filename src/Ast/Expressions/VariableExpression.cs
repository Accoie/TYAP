using Ast.Attributes;
using Ast.Statements;

namespace Ast.Expressions;

public sealed class VariableExpression : Expression
{
    private AstAttribute<AbstractVariableDeclaration> _variable;

    public VariableExpression(string name)
    {
        Name = name;
    }

    public AbstractVariableDeclaration Variable
    {
        get => _variable.Get();
        set => _variable.Set(value);
    }

    public string Name { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}