using Ast.Expressions;

namespace Ast.Statements;

public class FunctionCallStatement : Statement
{
    public FunctionCallStatement(FunctionCallExpression expression)
    {
        Expression = expression;
    }

    public FunctionCallExpression Expression { get; }

    public IReadOnlyList<Expression> Arguments => Expression.Arguments;

    public AbstractFunctionDeclaration Function
    {
        get => Expression.Function;
        set => Expression.Function = value;
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}