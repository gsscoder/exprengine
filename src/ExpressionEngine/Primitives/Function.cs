#region License
//
// Expression Engine Library: Function.cs
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
}
