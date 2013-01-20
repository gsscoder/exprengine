#region License
//
// Expression Engine Library: Evaluator.cs
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
using ExpressionEngine.Internal;
#endregion

namespace ExpressionEngine
{
    /// <summary>
    /// Provides methods for contextless expressions evaluation.
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// Evalutates an expression supplied as a <see cref="System.String"/> without persistence of context.
        /// </summary>
        /// <param name="expression">A <see cref="System.String"/> expression to evaluate.</param>
        /// <returns>An <see cref="System.Object"/> that represents the result of evaluation.</returns>
        public static object Evaluate(string expression)
        {
            var tree = SyntaxTree.ParseString(expression);
            var global = new Scope();
            BuiltIn.IntializeScope(global);
            var visitor = Visitor.Create(global);
            tree.Root.Accept(visitor);
            return visitor.Result;
        }

        /// <summary>
        /// Evalutates an expression supplied as a <see cref="System.String"/> without persistence of context.
        /// </summary>
        /// <typeparam name="T">The type specified for result.</typeparam>
        /// <param name="expression">A <see cref="System.String"/> expression to evaluate.</param>
        /// <returns>A <typeparamref name="T"/> that represents the result of evaluation.</returns>
        public static T Evaluate<T>(string expression)
        {
            return (T) Evaluate(expression);
        }
    }
}
