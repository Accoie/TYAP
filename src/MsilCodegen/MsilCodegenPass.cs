using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

using Ast;
using Ast.Expressions;
using Ast.Statements;

using ValueType = Runtime.ValueType;

namespace MsilCodegen;

public class MsilCodegenPass : IAstVisitor
{
    private readonly ModuleBuilder _moduleBuilder;
    private readonly TypeMapper _typeMapper;
    private readonly BuiltinFunctionEmitter _builtinFunctionEmitter;

    /// <summary>
    /// Тип Program будущей программы.
    /// </summary>
    private TypeBuilder _programTypeBuilder = null!;

    /// <summary>
    /// Генератор инструкций для текущего метода.
    /// </summary>
    private ILGenerator _il = null!;

    /// <summary>
    /// Стек областей видимости переменных.
    /// </summary>
    private readonly Stack<Dictionary<string, LocalBuilder>> _scopesStack;

    /// <summary>
    /// Стек меток конца цикла для прерывания цикла (break).
    /// </summary>
    private readonly Stack<Label> _loopBreaksStack;

    /// <summary>
    /// Стек меток цикла для продолжения цикла (continue).
    /// </summary>
    private readonly Stack<Label> _loopContinuesStack;

    /// <summary>
    /// Словарь методов, соответствующих пользовательским функциям исходной программы.
    /// </summary>
    private readonly Dictionary<string, MethodBuilder> _userFunctionMethodsMap;

    public MsilCodegenPass(ModuleBuilder moduleBuilder)
    {
        _moduleBuilder = moduleBuilder;
        _typeMapper = new TypeMapper();
        _builtinFunctionEmitter = new BuiltinFunctionEmitter();
        _scopesStack = new Stack<Dictionary<string, LocalBuilder>>();
        _loopBreaksStack = new Stack<Label>();
        _loopContinuesStack = new Stack<Label>();
        _userFunctionMethodsMap = new Dictionary<string, MethodBuilder>();
    }

    /// <summary>
    /// Текущая область видимости переменных.
    /// </summary>
    private Dictionary<string, LocalBuilder> CurrentScope => _scopesStack.Peek();

    public MethodBuilder GenerateProgramCode(BlockStatement program)
    {
        _programTypeBuilder = _moduleBuilder.DefineType(
            "Program",
            TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class
        );

        MethodBuilder mainMethod = DefineProgramClassMethod("Main", typeof(void), Type.EmptyTypes);
        _il = mainMethod.GetILGenerator();

        // Устанавливаем кодировку UTF-8 для консоли
        MethodInfo utf8Encoding = typeof(Encoding).GetProperty("UTF8")!.GetMethod!;
        MethodInfo outputEncodingSetter = typeof(Console).GetProperty("OutputEncoding")!.SetMethod!;
        _il.Emit(OpCodes.Call, utf8Encoding);
        _il.Emit(OpCodes.Call, outputEncodingSetter);

        BeginScope();

        program.Accept(this);

        _il.Emit(OpCodes.Ret);

        EndScope();

        _programTypeBuilder.CreateType();

        return mainMethod;
    }

    public void Visit(BinaryOperationExpression e)
    {
        if (e.Left.ResultType == ValueType.String && e.Right.ResultType == ValueType.String)
        {
            EmitStringsBinaryOperation(e);
        }
        else
        {
            EmitIntegersOrFloatBinaryOperation(e);
        }
    }

    public void Visit(UnaryOperationExpression e)
    {
        e.Operand.Accept(this);

        switch (e.Operation)
        {
            case UnaryOperation.Minus:
                if (e.ResultType != ValueType.Integer && e.ResultType != ValueType.Float)
                {
                    throw new InvalidOperationException(
                        $"Unary minus expect number, but got {e.ResultType}");
                }

                _il.Emit(OpCodes.Neg);
                break;

            case UnaryOperation.Not:
                EmitLogicalNot();
                break;

            default:
                throw new NotSupportedException();
        }
    }

    public void Visit(LiteralExpression e)
    {
        if (e.ResultType == ValueType.Integer)
        {
            _il.Emit(OpCodes.Ldc_I4, e.Value.AsInteger());
        }
        else if (e.ResultType == ValueType.String)
        {
            _il.Emit(OpCodes.Ldstr, e.Value.AsString());
        }
        else if (e.ResultType == ValueType.Float)
        {
            _il.Emit(OpCodes.Ldc_R8, e.Value.AsFloat());
        }
        else
        {
            throw new InvalidOperationException($"Literal of type '{e.ResultType}' is not supported");
        }
    }

    public void Visit(AssignmentStatement s)
    {
        s.Value.Accept(this);

        foreach (Dictionary<string, LocalBuilder> scope in _scopesStack)
        {
            if (scope.TryGetValue(s.Name, out LocalBuilder? local))
            {
                _il.Emit(OpCodes.Stloc, local);
                return;
            }
        }

        throw new InvalidOperationException(
            $"Variable '{s.Name}' is not found in current scope"
        );
    }

    public void Visit(InputStatement s)
    {
        LocalBuilder? local = null;
        foreach (Dictionary<string, LocalBuilder> scope in _scopesStack)
        {
            if (scope.TryGetValue(s.VariableName, out LocalBuilder? foundLocal))
            {
                local = foundLocal;
                break;
            }
        }

        if (local == null)
        {
            throw new InvalidOperationException(
                $"Variable '{s.VariableName}' is not found in current scope"
            );
        }

        MethodInfo readLineMethod = GetMethod(typeof(Console), "ReadLine", Type.EmptyTypes);
        _il.Emit(OpCodes.Call, readLineMethod);

        Type variableType = local.LocalType;

        if (variableType == typeof(string))
        {
        }
        else if (variableType == typeof(int))
        {
            MethodInfo parseIntMethod = GetMethod(typeof(int), "Parse", [typeof(string)]);
            _il.Emit(OpCodes.Call, parseIntMethod);
        }
        else if (variableType == typeof(double))
        {
            MethodInfo parseDoubleMethod = GetMethod(typeof(double), "Parse", [typeof(string)]);
            _il.Emit(OpCodes.Call, parseDoubleMethod);
        }
        else
        {
            throw new InvalidOperationException($"Input for type '{variableType}' is not supported");
        }

        _il.Emit(OpCodes.Stloc, local);
    }

    public void Visit(OutputStatement s)
    {
        foreach (Expression argument in s.Arguments)
        {
            argument.Accept(this);

            if (argument.ResultType == ValueType.Float)
            {
                WriteFloat();
            }
            else
            {
                Type argType = argument.ResultType switch
                {
                    ValueType.Integer => typeof(int),
                    ValueType.String => typeof(string),
                    _ => throw new InvalidOperationException($"Output type '{argument.ResultType}' is not supported"),
                };

                MethodInfo writeMethod = GetMethod(typeof(Console), "Write", [argType]);
                _il.Emit(OpCodes.Call, writeMethod);
            }
        }

        MethodInfo writelnMethod = GetMethod(typeof(Console), "WriteLine", Type.EmptyTypes);
        _il.Emit(OpCodes.Call, writelnMethod);
    }

    public void Visit(BlockStatement s)
    {
        if (s.IsNewScope)
        {
            BeginScope();
            try
            {
                foreach (Statement statement in s.Statements)
                {
                    statement.Accept(this);
                }
            }
            finally
            {
                EndScope();
            }
        }
        else
        {
            foreach (Statement statement in s.Statements)
            {
                statement.Accept(this);
            }
        }
    }

    public void Visit(VariableDeclarationStatement s)
    {
        Type ilType = s.DeclaredType switch
        {
            ValueType.Integer => typeof(int),
            ValueType.Float => typeof(double),
            ValueType.String => typeof(string),
            _ => throw new InvalidOperationException($"Type {s.DeclaredType} is not supported"),
        };

        LocalBuilder local = _il.DeclareLocal(ilType);

        CurrentScope[s.Name] = local;

        if (s.Value != null)
        {
            s.Value.Accept(this);
            _il.Emit(OpCodes.Stloc, local);
        }
    }

    public void Visit(VariableExpression e)
    {
        foreach (Dictionary<string, LocalBuilder> scope in _scopesStack)
        {
            if (scope.TryGetValue(e.Name, out LocalBuilder? local))
            {
                _il.Emit(OpCodes.Ldloc, local);
                return;
            }
        }

        throw new InvalidOperationException(
            $"Variable '{e.Name}' is not found in current scope"
        );
    }

    public void Visit(FunctionCallExpression s)
    {
        foreach (Expression argument in s.Arguments)
        {
            argument.Accept(this);
        }

        if (s.Function is BuiltInFunctionStatement)
        {
            _builtinFunctionEmitter.EmitCallBuiltinFunction(s.Name, _il);
        }
        else
        {
            if (!_userFunctionMethodsMap.TryGetValue(s.Name, out MethodBuilder? method))
            {
                throw new InvalidOperationException($"Cannot find method of .NET for function with name {s.Name}");
            }

            _il.Emit(OpCodes.Call, method);
        }
    }

    public void Visit(IfElseStatement s)
    {
        Label endLabel = _il.DefineLabel();
        if (s.ElseBranch != null)
        {
            Label elseLabel = _il.DefineLabel();
            s.Condition.Accept(this);
            _il.Emit(OpCodes.Brfalse, elseLabel);

            s.ThenBranch.Accept(this);
            _il.Emit(OpCodes.Br, endLabel);

            _il.MarkLabel(elseLabel);
            s.ElseBranch.Accept(this);
        }
        else
        {
            s.Condition.Accept(this);
            _il.Emit(OpCodes.Brfalse, endLabel);
            s.ThenBranch.Accept(this);
        }

        _il.MarkLabel(endLabel);
    }

    public void Visit(WhileLoopStatement e)
    {
        Label loopStart = _il.DefineLabel();
        Label loopEnd = _il.DefineLabel();

        _loopBreaksStack.Push(loopEnd);
        _loopContinuesStack.Push(loopStart);

        _il.MarkLabel(loopStart);
        e.Condition.Accept(this);
        _il.Emit(OpCodes.Brfalse, loopEnd);

        e.Body.Accept(this);
        _il.Emit(OpCodes.Br, loopStart);

        _il.MarkLabel(loopEnd);

        _loopBreaksStack.Pop();
        _loopContinuesStack.Pop();
    }

    public void Visit(ForLoopStatement e)
    {
        Label loopStart = _il.DefineLabel();
        Label loopEnd = _il.DefineLabel();
        Label loopIncrement = _il.DefineLabel();

        _loopBreaksStack.Push(loopEnd);
        _loopContinuesStack.Push(loopIncrement);

        BeginScope();

        ValueType iteratorType = e.Iterator.StartValue.ResultType;
        Type ilIteratorType = _typeMapper.MapType(iteratorType);
        LocalBuilder iterator = _il.DeclareLocal(ilIteratorType);
        CurrentScope[e.Iterator.Name] = iterator;

        e.Iterator.StartValue.Accept(this);
        _il.Emit(OpCodes.Stloc, iterator);

        _il.MarkLabel(loopStart);
        _il.Emit(OpCodes.Ldloc, iterator);
        e.EndValue.Accept(this);
        EmitConvertToCommonType(iteratorType, e.EndValue.ResultType);
        _il.Emit(OpCodes.Cgt);
        _il.Emit(OpCodes.Brtrue, loopEnd);

        e.Body.Accept(this);

        _il.MarkLabel(loopIncrement);
        _il.Emit(OpCodes.Ldloc, iterator);
        if (iteratorType == ValueType.Float)
        {
            _il.Emit(OpCodes.Ldc_R8, 1.0);
        }
        else
        {
            _il.Emit(OpCodes.Ldc_I4_1);
        }

        _il.Emit(OpCodes.Add);
        _il.Emit(OpCodes.Stloc, iterator);

        _il.Emit(OpCodes.Br, loopStart);

        _il.MarkLabel(loopEnd);

        _loopBreaksStack.Pop();
        _loopContinuesStack.Pop();
        EndScope();
    }

    public void Visit(IteratorDeclarationStatement d)
    {
    }

    public void Visit(ReturnStatement s)
    {
        s.Value?.Accept(this);

        _il.Emit(OpCodes.Ret);
    }

    public void Visit(BreakStatement e)
    {
        Label loopEnd = _loopBreaksStack.Peek();
        _il.Emit(OpCodes.Br, loopEnd);
    }

    public void Visit(ContinueStatement e)
    {
        if (_loopContinuesStack.Count == 0)
        {
            throw new InvalidOperationException("Statement 'continue' can only be used inside a loop");
        }

        Label continueLabel = _loopContinuesStack.Peek();
        _il.Emit(OpCodes.Br, continueLabel);
    }

    public void Visit(FunctionDeclarationStatement s)
    {
        MethodBuilder method = DefineProgramClassMethod(
            GetUserFunctionMethodName(s.Name),
            _typeMapper.MapType(s.ResultType),
            s.Parameters.Select(p => _typeMapper.MapType(p.ResultType)).ToArray()
        );
        _userFunctionMethodsMap[s.Name] = method;

        ILGenerator previousIl = _il;

        try
        {
            _il = method.GetILGenerator();
            BeginScope();

            for (int i = 0, iEnd = s.Parameters.Count; i < iEnd; ++i)
            {
                AbstractParameterDeclaration param = s.Parameters[i];
                EmitDefineParameter(param.Name, param.ResultType, i);
            }

            s.Body.Accept(this);

            _il.Emit(OpCodes.Ret);
        }
        finally
        {
            EndScope();
            _il = previousIl;
        }
    }

    public void Visit(FunctionCallStatement s)
    {
        s.Expression.Accept(this);

        if (s.Expression.ResultType != ValueType.Void)
        {
            _il.Emit(OpCodes.Pop);
        }
    }

    public void Visit(ParameterDeclarationStatement parameterDeclarationStatementStatement)
    {
    }

    /// <summary>
    /// Генерирует код возведения в степень.
    /// </summary>
    private void EmitPowerOperation(BinaryOperationExpression e)
    {
        MethodInfo mathPow = typeof(Math).GetMethod(
            "Pow",
            [typeof(double), typeof(double)]
        )!;

        e.Left.Accept(this);

        if (e.Left.ResultType == ValueType.Integer)
        {
            _il.Emit(OpCodes.Conv_R8);
        }

        e.Right.Accept(this);

        if (e.Right.ResultType == ValueType.Integer)
        {
            _il.Emit(OpCodes.Conv_R8);
        }

        _il.Emit(OpCodes.Call, mathPow);

        if (e.ResultType == ValueType.Integer)
        {
            _il.Emit(OpCodes.Conv_I4);
        }
    }

    /// <summary>
    /// Генерирует код вычисления бинарной операции над целыми и вещественными числами.
    /// </summary>
    private void EmitIntegersOrFloatBinaryOperation(BinaryOperationExpression e)
    {
        switch (e.Operation)
        {
            case BinaryOperation.Exponentiate:
                EmitPowerOperation(e);
                break;
            case BinaryOperation.And:
                EmitLogicalAnd(e);
                break;
            case BinaryOperation.Or:
                EmitLogicalOr(e);
                break;
            default:
                e.Left.Accept(this);
                e.Right.Accept(this);

                switch (e.Operation)
                {
                    case BinaryOperation.Add:
                        _il.Emit(OpCodes.Add);
                        break;
                    case BinaryOperation.Subtract:
                        _il.Emit(OpCodes.Sub);
                        break;
                    case BinaryOperation.Multiply:
                        _il.Emit(OpCodes.Mul);
                        break;
                    case BinaryOperation.Divide:
                        _il.Emit(OpCodes.Div);
                        break;
                    case BinaryOperation.Modulo:
                        _il.Emit(OpCodes.Rem);
                        break;
                    case BinaryOperation.Equal:
                        _il.Emit(OpCodes.Ceq);
                        break;
                    case BinaryOperation.NotEqual:
                        _il.Emit(OpCodes.Ceq);
                        EmitLogicalNot();
                        break;
                    case BinaryOperation.LessThan:
                        _il.Emit(OpCodes.Clt);
                        break;
                    case BinaryOperation.LessThanOrEqual:
                        _il.Emit(OpCodes.Cgt);
                        EmitLogicalNot();
                        break;
                    case BinaryOperation.GreaterThan:
                        _il.Emit(OpCodes.Cgt);
                        break;
                    case BinaryOperation.GreaterThanOrEqual:
                        _il.Emit(OpCodes.Clt);
                        EmitLogicalNot();
                        break;
                    default:
                        throw new NotSupportedException($"Cannot generate MSIL for binary operation {e.Operation}.");
                }

                break;
        }
    }

    /// <summary>
    /// Генерирует код логического "и" с вычислением по короткой схеме.
    /// </summary>
    private void EmitLogicalAnd(BinaryOperationExpression e)
    {
        Label falseLabel = _il.DefineLabel();
        Label endLabel = _il.DefineLabel();

        e.Left.Accept(this);
        _il.Emit(OpCodes.Ldc_I4_0);
        _il.Emit(OpCodes.Ceq);
        _il.Emit(OpCodes.Brtrue, falseLabel);

        e.Right.Accept(this);
        _il.Emit(OpCodes.Ldc_I4_0);
        _il.Emit(OpCodes.Ceq);
        _il.Emit(OpCodes.Brtrue, falseLabel);

        _il.Emit(OpCodes.Ldc_I4_1);
        _il.Emit(OpCodes.Br, endLabel);

        _il.MarkLabel(falseLabel);
        _il.Emit(OpCodes.Ldc_I4_0);

        _il.MarkLabel(endLabel);
    }

    /// <summary>
    /// Генерирует код логического "или" с вычислением по короткой схеме.
    /// </summary>
    private void EmitLogicalOr(BinaryOperationExpression e)
    {
        Label trueLabel = _il.DefineLabel();
        Label endLabel = _il.DefineLabel();

        e.Left.Accept(this);
        _il.Emit(OpCodes.Ldc_I4_0);
        _il.Emit(OpCodes.Ceq);
        _il.Emit(OpCodes.Brfalse, trueLabel);

        e.Right.Accept(this);
        _il.Emit(OpCodes.Ldc_I4_0);
        _il.Emit(OpCodes.Ceq);
        _il.Emit(OpCodes.Brfalse, trueLabel);

        _il.Emit(OpCodes.Ldc_I4_0);
        _il.Emit(OpCodes.Br, endLabel);

        _il.MarkLabel(trueLabel);
        _il.Emit(OpCodes.Ldc_I4_1);

        _il.MarkLabel(endLabel);
    }

    /// <summary>
    /// Генерирует код бинарной операции над строками.
    /// </summary>
    private void EmitStringsBinaryOperation(BinaryOperationExpression e)
    {
        switch (e.Operation)
        {
            case BinaryOperation.Add:
                e.Left.Accept(this);
                e.Right.Accept(this);

                MethodInfo concat = GetMethod(typeof(string), "Concat", [typeof(string), typeof(string)]);
                _il.Emit(OpCodes.Call, concat);
                break;

            case BinaryOperation.Equal:
            case BinaryOperation.NotEqual:
            case BinaryOperation.LessThan:
            case BinaryOperation.LessThanOrEqual:
            case BinaryOperation.GreaterThan:
            case BinaryOperation.GreaterThanOrEqual:
                e.Left.Accept(this);
                e.Right.Accept(this);

                MethodInfo compareOrdinal = GetMethod(typeof(string), "CompareOrdinal", [typeof(string), typeof(string)]);

                _il.Emit(OpCodes.Call, compareOrdinal);

                switch (e.Operation)
                {
                    case BinaryOperation.Equal:
                        _il.Emit(OpCodes.Ldc_I4_0);
                        _il.Emit(OpCodes.Ceq);
                        break;

                    case BinaryOperation.NotEqual:
                        _il.Emit(OpCodes.Ldc_I4_0);
                        _il.Emit(OpCodes.Ceq);
                        EmitLogicalNot();
                        break;

                    case BinaryOperation.LessThan:
                        _il.Emit(OpCodes.Ldc_I4_0);
                        _il.Emit(OpCodes.Clt);
                        break;

                    case BinaryOperation.LessThanOrEqual:
                        _il.Emit(OpCodes.Ldc_I4_0);
                        _il.Emit(OpCodes.Cgt);
                        EmitLogicalNot();
                        break;

                    case BinaryOperation.GreaterThan:
                        _il.Emit(OpCodes.Ldc_I4_0);
                        _il.Emit(OpCodes.Cgt);
                        break;

                    case BinaryOperation.GreaterThanOrEqual:
                        _il.Emit(OpCodes.Ldc_I4_0);
                        _il.Emit(OpCodes.Clt);
                        EmitLogicalNot();
                        break;
                }

                break;

            default:
                throw new NotSupportedException($"Unknown binary operation for string: {e.Operation}.");
        }
    }

    /// <summary>
    /// Выполняет логическое отрицание результата.
    /// </summary>
    private void EmitLogicalNot()
    {
        _il.Emit(OpCodes.Ldc_I4_0);
        _il.Emit(OpCodes.Ceq);
    }

    /// <summary>
    /// Пишет вещественное число в консоль с правильным форматированием.
    /// </summary>
    private void WriteFloat()
    {
        LocalBuilder tempDouble = _il.DeclareLocal(typeof(double));

        _il.Emit(OpCodes.Stloc, tempDouble);

        _il.Emit(OpCodes.Ldloca, tempDouble);

        _il.Emit(OpCodes.Ldstr, "G15");

        MethodInfo invariantCultureGetter = typeof(CultureInfo)
            .GetProperty("InvariantCulture")!.GetMethod!;

        _il.Emit(OpCodes.Call, invariantCultureGetter);

        MethodInfo toStringMethod = typeof(double).GetMethod(
            "ToString",
            [typeof(string), typeof(IFormatProvider)])!;

        _il.Emit(OpCodes.Call, toStringMethod);

        MethodInfo writeMethod = GetMethod(typeof(Console), "Write", [typeof(string)]);
        _il.Emit(OpCodes.Call, writeMethod);
    }

    /// <summary>
    /// Находит статический метод указанного типа стандартной библиотеки классов .NET.
    /// </summary>
    private static MethodInfo GetMethod(Type type, string methodName, Type[] parameterTypes)
    {
        MethodInfo? method = type.GetMethod(methodName, parameterTypes);
        if (method == null)
        {
            string parameterTypeNames = string.Join(", ", parameterTypes.Select(t => t.Name));
            throw new InvalidOperationException($"Cannot find method {type.Name}.{methodName}({parameterTypeNames}.");
        }

        return method;
    }

    /// <summary>
    /// Добавляет новую область видимости переменных на стек.
    /// </summary>
    private void BeginScope()
    {
        _scopesStack.Push(new Dictionary<string, LocalBuilder>());
        _il.BeginScope();
    }

    /// <summary>
    /// Убирает текущую область видимости переменных со стека.
    /// </summary>
    private void EndScope()
    {
        _il.EndScope();
        _scopesStack.Pop();
    }

    /// <summary>
    /// Создает локальную переменную для i-го параметра функции (нумерация начинается с нуля).
    /// </summary>
    private void EmitDefineParameter(string name, ValueType type, int argumentNo)
    {
        LocalBuilder local = _il.DeclareLocal(_typeMapper.MapType(type));

        _il.Emit(OpCodes.Ldarg, argumentNo);
        _il.Emit(OpCodes.Stloc, local);

        CurrentScope.Add(name, local);
    }

    /// <summary>
    /// Приводит два значения на стеке к общему типу перед сравнением.
    /// </summary>
    private void EmitConvertToCommonType(ValueType leftType, ValueType rightType)
    {
        if (leftType == ValueType.Float || rightType == ValueType.Float)
        {
            if (leftType == ValueType.Integer)
            {
                _il.Emit(OpCodes.Conv_R8);
            }

            if (rightType == ValueType.Integer)
            {
                _il.Emit(OpCodes.Conv_R8);
            }
        }
    }

    /// <summary>
    /// Декорирует имя функции, чтобы гарантировать отсутствие пересечений с системными именами методов
    ///  (такими как "Main").
    /// </summary>
    private string GetUserFunctionMethodName(string name)
    {
        return "Paspp" + name;
    }

    private MethodBuilder DefineProgramClassMethod(string name, Type returnType, Type[] parameterTypes)
    {
        return _programTypeBuilder.DefineMethod(
            name,
            MethodAttributes.Public | MethodAttributes.Static,
            returnType,
            parameterTypes
        );
    }
}