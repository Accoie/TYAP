using Ast.Expressions;
using Ast.Statements;

using ValueType = Runtime.ValueType;

namespace Ast.Declaration;

public sealed class VariableDeclaration : AbstractVariableDeclaration
{
    public VariableDeclaration(string name, ValueType type, Expression? value)
        : base(name)
    {
        Value = value;
        DeclaredType = type;
    }

    public ValueType DeclaredType { get; }

    public Expression? Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}