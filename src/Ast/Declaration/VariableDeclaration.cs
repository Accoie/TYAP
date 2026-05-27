using Ast.Expressions;
using Ast.Types;

namespace Ast.Declaration;

public sealed class VariableDeclaration : AbstractVariableDeclaration
{
    public VariableDeclaration(string name, TypeNode type, Expression? value)
        : base(name)
    {
        Value = value;
        DeclaredType = type;
    }

    public TypeNode DeclaredType { get; }

    public Expression? Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
