using Ast.Expressions;

namespace Ast.Statements;

public class InputStatement : Statement
{
    public InputStatement(Expression target)
    {
        Target = target;
    }

    /// <summary>
    /// Lvalue: переменная или элемент массива.
    /// </summary>
    public Expression Target { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}