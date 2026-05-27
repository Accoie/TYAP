using Ast.Types;

namespace Ast.Declaration;

/// <summary>
/// Объявление параметра функции.
/// </summary>
public class ParameterDeclaration : AbstractParameterDeclaration
{
    public ParameterDeclaration(string name, TypeNode type)
        : base(name)
    {
        ResultType = type;
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
