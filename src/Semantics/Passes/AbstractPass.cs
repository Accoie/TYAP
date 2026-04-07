using Ast;
using Ast.Expressions;
using Ast.Statements;

namespace Semantics.Passes;

/// <summary>
/// Базовый класс для проходов по AST с целью вычисления атрибутов и семантических проверок.
/// </summary>
public abstract class AbstractPass : IAstVisitor
{
    public virtual void Visit(LiteralExpression e)
    {
    }

    public virtual void Visit(BinaryOperationExpression e)
    {
        e.Left.Accept(this);
        e.Right.Accept(this);
    }

    public virtual void Visit(UnaryOperationExpression e)
    {
        e.Operand.Accept(this);
    }

    public virtual void Visit(VariableExpression e)
    {
    }

    public virtual void Visit(AssignmentStatement s)
    {
        s.Value.Accept(this);
    }

    public virtual void Visit(InputStatement s)
    {
    }

    public virtual void Visit(OutputStatement s)
    {
        foreach (Expression arg in s.Arguments)
        {
            arg.Accept(this);
        }
    }

    public virtual void Visit(BlockStatement s)
    {
        foreach (Statement statement in s.Statements)
        {
            statement.Accept(this);
        }
    }

    public virtual void Visit(VariableDeclarationStatement s)
    {
        s.Value?.Accept(this);
    }
}