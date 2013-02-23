#region License
//
// Expression Engine Library: Context.cs
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
using ExpressionEngine.Internal;
#endregion

namespace ExpressionEngine
{
    /// <summary>
    /// Provides methods for expressions evaluation.
    /// </summary>
    public sealed class Context
    {
        /// <summary>
        /// Creates an instance of <see cref="ExpressionEngine.Context"/>.
        /// </summary>
        public Context()
        {
            _global = new Scope();
            BuiltIn.IntializeScope(_global);
        }

        /// <summary>
        /// Evalutates an expression supplied as a <see cref="System.String"/>.
        /// </summary>
        /// <param name="expression">A <see cref="System.String"/> expression to evaluate.</param>
        /// <returns>An <see cref="System.Object"/> that represents the result of evaluation.</returns>
        public object Evaluate(string expression)
        {
            var tree = SyntaxTree.ParseString(expression);
            var visitor = Visitor.Create(_global);
            tree.Root.Accept(visitor);
            return visitor.Result;
        }

        /// <summary>
        /// Evalutates an expression supplied as a <see cref="System.String"/>.
        /// </summary>
        /// <typeparam name="T">The type specified for result.</typeparam>
        /// <param name="expression">A <see cref="System.String"/> expression to evaluate.</param>
        /// <returns>A <typeparamref name="T"/> that represents the result of evaluation.</returns>
        public T EvaluateAs<T>(string expression)
        {
            return (T) Evaluate(expression);
        }

        /// <summary>
        /// Define a function in context of global scope.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <param name="function">The lamda expression of the function.</param>
        /// <returns>An <see cref="ExpressionEngine.Context"/> instance with modified scope.</returns>
        public Context SetFunction(string name, Func<object[], object> function)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Can't define a function with null or empty name.", "name");
            if (function == null) throw new ArgumentNullException("name", "Can't define a function with null lamda.");

            _global[name] = new Function(name, function);
            return this;
        }

        /// <summary>
        /// Define a variable in context of global scope.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="variable">The value of the variable.</param>
        /// <returns>An <see cref="ExpressionEngine.Context"/> instance with modified scope.</returns>
        public Context SetVariable(string name, double variable)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Can't define a variable with null or empty name.", "name");

            _global[name] = variable;
            return this;
        }

        public Context SetObject(string name, object obj)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Can't define an object with null or empty name.", "name");

            _global[name] = obj;
            return this;
        }

        private readonly Scope _global;
    }
}