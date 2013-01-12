#region License
//
// Expression Engine Library: Kernel.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2012 - 2013 Giacomo Stelluti Scala
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.Globalization;
#endregion

namespace ExpressionEngine.Internal
{
    sealed class Kernel
    {
        private Kernel() {}

        static Kernel() {}

        public static Kernel Instance { get { return Singleton; } }

        public IBuiltInService BuiltIn = new BuiltInService();

        public ITypeService Primitives = new TypeService();

        public ICache<object> Cache = new ObjectCache<object>();

        /// <summary>
        /// Convenience method for building an AST from a string, used internally.
        /// </summary>
        public Model.SyntaxTree ParseString(string value)
        {
            using (var scanner = new Lexer(Text.OfString(value)))
            {
                return new Parser(scanner).Parse();
            }
        }

        #region BuiltIns
        internal interface IBuiltInService
        {
            object ExecuteBuiltInFunction(string name, object[] args);

            object GetBuiltInVariable(string name);

            bool IsBuiltInFunction(string name);

            bool IsBuiltInVariable(string name);
        }

        private sealed class BuiltInService : IBuiltInService
        {
            #region ParameterInfo support struct
            private struct ParameterInfo
            {
                private ParameterInfo(byte min, byte max)
                {
                    _min = min;
                    _max = max;
                }
                public static ParameterInfo OneParameter() { return new ParameterInfo(1, 1); }
                public static ParameterInfo TwoParameter() { return new ParameterInfo(2, 2); }
                public static ParameterInfo OneTwoParameter() { return new ParameterInfo(1, 2); }
                public bool Match(int length)
                {
                    if (_min == 1 && _max == 1) return length == 1;
                    if (_min == 1 && _max == 2) return length == 1 || length == 2;
                    throw new InvalidOperationException(); // Unreachable code
                }
                public override string ToString()
                {
                    if (_min == 1 && _max == 1) { return "one parameter"; }
                    if (_min == 2 && _max == 2) { return "two paramters"; }
                    if (_min == 1 && _max == 2) { return "one or two parameters"; }
                    throw new InvalidOperationException(); // Unreachable code
                }
                private readonly byte _min;
                private readonly byte _max;
            }
            #endregion

            public object ExecuteBuiltInFunction(string name, object[] args)
            {
                var param = _funcsLookup[name];
                if (!param.Match(args.Length))
                {
                    throw new ExpressionException(string.Format(CultureInfo.InvariantCulture, "Function '{0}' requires only {1}.", name, param.ToString()));
                }
                var typed = Array.ConvertAll(args, Kernel.Instance.Primitives.ToReal);
                if (string.CompareOrdinal("log", name) == 0)
                {
                    if (args.Length == 1) { return Math.Log(typed[0]); }
                    else if (args.Length == 2) { return Math.Log(typed[0], typed[1]); }
                }
                if (string.CompareOrdinal("abs", name) == 0) { return Math.Abs(typed[0]); }
                if (string.CompareOrdinal("asin", name) == 0) { return Math.Asin(typed[0]); }
                if (string.CompareOrdinal("sin", name) == 0) { return Math.Sin(typed[0]); }
                if (string.CompareOrdinal("sinh", name) == 0) { return Math.Sinh(typed[0]); }
                if (string.CompareOrdinal("acos", name) == 0) { return Math.Acos(typed[0]); }
                if (string.CompareOrdinal("cos", name) == 0) { return Math.Cos(typed[0]); }
                if (string.CompareOrdinal("cosh", name) == 0) { return Math.Cosh(typed[0]); }
                if (string.CompareOrdinal("sqrt", name) == 0) { return Math.Sqrt(typed[0]); }
                if (string.CompareOrdinal("atan", name) == 0) { return Math.Atan(typed[0]); }
                if (string.CompareOrdinal("tan", name) == 0) { return Math.Tan(typed[0]); }
                if (string.CompareOrdinal("tanh", name) == 0) { return Math.Tanh(typed[0]); }

                throw new InvalidOperationException(); // Unreachable code
            }

            public object GetBuiltInVariable(string name)
            {
                return _varsLookup[name];
            }
            
            public bool IsBuiltInFunction(string name)
            {
                return _funcsLookup.ContainsKey(name);
            }

            public bool IsBuiltInVariable(string name)
            {
                return _varsLookup.ContainsKey(name);
            }

            private readonly Dictionary<string, ParameterInfo> _funcsLookup = new Dictionary<string, ParameterInfo>()
                {
                    {"log", ParameterInfo.OneTwoParameter()},
                    {"abs", ParameterInfo.OneParameter()},
                    {"asin", ParameterInfo.OneParameter()},
                    {"sin", ParameterInfo.OneParameter()},
                    {"sinh", ParameterInfo.OneParameter()},
                    {"acos", ParameterInfo.OneParameter()},
                    {"cos", ParameterInfo.OneParameter()},
                    {"cosh", ParameterInfo.OneParameter()},
                    {"sqrt", ParameterInfo.OneParameter()},
                    {"atan", ParameterInfo.OneParameter()},
                    {"tan", ParameterInfo.OneParameter()},
                    {"tanh", ParameterInfo.OneParameter()}
                };
            private readonly Dictionary<string, double> _varsLookup = new Dictionary<string, double>()
                {
                    {"e", Math.E},
                    {"pi", Math.PI}
                };
        }
        #endregion

        #region Primitive Types
        internal interface ITypeService
        {
            PrimitiveType ToPrimitiveType(Type type);

            double ToReal(object value);
        }

        private sealed class TypeService : ITypeService
        {
            public PrimitiveType ToPrimitiveType(Type type)
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

            public double ToReal(object value)
            {
                if (value is long)
                {
                    return (double) (long) value;
                }
                if (value is double)
                {
                    return (double) value;
                }
                throw new ExpressionException(string.Format("Can't convert object of type '{0}' to real.", value.GetType()));
            }
        }
        #endregion

        private static readonly Kernel Singleton = new Kernel();
    }

    #region Version
    #endregion
}