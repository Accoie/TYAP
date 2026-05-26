using Ast.Expressions;

using ValueType = Runtime.ValueType;

namespace Ast.Types;

/// <summary>
/// Тип данных в языке Pascal++.
/// </summary>
public abstract class TypeNode
{
    public abstract ValueType GetScalarOrElementType();

    public bool IsArray => this is ArrayTypeNode;
}

/// <summary>
/// Скалярный тип: integer, float или string.
/// </summary>
public sealed class ScalarTypeNode : TypeNode
{
    public ScalarTypeNode(ValueType type)
    {
        Type = type;
    }

    public ValueType Type { get; }

    public override ValueType GetScalarOrElementType() => Type;
}

/// <summary>
/// Тип массива: arr[size] of elementType.
/// </summary>
public sealed class ArrayTypeNode : TypeNode
{
    public ArrayTypeNode(Expression size, ValueType elementType)
    {
        Size = size;
        ElementType = elementType;
    }

    /// <summary>
    /// Выражение, задающее размер массива.
    /// </summary>
    public Expression Size { get; }

    public ValueType ElementType { get; }

    public override ValueType GetScalarOrElementType() => ElementType;
}
