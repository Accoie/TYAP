using Ast.Expressions;
using Ast.Statements;

namespace Ast;

public interface IAstVisitor
{
    void Visit(LiteralExpression e);

    void Visit(OutputStatement s);

    void Visit(BlockStatement s);
}