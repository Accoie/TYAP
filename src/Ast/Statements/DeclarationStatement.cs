using Ast.Attributes;

using ValueType = Runtime.ValueType;

namespace Ast.Statements;

public abstract class DeclarationStatement : Statement
{
    private AstAttribute<ValueType> resultType;

    /// <summary>
    ///     Тип результата объявления.
    /// </summary>
    public ValueType ResultType
    {
        get => resultType.Get();

        set => resultType.Set(value);
    }
}