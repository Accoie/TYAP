using System.Globalization;
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
    }

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

        program.Accept(this);

        _il.Emit(OpCodes.Ret);

        _programTypeBuilder.CreateType();

        return mainMethod;
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
            new[] { typeof(string), typeof(IFormatProvider) })!;

        _il.Emit(OpCodes.Call, toStringMethod);

        MethodInfo writeMethod = GetMethod(typeof(Console), "Write", [typeof(string)]);
        _il.Emit(OpCodes.Call, writeMethod);
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