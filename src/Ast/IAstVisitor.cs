using Ast.Expressions;
using Ast.Statements;

namespace Ast;

public interface IAstVisitor
{
    void Visit(BinaryOperationExpression e);

    void Visit(UnaryOperationExpression e);

    void Visit(LiteralExpression e);

    void Visit(AssignmentStatement s);

    void Visit(InputStatement s);

    void Visit(OutputStatement s);

    void Visit(BlockStatement s);

    void Visit(VariableDeclarationStatement s);

    void Visit(VariableExpression variableExpression);
}