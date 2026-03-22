using Ast.Expressions;

using ValueType = Runtime.ValueType;

namespace Ast.Statements;

public class ReturnStatement : Statement
{
    public ReturnStatement(Expression? value, ValueType type)
    {
        Value = value;
        Type = type;
    }

    public Expression? Value { get; }

    public ValueType Type { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}