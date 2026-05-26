using Ast.Declaration;
using Ast.Expressions;
using Ast.Statements;

using Semantics.Exceptions;

namespace Semantics.Passes;

public sealed class CheckContextSensitiveRulesPass : AbstractPass
{
    private readonly Stack<ExpressionContext> expressionContextStack;

    public CheckContextSensitiveRulesPass()
    {
        expressionContextStack = [];
        expressionContextStack.Push(ExpressionContext.Default);
    }

    private enum ExpressionContext
    {
        Default,
        InsideLoop,
        InsideFunction,
    }

    public override void Visit(ReturnStatement s)
    {
        if (!expressionContextStack.Contains(ExpressionContext.InsideFunction))
        {
            throw new InvalidExpressionException("'return' cannot be outside the function block.");
        }

        base.Visit(s);
    }

    public override void Visit(FunctionDeclaration d)
    {
        expressionContextStack.Push(ExpressionContext.InsideFunction);
        try
        {
            base.Visit(d);
            BlockStatement body = d.Body;
        }
        finally
        {
            expressionContextStack.Pop();
        }
    }

    public override void Visit(WhileLoopStatement e)
    {
        expressionContextStack.Push(ExpressionContext.InsideLoop);
        try
        {
            base.Visit(e);
        }
        finally
        {
            expressionContextStack.Pop();
        }
    }

    public override void Visit(ForLoopStatement e)
    {
        expressionContextStack.Push(ExpressionContext.InsideLoop);
        try
        {
            base.Visit(e);
        }
        finally
        {
            expressionContextStack.Pop();
        }
    }

    public override void Visit(BreakStatement e)
    {
        base.Visit(e);

        if (expressionContextStack.Peek() != ExpressionContext.InsideLoop)
        {
            throw new InvalidExpressionException("The \"breakout\" expression is allowed only inside the loop");
        }
    }

    public override void Visit(ContinueStatement e)
    {
        base.Visit(e);

        if (expressionContextStack.Peek() != ExpressionContext.InsideLoop)
        {
            throw new InvalidExpressionException("The \"contra\" expression is allowed only inside the loop");
        }
    }
}