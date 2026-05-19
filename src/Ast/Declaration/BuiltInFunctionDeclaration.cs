using Ast.Statements;

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
        ResultType = resultType;
    }

    public override void Accept(IAstVisitor visitor)
    {
        throw new InvalidOperationException($"Visitor cannot be applied to {GetType()}");
    }
}