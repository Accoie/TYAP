using Ast.Expressions;

namespace Ast.Statements;

public class AssignmentStatement : Statement
{
    public AssignmentStatement(Expression target, Expression value)
    {
        Target = target;
        Value = value;
    }

    /// <summary>
    /// Lvalue: переменная или доступ к элементу массива.
    /// </summary>
    public Expression Target { get; }

    public Expression Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}