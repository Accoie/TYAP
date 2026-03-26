using System.Reflection;
using System.Reflection.Emit;

using Ast;
using Ast.Expressions;
using Ast.Statements;

using ValueType = Runtime.ValueType;

namespace MsilCodegen;

public class MsilCodegenPass : IAstVisitor
{
    private readonly ModuleBuilder _moduleBuilder;
    private readonly TypeMapper _typeMapper;

    /// <summary>
    /// Тип Program будущей программы.
    /// </summary>
    private TypeBuilder _programTypeBuilder = null!;

    /// <summary>
    /// Генератор инструкций для текущего метода.
    /// </summary>
    private ILGenerator _il = null!;

    public MsilCodegenPass(ModuleBuilder moduleBuilder)
    {
        _moduleBuilder = moduleBuilder;
        _typeMapper = new TypeMapper();
    }

    /// <summary>
    /// Создаёт класс Program и метод Main(), возвращает MethodBuilder для метода Main().
    /// </summary>
    public MethodBuilder GenerateProgramCode(BlockStatement program)
    {
        // Создаём класс Program.
        _programTypeBuilder = _moduleBuilder.DefineType(
            "Program",
            TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class
        );

        MethodBuilder mainMethod = DefineProgramClassMethod("Main", typeof(void), Type.EmptyTypes);
        _il = mainMethod.GetILGenerator();

        program.Accept(this);

        _il.Emit(OpCodes.Ret);

        _programTypeBuilder.CreateType();

        return mainMethod;
    }

    public void Visit(BinaryOperationExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(UnaryOperationExpression e)
    {
        throw new NotImplementedException();
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

    public void Visit(FunctionCallExpression s)
    {
        throw new NotImplementedException();
    }

    public void Visit(AssignmentStatement s)
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

    public void Visit(InputStatement s)
    {
        throw new NotImplementedException();
    }

    public void Visit(OutputStatement s)
    {
        foreach (Expression argument in s.Arguments)
        {
            argument.Accept(this);

            Type argType = argument.ResultType switch
            {
                ValueType.Integer => typeof(int),
                ValueType.Float => typeof(double),
                ValueType.String => typeof(string),
                _ => throw new NotImplementedException($"Output of type {argument.ResultType}"),
            };

            MethodInfo writeMethod = GetMethod(typeof(Console), "WriteLine", [argType]);
            _il.Emit(OpCodes.Call, writeMethod);
        }
    }

    public void Visit(BlockStatement s)
    {
        foreach (Statement statement in s.Statements)
        {
            statement.Accept(this);
        }
    }

    public void Visit(ReturnStatement s)
    {
        throw new NotImplementedException();
    }

    public void Visit(VariableDeclarationStatement s)
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

    public void Visit(VariableExpression variableExpression)
    {
        throw new NotImplementedException();
    }

    public void Visit(IteratorDeclaration iteratorDeclaration)
    {
        throw new NotImplementedException();
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
}