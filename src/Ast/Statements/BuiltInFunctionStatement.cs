using Runtime;

namespace Ast.Statements;

/// <summary>
/// Определение встроенной функции языка.
/// </summary>
public sealed class BuiltInFunctionStatement : AbstractFunctionDeclaration
{
    public BuiltInFunctionStatement(
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