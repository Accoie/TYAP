using Ast.Statements;
using Ast.Types;

namespace Ast.Declaration;

/// <summary>
/// Объявляет параметр встроенной функции.
/// </summary>
public class BuiltInFunctionParameterDeclaration : AbstractParameterDeclaration
{
    public BuiltInFunctionParameterDeclaration(string name, Runtime.ValueType type)
        : base(name)
    {
        ResultType = new ScalarTypeNode(type);
    }

    public override void Accept(IAstVisitor visitor)
    {
        throw new InvalidOperationException($"Visitor cannot be applied to {GetType()}");
    }
}