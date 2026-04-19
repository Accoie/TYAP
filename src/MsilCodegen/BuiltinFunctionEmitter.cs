using System.Globalization;
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
            { "abs_f", EmitAbs },
            { "min_f", EmitMin },
            { "max_f", EmitMax },
            { "round", EmitRound },
            { "len", EmitLength },
            { "getsymbol", EmitGetSymbol },
            { "tostring_i", EmitToStringI },
            { "tostring_f", EmitToStringF },
        };
    }

    public void EmitCallBuiltinFunction(string name, ILGenerator il)
    {
        if (!_functionsMap.TryGetValue(name, out Action<ILGenerator>? action))
        {
            throw new InvalidOperationException($"Unknown builtin function: {name}");
        }

        action(il);
    }

    /// <summary>
    /// Генерирует вызов встроенной функции abs.
    /// </summary>
    private void EmitAbs(ILGenerator il)
    {
        MethodInfo method = GetMethod(typeof(Math), "Abs", [typeof(double)]);
        il.Emit(OpCodes.Call, method);
    }

    /// <summary>
    /// Генерирует вызов встроенной функции min.
    /// </summary>
    private void EmitMin(ILGenerator il)
    {
        MethodInfo method = GetMethod(typeof(Math), "Min", [typeof(double), typeof(double)]);
        il.Emit(OpCodes.Call, method);
    }

    /// <summary>
    /// Генерирует вызов встроенной функции max.
    /// </summary>
    private void EmitMax(ILGenerator il)
    {
        MethodInfo method = GetMethod(typeof(Math), "Max", [typeof(double), typeof(double)]);
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
        LocalBuilder tempIndex = il.DeclareLocal(typeof(int));
        il.Emit(OpCodes.Stloc, tempIndex);
        il.Emit(OpCodes.Ldloc, tempIndex);
        il.Emit(OpCodes.Ldc_I4_1);
        MethodInfo method = GetMethod(typeof(string), "Substring", [typeof(int), typeof(int)]);
        il.Emit(OpCodes.Callvirt, method);
    }

    /// <summary>
    /// Генерирует вызов встроенной функции tostring для int.
    /// </summary>
    private void EmitToStringI(ILGenerator il)
    {
        LocalBuilder tempInt = il.DeclareLocal(typeof(int));
        il.Emit(OpCodes.Stloc, tempInt);
        il.Emit(OpCodes.Ldloca, tempInt);
        MethodInfo toStringMethod = typeof(int).GetMethod("ToString", Type.EmptyTypes)!;
        il.Emit(OpCodes.Call, toStringMethod);
    }

    private void EmitToStringF(ILGenerator il)
    {
        LocalBuilder tempDouble = il.DeclareLocal(typeof(double));
        il.Emit(OpCodes.Stloc, tempDouble);
        il.Emit(OpCodes.Ldloca, tempDouble);
        il.Emit(OpCodes.Ldstr, "G15");
        MethodInfo invariantCultureGetter = typeof(CultureInfo)
            .GetProperty("InvariantCulture")!.GetMethod!;
        il.Emit(OpCodes.Call, invariantCultureGetter);
        MethodInfo toStringMethod = typeof(double).GetMethod(
            "ToString",
            [typeof(string), typeof(IFormatProvider)])!;
        il.Emit(OpCodes.Call, toStringMethod);
    }

    /// <summary>
    /// Находит статический метод.
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