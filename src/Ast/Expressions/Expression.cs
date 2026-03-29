using Ast.Attributes;

namespace Ast.Expressions;

public abstract class Expression : AstNode
{
    private AstAttribute<Runtime.ValueType> _resultType;

    /// <summary>
    /// Тип результата выражения.
    /// </summary>
    public Runtime.ValueType ResultType
    {
        get => _resultType.Get();

        set => _resultType.Set(value);
    }
}