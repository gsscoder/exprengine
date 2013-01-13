using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionEngine.Internal;

namespace ExpressionEngine.Core
{
    static class TypeService
    {
        public static PrimitiveType ToPrimitiveType(Type type)
        {
            if (type == typeof(long))
            {
                return PrimitiveType.Integer;
            }
            if (type == typeof(double))
            {
                return PrimitiveType.Real;
            }
            throw new InvalidOperationException("Type not yet supported.");
        }

        public static double ToReal(object value)
        {
            if (value is long)
            {
                return (double)(long)value;
            }
            if (value is double)
            {
                return (double)value;
            }
            throw new ExpressionException(string.Format("Can't convert object of type '{0}' to real.", value.GetType()));
        }
    }
}