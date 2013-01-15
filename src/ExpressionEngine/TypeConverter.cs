using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionEngine.Primitives
{
    public static class TypeConverter
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
            throw new EvaluatorException(string.Format("Can't convert object of type '{0}' to number.", value.GetType()));
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
            throw new EvaluatorException(string.Format("Can't convert object of type '{0}' to boolean.", value.GetType()));
        }

        //public static object 
    }
}
