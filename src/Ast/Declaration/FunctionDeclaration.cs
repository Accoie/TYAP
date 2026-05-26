using Ast.Statements;
using Ast.Types;

using ValueType = Runtime.ValueType;

namespace Ast.Declaration;

public sealed class FunctionDeclaration : AbstractFunctionDeclaration
{
    public FunctionDeclaration(string name, List<ParameterDeclaration> parameters, BlockStatement body, ValueType type)
        : base(name, parameters)
    {
        Body = body;
        ResultType = new ScalarTypeNode(type);
    }

    public ValueType ReturnType => ((ScalarTypeNode)ResultType).Type;

    public BlockStatement Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}