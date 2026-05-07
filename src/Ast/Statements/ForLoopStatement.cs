using Ast.Expressions;

namespace Ast.Statements;

public sealed class ForLoopStatement : Statement
{
    public ForLoopStatement(
        IteratorDeclarationStatement iterator,
        Expression endCondition,
        BlockStatement body
    )
    {
        EndValue = endCondition;
        Body = body;
        Iterator = iterator;
    }

    public Expression EndValue { get; }

    public BlockStatement Body { get; }

    public IteratorDeclarationStatement Iterator { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}