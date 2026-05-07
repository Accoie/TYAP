namespace Ast.Statements;

/// <summary>
/// Объявление параметра функции.
/// </summary>
public class ParameterDeclarationStatement : AbstractParameterDeclaration
{
    public ParameterDeclarationStatement(string name, Runtime.ValueType type)
        : base(name)
    {
        ResultType = type;
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}