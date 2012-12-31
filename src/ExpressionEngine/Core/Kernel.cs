#region License
//
// Expression Engine Library: Kernel.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2012 Giacomo Stelluti Scala
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
using System.IO;
#endregion

namespace ExpressionEngine.Core
{
	static class Kernel
	{
		public static Model.Expression ParseString(string value)
		{
			using (var scanner = new Scanner(new StringReader(value)))
			{
				return new Parser(scanner).Parse();
			}
		}

        #region BuiltIns
        internal abstract class BuiltIn
        {
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
            public static double Function(string name, double[] args)
            {
                ParameterInfo param;
                if (!FuncsLookup.TryGetValue(name, out param))
                {
                    throw new ExpressionException(string.Format(CultureInfo.InvariantCulture, "Undefined function: '{0}'.", name));
                }
                if (!param.Match(args.Length))
                {
                    throw new ExpressionException(string.Format(CultureInfo.InvariantCulture, "Function '{0}' requires only {1}.", name, param.ToString()));
                };
                if (string.CompareOrdinal("log", name) == 0)
                {
                    if (args.Length == 1) { return Math.Log(args[0]); }
                    else if (args.Length == 2) { return Math.Log(args[0], args[1]); }
                }
                if (string.CompareOrdinal("abs", name) == 0) { return Math.Abs(args[0]); }
                if (string.CompareOrdinal("asin", name) == 0) { return Math.Asin(args[0]); }
                if (string.CompareOrdinal("sin", name) == 0) { return Math.Sin(args[0]); }
                if (string.CompareOrdinal("sinh", name) == 0) { return Math.Sinh(args[0]); }
                if (string.CompareOrdinal("acos", name) == 0) { return Math.Acos(args[0]); }
                if (string.CompareOrdinal("cos", name) == 0) { return Math.Cos(args[0]); }
                if (string.CompareOrdinal("cosh", name) == 0) { return Math.Cosh(args[0]); }
                if (string.CompareOrdinal("sqrt", name) == 0) { return Math.Sqrt(args[0]); }
                if (string.CompareOrdinal("atan", name) == 0) { return Math.Atan(args[0]); }
                if (string.CompareOrdinal("tan", name) == 0) { return Math.Tan(args[0]); }
                if (string.CompareOrdinal("tanh", name) == 0) { return Math.Tanh(args[0]); }

                throw new InvalidOperationException(); // Unreachable code
            }
            public static double Variable(string name)
            {
                double value;
                if (!VarsLookup.TryGetValue(name, out value))
                {
                    throw new ExpressionException(string.Format(CultureInfo.InvariantCulture, "Undefined variable: '{0}'.", name));
                }
                return value;
            }
            private static readonly Dictionary<string, ParameterInfo> FuncsLookup = new Dictionary<string, ParameterInfo>()
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
            private static readonly Dictionary<string, double> VarsLookup = new Dictionary<string, double>()
                {
                    {"e", Math.E},
                    {"pi", Math.PI}
                };
        }
        #endregion
    }
}