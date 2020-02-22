using System;

class Function
{
    private Function() {}

    public Function(string name, Func<object[], object> func)
    {
        Name = name;
        Delegate = func;
    }

    public string Name { get; private set; }

    //private static bool CheckSignature(Delegate func)
    //{
    //    if (!IsNumericType(func.Method.ReturnType))
    //    {
    //        return false;
    //    }
    //    var @params = func.Method.GetParameters();
    //    if (@params.Length > 0)
    //    {
    //        foreach (var p in @params)
    //        {
    //            if (!IsNumericType(p.ParameterType))
    //            {
    //                return false;
    //            }
    //        }
    //    }
    //    return true;
    //}

    //private static bool IsNumericType(Type type)
    //{
    //    if (type.IsArray)
    //    {
    //        return type == typeof(byte[]) || type == typeof(short[]) || type == typeof(ushort[]) || type == typeof(int[]) ||
    //            type == typeof(uint[]) || type == typeof(long[]) || type == typeof(ulong[]) || type == typeof(float[]) ||
    //            type == typeof(double[]);
    //    }
    //    return type == typeof(byte) || type == typeof(short) || type == typeof(ushort) || type == typeof(int) ||
    //        type == typeof(uint) || type == typeof(long) || type == typeof(ulong) || type == typeof(float) ||
    //        type == typeof(double);
    //}

    public Func<object[], object> Delegate { get; private set; }
}