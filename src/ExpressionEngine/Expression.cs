#region License
//
// Expression Engine Library: MathExpressionBase.cs
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
using ExpressionEngine.Core;
#endregion

namespace ExpressionEngine
{
	/// <summary>
	/// Represents mathematical expression. Its result is stored in the <see cref="Value"/> property.
	/// </summary>
    public class Expression
    {
        private Expression() {}

        private Expression(string text)
        {
            _value = double.NaN;
        }

		protected Expression(string text, double value)
		{
			_text = text;
			_value = value;
		}

		/// <summary>
		/// Creates an <see cref="ExpressionEngine.Expression"/> instance with infix notation string.
		/// </summary>
		/// <param name='text'>Infix notation string to be evaluated.</param>
        /// <returns>A <see cref="ExpressionEngine.Expression"/> instance or appropriate mutable derived type.</returns>
		public static Expression Create(string text)
		{
		    return Create(text, null, null);
		}

        /// <summary>
        /// Creates an <see cref="ExpressionEngine.Expression"/> instance with infix notation string,
        /// with user defined variables.
        /// </summary> 
        /// <param name="text">Infix notation string to be evaluated.</param>
        /// <param name="variables">A generic dictionary of <see cref="System.String"/> keys as variable names
        /// and <see cref="System.Double"/> for values.</param>
        /// <returns>A <see cref="ExpressionEngine.Expression"/> instance or appropriate mutable derived type.</returns>
        public static Expression Create(string text, IDictionary<string, double> variables)
        {
            return Create(text, variables, null);
        }

        /// <summary>
        /// Creates an <see cref="ExpressionEngine.Expression"/> instance with infix notation string,
        /// with user defined functions.
        /// </summary> 
        /// <param name="text">Infix notation string to be evaluated.</param>
        /// <param name="functions">A generic dictionary of <see cref="System.String"/> keys as function names
        /// and lambda expressions for function bodies.</param>
        /// <returns>A <see cref="ExpressionEngine.Expression"/> instance or appropriate mutable derived type.</returns>
        public static Expression Create(string text, IDictionary<string, Func<double[], double>> functions)
        {
            return Create(text, null, functions);
        }

        /// <summary>
        /// Creates an <see cref="ExpressionEngine.Expression"/> instance with infix notation string,
        /// with user defined variables and functions.
        /// </summary> 
        /// <param name="text">Infix notation string to be evaluated.</param>
        /// <param name="variables">A generic dictionary of <see cref="System.String"/> keys as variable names
        /// and <see cref="System.Double"/> for values.</param>
        /// <param name="functions">A generic dictionary of <see cref="System.String"/> keys as function names
        /// and lambda expressions for function bodies.</param>
        /// <returns>A <see cref="ExpressionEngine.Expression"/> instance or appropriate mutable derived type.</returns>
        public static Expression Create(string text, IDictionary<string, double> variables,
            IDictionary<string, Func<double[], double>> functions)
        {
            var tree = Kernel.ParseString(text);
            if (!tree.HasUserDefinedNames)
            {
                if (variables != null || functions != null)
                {
                    throw new ExpressionException("Functions or variables supplied for an immutable expression.");
                }
                // No user defined names? Expression can be immutable.
                var visitor = ExpressionVisitor.Create();
                tree.Root.Accept(visitor);
                return new Expression(text, (double)visitor.Result);
            }
            // We can create a mutable expression.
            return new MutableExpression(text, variables, functions);
        }

		/// <summary>
		/// Gets the <see cref="System.String"/> with the mathematical expression of the current instance.
		/// </summary>
		/// <value>A <see cref="System.String"/> with the mathematical expression.</value>
		public string Text { get { return _text; } }

        /// <summary>
        /// Evaluates the mathematical expression as a <see cref="System.Double"/>.
        /// </summary>
        /// <value>The <see cref="System.Double"/> of the expression.</value>
		public virtual double Value { get { return _value; } } // Here will be placed a caching subsystem

        /// <summary>
        /// Provides a way to define o change a variable value using its name. Calling this method when the
        /// expression is immutable will raise a <see cref="ExpressionEngine.ExpressionException"/>.
        /// </summary>
        /// <param name="name">A <see cref="System.String"/> name of the variable to create or change.</param>
        /// <param name="value">A <see cref="System.Double"/> of the value.</param>
        public virtual void DefineVariable(string name, double value)
        {
            throw new ExpressionException("Immutable exceptions do not support variables.");
        }

        /// <summary>
        /// Provides a way to define o change a function using its name. Calling this method when the
        /// expression is immutable will raise a <see cref="ExpressionEngine.ExpressionException"/>.
        /// </summary>
        /// <param name="name">A <see cref="System.String"/> name of the function to create or change.</param>
        /// <param name="body">A lambda expression that accepts an array of <see cref="System.Double"/> and
        /// returns a <see cref="System.Double"/> scalar.</param>
        public virtual void DefineFunction(string name, Func<double[], double> body)
        {
            throw new ExpressionException("Immutable exceptions do not support functions.");
        }

		/// <summary>
		/// Serves as a hash function for a <see cref="ExpressionEngine.Expression"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and
		/// data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			return _text.GetHashCode() ^ _value.GetHashCode();
		}

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="ExpressionEngine.Expression"/> instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="ExpressionEngine.Expression"/> instance.</returns>
        public override string ToString()
        {
            return Text;
        }

		private readonly string _text;
		private readonly double _value;

        private sealed class MutableExpression : Expression
        {
            private MutableExpression() {}

            public MutableExpression(string text, IDictionary<string, double> variables,
                IDictionary<string, Func<double[], double>> functions) : base(text)
            {
                _variables = variables == null ? new Dictionary<string, double>() :
                    new Dictionary<string, double>(variables);
                _functions = functions == null ? new Dictionary<string, Func<double[], double>>() :
                    new Dictionary<string, Func<double[], double>>(functions);
                _tree = Kernel.ParseString(text);
            }

            public override double Value
            {
                get
                {
                    // Here will be placed a caching subsystem
                    var visitor = ExpressionVisitor.Create();
                    visitor.UserDefinedVariables = _variables;
                    visitor.UserDefinedFunctions = _functions;
                    _tree.Root.Accept(visitor);
                    return (double) visitor.Result;
                }
            }

            public override void DefineVariable(string name, double value)
            {
                if (_variables.ContainsKey(name))
                {
                    _variables[name] = value;
                }
                else
                {
                    _variables.Add(name, value);
                }
            }

            public override void DefineFunction(string name, Func<double[], double> body)
            {
                if (_variables.ContainsKey(name))
                {
                    _functions[name] = body;
                }
                else
                {
                    _functions.Add(name, body);
                }
            }

            private readonly Model.Ast _tree;
            private readonly IDictionary<string, double> _variables;
            private readonly IDictionary<string, Func<double[], double>> _functions;
        }
    }
}