using System.Reflection;
using System.Reflection.Emit;

namespace MsilCodegen;

public class BuiltinFunctionEmitter
{
    private readonly Dictionary<string, Action<ILGenerator>> _functionsMap;

    public BuiltinFunctionEmitter()
    {
        _functionsMap = new Dictionary<string, Action<ILGenerator>>
        {
            {
                "abs", EmitABS
            },
            {
                "min", EmitMin
            },
            {
                "max", EmitMax
            },
            {
                "len", EmitLength
            },
            {
                "round", EmitRound
            },
            {
                "getsymbol", EmitGetSymbol
            },
            {
                "tostring", EmitToString
            },
        };
    }

    public void EmitCallBuiltinFunction(string name, ILGenerator il)
    {
        Action<ILGenerator> action = _functionsMap[name];
        action(il);
    }

    /// <summary>
    /// Генерирует вызов встроенной функции abs.
    /// </summary>
    private void EmitABS(ILGenerator il)
    {
        MethodInfo method = GetMethod(typeof(Math), "Abs", [typeof(int)]);
        il.Emit(OpCodes.Call, method);
    }

    /// <summary>
    /// Генерирует вызов встроенной функции min.
    /// </summary>
    private void EmitMin(ILGenerator il)
    {
        // Для двух аргументов: Math.Min(int, int) или перегрузка для double
        MethodInfo method = GetMethod(typeof(Math), "Min", [typeof(int), typeof(int)]);
        il.Emit(OpCodes.Call, method);
    }

    /// <summary>
    /// Генерирует вызов встроенной функции max.
    /// </summary>
    private void EmitMax(ILGenerator il)
    {
        MethodInfo method = GetMethod(typeof(Math), "Max", [typeof(int), typeof(int)]);
        il.Emit(OpCodes.Call, method);
    }

    /// <summary>
    /// Генерирует вызов встроенной функции len.
    /// </summary>
    private void EmitLength(ILGenerator il)
    {
        MethodInfo getter = GetPropertyGetterMethod(typeof(string), "Length");
        il.Emit(OpCodes.Callvirt, getter);
    }

    /// <summary>
    /// Генерирует вызов встроенной функции round.
    /// </summary>
    private void EmitRound(ILGenerator il)
    {
        MethodInfo method = GetMethod(typeof(Math), "Round", [typeof(double)]);
        il.Emit(OpCodes.Call, method);
    }

    /// <summary>
    /// Генерирует вызов встроенной функции getsymbol.
    /// </summary>
    private void EmitGetSymbol(ILGenerator il)
    {
        MethodInfo method = GetMethod(typeof(string), "Substring", [typeof(int), typeof(int)]);
        il.Emit(OpCodes.Callvirt, method);
    }

    /// <summary>
    /// Генерирует вызов встроенной функции tostring.
    /// </summary>
    private void EmitToString(ILGenerator il)
    {
        MethodInfo method = typeof(object).GetMethod("ToString", Type.EmptyTypes)!;
        il.Emit(OpCodes.Callvirt, method);
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
            throw new InvalidOperationException($"Cannot find method {type.Name}.{methodName}({parameterTypeNames}).");
        }

        return method;
    }

    private static MethodInfo GetPropertyGetterMethod(Type type, string propertyName)
    {
        PropertyInfo? outProperty = type.GetProperty(propertyName);
        if (outProperty == null)
        {
            throw new InvalidOperationException($"Cannot find property {type.Name}.{propertyName}.");
        }

        MethodInfo? getterMethod = outProperty.GetGetMethod();
        if (getterMethod == null)
        {
            throw new InvalidOperationException($"Property {type.Name}.{propertyName} has no getter.");
        }

        return getterMethod;
    }
}