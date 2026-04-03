using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.AccessControl;

using Ast;
using Ast.Expressions;
using Ast.Statements;

using ValueType = Runtime.ValueType;

namespace MsilCodegen;

public class MsilCodegenPass : IAstVisitor
{
    private readonly ModuleBuilder _moduleBuilder;

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

    public MsilCodegenPass(ModuleBuilder moduleBuilder)
    {
        _moduleBuilder = moduleBuilder;
        _scopesStack = new Stack<Dictionary<string, LocalBuilder>>();
    }

    /// <summary>
    /// Текущая область видимости переменных.
    /// </summary>
    private Dictionary<string, LocalBuilder> CurrentScope => _scopesStack.Peek();

    /// <summary>
    /// Создаёт класс Program и метод Main(), возвращает MethodBuilder для метода Main().
    /// </summary>
    public MethodBuilder GenerateProgramCode(BlockStatement program)
    {
        _programTypeBuilder = _moduleBuilder.DefineType(
            "Program",
            TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class
        );

        MethodBuilder mainMethod = DefineProgramClassMethod("Main", typeof(void), Type.EmptyTypes);
        _il = mainMethod.GetILGenerator();

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
                        $"Unary minus requires numeric type, got {e.ResultType}");
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
            throw new NotImplementedException($"Literal of type {e.ResultType} are not supported yet.");
        }
    }

    public void Visit(AssignmentStatement s)
    {
        s.Value.Accept(this);

        if (!CurrentScope.TryGetValue(s.Name, out LocalBuilder? local))
        {
            throw new InvalidOperationException(
                $"Переменная '{s.Name}' не найдена в текущей области видимости"
            );
        }

        _il.Emit(OpCodes.Stloc, local);
    }

    public void Visit(InputStatement s)
    {
        if (!CurrentScope.TryGetValue(s.VariableName, out LocalBuilder? local))
        {
            throw new InvalidOperationException(
                $"Переменная '{s.VariableName}' не найдена в текущей области видимости"
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
            throw new NotImplementedException($"Ввод для типа {variableType} не поддерживается");
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
                    _ => throw new NotImplementedException($"Output of type {argument.ResultType}"),
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
        foreach (Statement statement in s.Statements)
        {
            statement.Accept(this);
        }
    }

    public void Visit(VariableDeclarationStatement s)
    {
        Type ilType = s.DeclaredType switch
        {
            ValueType.Integer => typeof(int),
            ValueType.Float => typeof(double),
            ValueType.String => typeof(string),
            _ => throw new NotImplementedException($"Тип {s.DeclaredType} не поддерживается"),
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
        if (!CurrentScope.TryGetValue(e.Name, out LocalBuilder? local))
        {
            throw new InvalidOperationException(
                $"Переменная '{e.Name}' не найдена в текущей области видимости"
            );
        }

        _il.Emit(OpCodes.Ldloc, local);
    }

    public void Visit(FunctionCallExpression s)
    {
        throw new NotImplementedException();
    }

    public void Visit(IfElseStatement s)
    {
        throw new NotImplementedException();
    }

    public void Visit(ForLoopStatement s)
    {
        throw new NotImplementedException();
    }

    public void Visit(ReturnStatement s)
    {
        throw new NotImplementedException();
    }

    public void Visit(FunctionDeclarationStatement s)
    {
        throw new NotImplementedException();
    }

    public void Visit(WhileLoopStatement s)
    {
        throw new NotImplementedException();
    }

    public void Visit(BreakStatement s)
    {
        throw new NotImplementedException();
    }

    public void Visit(ContinueStatement s)
    {
        throw new NotImplementedException();
    }

    public void Visit(FunctionCallStatement s)
    {
        throw new NotImplementedException();
    }

    public void Visit(ParameterDeclaration parameterDeclarationStatement)
    {
        throw new NotImplementedException();
    }

    public void Visit(IteratorDeclaration iteratorDeclaration)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Генерирует код вычисления бинарной операции над целыми и вещественными числами.
    /// </summary>
    private void EmitIntegersOrFloatBinaryOperation(BinaryOperationExpression e)
    {
        switch (e.Operation)
        {
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
                throw new NotSupportedException($"Unexpected string binary operation {e.Operation}.");
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

        // Берем значение double из стека и объявляем переменную.
        _il.Emit(OpCodes.Stloc, tempDouble);

        _il.Emit(OpCodes.Ldloca, tempDouble);

        _il.Emit(OpCodes.Ldstr, "G15");

        // Получаем culture info и загружаем в стек для форматирования
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