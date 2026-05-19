using Ast.Declaration;
using Ast.Expressions;
using Ast.Statements;

using Semantics.Exceptions;

namespace Semantics.Passes;

/// <summary>
/// Проверяет соблюдение контекстно-зависимых правил языка.
/// </summary>
public sealed class CheckContextSensitiveRulesPass : AbstractPass
{
    private readonly Stack<ExpressionContext> _expressionContextStack;

    public CheckContextSensitiveRulesPass()
    {
        _expressionContextStack = new Stack<ExpressionContext>();
        _expressionContextStack.Push(ExpressionContext.Default);
    }

    private enum ExpressionContext
    {
        Default,
        InsideLoop,
        InsideFunction,
    }

    /// <summary>
    /// Проверяет корректность программы с точки зрения использования функций.
    /// </summary>
    public override void Visit(FunctionCallExpression e)
    {
        base.Visit(e);

        if (e.Arguments.Count != e.Function.Parameters.Count)
        {
            throw new InvalidFunctionCallException(
                $"Function '{e.Name}' expects {e.Function.Parameters.Count} arguments, " +
                $"but got {e.Arguments.Count}"
            );
        }
    }

    public override void Visit(FunctionCallStatement s)
    {
        s.Expression.Accept(this);
    }

    public override void Visit(ReturnStatement s)
    {
        if (!_expressionContextStack.Contains(ExpressionContext.InsideFunction))
        {
            throw new InvalidExpressionException("'return' cannot be outside the function block.");
        }

        base.Visit(s);
    }

    public override void Visit(FunctionDeclaration d)
    {
        _expressionContextStack.Push(ExpressionContext.InsideFunction);
        try
        {
            base.Visit(d);
        }
        finally
        {
            _expressionContextStack.Pop();
        }
    }

    public override void Visit(WhileLoopStatement e)
    {
        _expressionContextStack.Push(ExpressionContext.InsideLoop);
        try
        {
            base.Visit(e);
        }
        finally
        {
            _expressionContextStack.Pop();
        }
    }

    public override void Visit(ForLoopStatement e)
    {
        _expressionContextStack.Push(ExpressionContext.InsideLoop);
        try
        {
            base.Visit(e);
        }
        finally
        {
            _expressionContextStack.Pop();
        }
    }

    public override void Visit(BreakStatement e)
    {
        base.Visit(e);

        if (_expressionContextStack.Peek() != ExpressionContext.InsideLoop)
        {
            throw new InvalidExpressionException("The \"break\" expression is allowed only inside the loop");
        }
    }

    public override void Visit(ContinueStatement e)
    {
        base.Visit(e);

        if (_expressionContextStack.Peek() != ExpressionContext.InsideLoop)
        {
            throw new InvalidExpressionException("The \"continue\" expression is allowed only inside the loop");
        }
    }
}