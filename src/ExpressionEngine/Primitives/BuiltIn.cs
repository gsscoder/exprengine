#region License
//
// Expression Engine Library: BuiltIn.cs
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
#endregion

namespace ExpressionEngine.Primitives
{
    class BuiltIn
    {
        private BuiltIn() { }

        static BuiltIn() {}

        public static BuiltIn Instance { get { return Singleton; } }

        public Func<object[], object> Log = arguments =>
            {
                switch (arguments.Length)
                {
                    case 1:
                        return (double) Math.Log((double) arguments[0]);
                    case 2:
                        return (double) Math.Log((double) arguments[0], (double) arguments[1]);
                    default:
                        throw new EvaluatorException("log requires one or two arguments");
                }
            };

        public Func<object[], object> Abs = arguments =>
            {
                if (arguments.Length == 1)
                {
                    return (double) Math.Abs((double) arguments[0]);
                }
                throw new EvaluatorException("abs requires one argument");
            };

        public Func<object[], object> Asin = arguments =>
            {
                if (arguments.Length == 1)
                {
                    return (double) Math.Asin((double) arguments[0]);
                }
                throw new EvaluatorException("asin requires one argument");
            };

        public Func<object[], object> Sin = arguments =>
            {
                if (arguments.Length == 1)
                {
                    return (double) Math.Sin((double) arguments[0]);
                }
                throw new EvaluatorException("sin requires one argument");
            };

        public Func<object[], object> Sinh = arguments =>
            {
                if (arguments.Length == 1)
                {
                    return (double) Math.Sinh((double) arguments[0]);
                }
                throw new EvaluatorException("sinh requires one argument");
            };

        public Func<object[], object> Acos = arguments =>
            {
                if (arguments.Length == 1)
                {
                    return (double) Math.Acos((double) arguments[0]);
                }
                throw new EvaluatorException("acos requires one argument");
            };

        public Func<object[], object> Cos = arguments =>
            {
                if (arguments.Length == 1)
                {
                    return (double) Math.Cos((double) arguments[0]);
                }
                throw new EvaluatorException("cos requires one argument");
            };

        public Func<object[], object> Cosh = arguments =>
            {
                if (arguments.Length == 1)
                {
                    return (double) Math.Cosh((double) arguments[0]);
                }
                throw new EvaluatorException("cosh requires one argument");
            };

        public Func<object[], object> Sqrt = arguments =>
            {
                if (arguments.Length == 1)
                {
                    return (double) Math.Sqrt((double) arguments[0]);
                }
                throw new EvaluatorException("sqrt requires one argument");
            };

        public Func<object[], object> Atan = arguments =>
            {
                if (arguments.Length == 1)
                {
                    return (double) Math.Atan((double) arguments[0]);
                }
                throw new EvaluatorException("atan requires one argument");
            };

        public Func<object[], object> Tan = arguments =>
            {
                if (arguments.Length == 1)
                {
                    return (double) Math.Tan((double) arguments[0]);
                }
                throw new EvaluatorException("tan requires one argument");
            };

        public Func<object[], object> Tanh = arguments =>
            {
                if (arguments.Length == 1)
                {
                    return (double) Math.Tanh((double) arguments[0]);
                }
                throw new EvaluatorException("tanh requires one argument");
            };

        public Func<object[], object> Pow = arguments =>
            {
                if (arguments.Length == 2)
                {
                    return (double) Math.Pow((double) arguments[0], (double) arguments[1]);
                }
                throw new EvaluatorException("pow requires one argument");
            };

        private static readonly BuiltIn Singleton = new BuiltIn();
    }
}