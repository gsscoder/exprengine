using System;

sealed class Function
{
    public Function(string name, Func<object[], object> func, bool paramsLess = false)
    {
        Name = name;
        Delegate = func;
        ParametersLess = paramsLess;
    }

    public string Name { get; private set; }

    public Func<object[], object> Delegate { get; private set; }

    public bool ParametersLess { get; private set; }
}