using Ast.Attributes;
using Ast.Types;

namespace Ast.Expressions;

/// <summary>
/// Литерал массива: [ expr, expr, ... ].
/// </summary>
public sealed class ArrayLiteralExpression : Expression
{
    private AstAttribute<ArrayTypeNode> _inferredArrayType;

    public ArrayLiteralExpression(IReadOnlyList<Expression> elements)
    {
        Elements = elements;
    }

    public IReadOnlyList<Expression> Elements { get; }

    /// <summary>
    /// Тип массива, выведенный из литерала (заполняется семантическим анализом).
    /// </summary>
    public ArrayTypeNode InferredArrayType
    {
        get => _inferredArrayType.Get();
        set => _inferredArrayType.Set(value);
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
