using System;
using System.Diagnostics.CodeAnalysis;
using ExpressionEngine;

sealed class BuiltIn
{
    private BuiltIn() {}

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Singleton, by design.")]
    static BuiltIn() {}

    public static BuiltIn Instance { get { return Singleton; } }

    public static void IntializeScope(Scope scope)
    {
        scope["log"] = new Function("log", BuiltIn.Instance.Log);
        scope["asin"] = new Function("asin", BuiltIn.Instance.Asin);
        scope["sin"] = new Function("sin", BuiltIn.Instance.Sin);
        scope["sinh"] = new Function("sinh", BuiltIn.Instance.Sinh);
        scope["acos"] = new Function("acos", BuiltIn.Instance.Acos);
        scope["cos"] = new Function("cos", BuiltIn.Instance.Cos);
        scope["cosh"] = new Function("cosh", BuiltIn.Instance.Cosh);
        scope["sqrt"] = new Function("sqrt", BuiltIn.Instance.Sqrt);
        scope["atan"] = new Function("atan", BuiltIn.Instance.Atan);
        scope["tan"] = new Function("tan", BuiltIn.Instance.Tan);
        scope["tanh"] = new Function("tanh", BuiltIn.Instance.Tanh);
        scope["pow"] = new Function("pow", BuiltIn.Instance.Pow);
        scope["e"] = Math.E;
        scope["pi"] = Math.PI;
    }

    public Func<object[], object> Log = arguments =>
        {
            switch (arguments.Length)
            {
                case 1:
                    return Math.Log(TypeConverter.ToNumber(arguments[0]));
                case 2:
                    return Math.Log(TypeConverter.ToNumber(arguments[0]), TypeConverter.ToNumber(arguments[1]));
                default:
                    throw new EvaluatorException("log requires one or two arguments");
            }
        };

    public Func<object[], object> Abs = arguments =>
        {
            if (arguments.Length == 1)
            {
                return Math.Abs(TypeConverter.ToNumber(arguments[0]));
            }
            throw new EvaluatorException("abs requires one argument");
        };

    public Func<object[], object> Asin = arguments =>
        {
            if (arguments.Length == 1)
            {
                return Math.Asin(TypeConverter.ToNumber(arguments[0]));
            }
            throw new EvaluatorException("asin requires one argument");
        };

    public Func<object[], object> Sin = arguments =>
        {
            if (arguments.Length == 1)
            {
                return Math.Sin(TypeConverter.ToNumber(arguments[0]));
            }
            throw new EvaluatorException("sin requires one argument");
        };

    public Func<object[], object> Sinh = arguments =>
        {
            if (arguments.Length == 1)
            {
                return Math.Sinh(TypeConverter.ToNumber(arguments[0]));
            }
            throw new EvaluatorException("sinh requires one argument");
        };

    public Func<object[], object> Acos = arguments =>
        {
            if (arguments.Length == 1)
            {
                return Math.Acos(TypeConverter.ToNumber(arguments[0]));
            }
            throw new EvaluatorException("acos requires one argument");
        };

    public Func<object[], object> Cos = arguments =>
        {
            if (arguments.Length == 1)
            {
                return Math.Cos(TypeConverter.ToNumber(arguments[0]));
            }
            throw new EvaluatorException("cos requires one argument");
        };

    public Func<object[], object> Cosh = arguments =>
        {
            if (arguments.Length == 1)
            {
                return Math.Cosh(TypeConverter.ToNumber(arguments[0]));
            }
            throw new EvaluatorException("cosh requires one argument");
        };

    public Func<object[], object> Sqrt = arguments =>
        {
            if (arguments.Length == 1)
            {
                return Math.Sqrt(TypeConverter.ToNumber(arguments[0]));
            }
            throw new EvaluatorException("sqrt requires one argument");
        };

    public Func<object[], object> Atan = arguments =>
        {
            if (arguments.Length == 1)
            {
                return Math.Atan(TypeConverter.ToNumber(arguments[0]));
            }
            throw new EvaluatorException("atan requires one argument");
        };

    public Func<object[], object> Tan = arguments =>
        {
            if (arguments.Length == 1)
            {
                return Math.Tan(TypeConverter.ToNumber(arguments[0]));
            }
            throw new EvaluatorException("tan requires one argument");
        };

    public Func<object[], object> Tanh = arguments =>
        {
            if (arguments.Length == 1)
            {
                return Math.Tanh(TypeConverter.ToNumber(arguments[0]));
            }
            throw new EvaluatorException("tanh requires one argument");
        };

    public Func<object[], object> Pow = arguments =>
        {
            if (arguments.Length == 2)
            {
                return Math.Pow(TypeConverter.ToNumber(arguments[0]), (double) arguments[1]);
            }
            throw new EvaluatorException("pow requires one argument");
        };

    private static readonly BuiltIn Singleton = new BuiltIn();
}