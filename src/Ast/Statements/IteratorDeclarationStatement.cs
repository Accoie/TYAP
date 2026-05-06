using Ast.Expressions;

namespace Ast.Statements;

public class IteratorDeclarationStatement : AbstractVariableDeclaration
{
    public IteratorDeclarationStatement(string name, Expression startValue)
        : base(name)
    {
        StartValue = startValue;
    }

    public Expression StartValue { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}