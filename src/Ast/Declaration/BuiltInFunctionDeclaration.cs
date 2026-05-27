using Ast.Types;

namespace Ast.Declaration;

/// <summary>
/// Определение встроенной функции языка.
/// </summary>
public sealed class BuiltInFunctionDeclaration : AbstractFunctionDeclaration
{
    public BuiltInFunctionDeclaration(
        string name,
        IReadOnlyList<BuiltInFunctionParameterDeclaration> parameters,
        Runtime.ValueType resultType
    )
        : base(name, parameters)
    {
        ResultType = new ScalarTypeNode(resultType);
    }

    public Runtime.ValueType ReturnType => ((ScalarTypeNode)ResultType).Type;

    public override void Accept(IAstVisitor visitor)
    {
        throw new InvalidOperationException($"Visitor cannot be applied to {GetType()}");
    }
}