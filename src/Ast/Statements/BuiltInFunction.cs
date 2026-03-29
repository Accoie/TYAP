using Runtime;

namespace Ast.Statements;

/// <summary>
/// Определение встроенной функции языка.
/// </summary>
public sealed class BuiltInFunction : AbstractFunctionDeclaration
{
    private readonly Func<IReadOnlyList<Value>, Value> _implementation;

    public BuiltInFunction(
        string name,
        IReadOnlyList<BuiltInFunctionParameter> parameters,
        Runtime.ValueType resultType,
        Func<IReadOnlyList<Value>, Value> implementation
    )
        : base(name, parameters)
    {
        ResultType = resultType;
        this._implementation = implementation;
    }

    public Value Invoke(IReadOnlyList<Value> arguments)
    {
        return _implementation(arguments);
    }

    public override void Accept(IAstVisitor visitor)
    {
        throw new InvalidOperationException($"Visitor cannot be applied to {GetType()}");
    }
}