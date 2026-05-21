using Ast.Statements;
using Ast.Types;

namespace Ast.BuiltIn;

/// <summary>
/// Определение встроенной функции языка.
/// </summary>
public sealed class BuiltInFunction : AbstractFunctionDeclaration
{
    public BuiltInFunction(
        string name,
        IReadOnlyList<BuiltInFunctionParameter> parameters,
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