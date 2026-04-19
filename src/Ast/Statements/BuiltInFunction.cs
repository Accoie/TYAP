using Runtime;

namespace Ast.Statements;

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
        ResultType = resultType;
    }

    public override void Accept(IAstVisitor visitor)
    {
        throw new InvalidOperationException($"Visitor cannot be applied to {GetType()}");
    }
}