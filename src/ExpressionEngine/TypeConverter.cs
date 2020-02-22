using System;
using System.Globalization;
using ExpressionEngine;

static class TypeConverter
{
    internal static PrimitiveType ToPrimitiveType(Type type)
    {
        if (type == typeof(bool))
        {
            return PrimitiveType.Boolean;
        }
        if (type == typeof(double))
        {
            return PrimitiveType.Number;
        }
        if (type == typeof(string))
        {
            return PrimitiveType.String;
        }
        throw new EvaluatorException("Type not yet supported.");
    }

    public static double ToNumber(object value)
    {
        if (value is bool)
        {
            return (bool) value ? 1 : 0;
        }
        if (value is double)
        {
            return (double) value;
        }
        if (value is string)
        {
            double converted;
            if (double.TryParse((string) value, NumberStyles.Any, CultureInfo.CurrentCulture, out converted))
            {
                return converted;
            }
            return double.NaN;
        }
        throw new EvaluatorException("Can't convert object of type '{0}' to number.".FormatInvariant(value.GetType()));
    }

    public static bool ToBoolean(object value)
    {
        if (value is bool)
        {
            return (bool) value;
        }
        if (value is double)
        {
            return ((double) value) != 0 && double.IsNaN((double) value) == false;
        }
        if (value is string)
        {
            bool converted;
            if (bool.TryParse((string) value, out converted))
            {
                return converted;
            }
        }
        throw new EvaluatorException("Can't convert object of type '{0}' to boolean.".FormatInvariant(value.GetType()));
    }

    public static string ToString(object value)
    {
        if (value is string)
        {
            return (string) value;
        }
        return Convert.ToString(value, CultureInfo.CurrentCulture);
    }
}