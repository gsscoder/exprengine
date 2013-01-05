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
using System.Text;
using System.Threading;
using ExpressionEngine.Core;
#endregion

namespace ExpressionEngine
{
	/// <summary>
	/// Represents mathematical immutable expression. Its result is stored in the <see cref="Value"/> property.
	/// </summary>
    public class Expression
    {
        protected Expression() {}

        protected Expression(string text)
            : this(text, double.NaN, false)
        {
        }

		protected Expression(string text, double value)
            : this(text, value, false)
		{
		}

        protected Expression(string text, double value, bool isValueCacheRetrieved)
        {
            _text = text;
            _value = value;
            _isValueCacheRetrieved = isValueCacheRetrieved;
        }

		/// <summary>
		/// Creates an <see cref="ExpressionEngine.Expression"/> instance with infix notation string.
		/// </summary>
		/// <param name='text'>Infix notation string to be evaluated.</param>
        /// <returns>A <see cref="ExpressionEngine.Expression"/> instance or appropriate mutable derived type.</returns>
		public static Expression Create(string text)
		{
            if (string.IsNullOrEmpty(text)) { throw new ArgumentNullException("text", "Expression text can't be null."); }

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
            if (string.IsNullOrEmpty(text)) { throw new ArgumentNullException("text", "Expression text can't be null."); }

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
            if (string.IsNullOrEmpty(text)) { throw new ArgumentNullException("text", "Expression text can't be null."); }

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
            var tree = Kernel.Instance.ParseString(text);
            if (!tree.HasUserDefinedNames)
            {
                if (variables != null || functions != null)
                {
                    throw new ExpressionException("Functions or variables supplied for an immutable expression.");
                }
                // No user defined names? Expression is immutable.
                var cache = Kernel.Instance.Cache;
                var normalizedText = Expression.NormalizeText(text);
                // Searching a pre-calculated value .
                if (!cache.Contains(normalizedText))
                {
                    var visitor = ExpressionVisitor.Create(null, null);
                    tree.Root.Accept(visitor);
                    var value = (double) visitor.Result;
                    cache.Add(normalizedText, value);
                    return new Expression(text, value);
                }
                return new Expression(text, cache[normalizedText], true);
            }
            // We can create a mutable expression.
            return new MutableExpression(text, variables, functions);
        }

        /// <summary>
        /// Returns a <see cref="ExpressionEngine.Expression"/> wrapper that is synchronized (thread safe).
        /// </summary>
        /// <param name="mutableExpression">A mutable derived type of <see cref="ExpressionEngine.Expression"/>.</param>
        /// <returns>A <see cref="ExpressionEngine.Expression"/> wrapper synchronized (thread safe).</returns>
        public static Expression Synchronized(Expression mutableExpression)
        {
            var synchronized = mutableExpression as MutableExpression;
            if (synchronized == null)
            {
                throw new ExpressionException("Immutable expressions are implicitly thread safe.");
            }
            return new SynchronizedMutableExpression(synchronized);
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
        /// expression is immutable will raise a <see cref="ExpressionEngine.Expression"/>.
        /// </summary>
        /// <param name="name">A <see cref="System.String"/> name of the variable to create or change.</param>
        /// <param name="value">A <see cref="System.Double"/> of the value.</param>
        public virtual void DefineVariable(string name, double value)
        {
            throw new ExpressionException("Immutable exceptions do not support variables.");
        }

        /// <summary>
        /// Provides a way to define o change a function using its name. Calling this method when the
        /// expression is immutable will raise a <see cref="ExpressionEngine.Expression"/>.
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
            return NormalizedText.GetHashCode() ^ _value.GetHashCode();
		}

        /// <summary>
        /// Returns a value that indicates whether the current instance and a specified expression are equal.
        /// </summary>
        /// <param name="value">true if this expression and <paramref name="value"/> are equal; otherwise, false.</param>
        /// <returns>The expression to compare.</returns>
        public virtual bool Equals(Expression value)
        {
            if ((object) value == null)
            {
                return false;
            }
            return NormalizedText == value.NormalizedText && Value.Equals(value.Value);
        }

        /// <summary>
        /// Returns a value that indicates whether the current instance and a specified object have the same value.
        /// </summary>
        /// <param name="obj">true if the <paramref name="obj"/> parameter is a <see cref="ExpressionEngine.Expression"/> object or
        /// a type capable of implicit conversion to a <see cref="ExpressionEngine.Expression"/> object, and its value is equal to
        /// the current <see cref="ExpressionEngine.Expression"/> object; otherwise, false.</param>
        /// <returns>The object to compare.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            var typed = obj as Expression;
            if (typed == null)
            {
                return false;
            }
            return NormalizedText == typed.NormalizedText && Value.Equals(typed.Value);
        }

        /// <summary>
        /// Returns a value that indicates whether two expressions are equal.
        /// </summary>
        /// <param name="left">The first expression number to compare.</param>
        /// <param name="right">The second expression number to compare.</param>
        /// <returns>true if the <paramref name="left"/> and <paramref name="right"/> parameters are equal; otherwise, false.</returns>
        public static bool operator ==(Expression left, Expression right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (((object)left == null) || ((object)right == null))
            {
                return false;
            }
            return left.NormalizedText == right.NormalizedText && left.Value.Equals(right.Value);
        }

        /// <summary>
        /// Returns a value that indicates whether two expressions are not equal.
        /// </summary>
        /// <param name="left">The first expression number to compare.</param>
        /// <param name="right">The second expression number to compare.</param>
        /// <returns>true if the <paramref name="left"/> and <paramref name="right"/> parameters are not equal; otherwise, false.</returns>
        public static bool operator !=(Expression left, Expression right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="ExpressionEngine.Expression"/> instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="ExpressionEngine.Expression"/> instance.</returns>
        public override string ToString()
        {
            return Text;
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="ExpressionEngine.Expression"/> is synchronized (thread safe).
        /// </summary>
	    public virtual bool IsSynchronized
	    {
            get { return false; }
	    }

        /// <summary>
        /// Gets an object that can be used to synchronize access to a <see cref="ExpressionEngine.Expression"/> instance.
        /// </summary>
        public virtual object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                }
                return _syncRoot;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the value of the <see cref="ExpressionEngine.Expression"/> is retrieved from cache.
        /// </summary>
	    public virtual bool IsValueCacheRetrieved
	    {
            get { return _isValueCacheRetrieved; }
	    }

	    private string NormalizedText
	    {
	        get
	        {
	            if (_normalizedText == null)
	            {
	                _normalizedText = NormalizeText(Text);
	            }
	            return _normalizedText;
	        }
	    }

        private static string NormalizeText(string value)
        {
            var normalized = new StringBuilder(value.Length);
            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (!Scanner.IsWhiteSpace(c))
                {
                    normalized.Append(c);
                }
            }
            return normalized.ToString();
        }

        private object _syncRoot;
	    private readonly bool _isValueCacheRetrieved;
	    private string _normalizedText;
		private readonly string _text;
		private readonly double _value;

        /// <summary>
        /// Represents mathematical mutable expression. Its result is calculated when the <see cref="Value"/> property is read.
        /// This type has no public constructor, because its instances are managed by <see cref="ExpressionEngine.Expression"/>.
        /// </summary>
        private sealed class MutableExpression : Expression
        {
            private MutableExpression() : base() { }

            public MutableExpression(string text, IDictionary<string, double> variables,
                IDictionary<string, Func<double[], double>> functions)
                : base(text)
            {
                _variables = variables == null ? new Dictionary<string, double>() :
                    new Dictionary<string, double>(variables);
                _functions = functions == null ? new Dictionary<string, Func<double[], double>>() :
                    new Dictionary<string, Func<double[], double>>(functions);
                _tree = Kernel.Instance.ParseString(text);
            }

            public override double Value
            {
                get
                {
                    // Here will be placed a caching subsystem
                    var visitor = ExpressionVisitor.Create(_variables, _functions);
                    _tree.Root.Accept(visitor);
                    return (double)visitor.Result;
                }
            }

            public override void DefineVariable(string name, double value)
            {
                if (Kernel.Instance.BuiltIn.IsBuiltInVariable(name))
                {
                    throw new ExpressionException("Can't (re)define a built-in variable.");
                }
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
                if (Kernel.Instance.BuiltIn.IsBuiltInFunction(name))
                {
                    throw new ExpressionException("Can't (re)define a built-in function.");
                }
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

        /// <summary>
        /// Represents mathematical thread safe <see cref="ExpressionEngine.Expression"/> wrapper. This type has no
        /// public constructor, because its instances are managed by <see cref="ExpressionEngine.Expression"/>.
        /// </summary>
        private sealed class SynchronizedMutableExpression : Expression
        {
            private SynchronizedMutableExpression() { }

            public SynchronizedMutableExpression(MutableExpression expression)
            {
                _expression = expression;
                _root = expression.SyncRoot;
            }

            public override bool IsSynchronized
            {
                get { return true; }
            }

            public override object SyncRoot
            {
                get { return _root; }
            }

            public override bool IsValueCacheRetrieved
            {
                get
                {
                    lock (_root)
                    {
                        return _expression.IsValueCacheRetrieved;
                    }
                }
            }

            public override bool Equals(Expression value)
            {
                lock (_root)
                {
                    return _expression.Equals(value);
                }
            }

            public override double Value
            {
                get
                {
                    lock (_root)
                    {
                        return _expression.Value;
                    }
                }
            }

            public override void DefineVariable(string name, double value)
            {
                lock (_root)
                {
                    _expression.DefineVariable(name, value);
                }
            }

            public override void DefineFunction(string name, Func<double[], double> body)
            {
                lock (_root)
                {
                    _expression.DefineFunction(name, body);
                }
            }

            private object _root;
            private readonly MutableExpression _expression;
        }
    }
}