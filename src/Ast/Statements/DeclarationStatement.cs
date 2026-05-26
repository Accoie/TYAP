using Ast.Attributes;
using Ast.Types;

namespace Ast.Statements;

public abstract class DeclarationStatement : Statement
{
    private AstAttribute<TypeNode> _resultType;

    /// <summary>
    /// Тип объявления.
    /// </summary>
    public TypeNode ResultType
    {
        get => _resultType.Get();

        set => _resultType.Set(value);
    }
}