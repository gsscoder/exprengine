#region License
//
// Expression Engine Library: MathExpressionBase.cs
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
using ExpressionEngine.Core;
#endregion
namespace ExpressionEngine
{
	/// <summary>
	/// Represents a mathematical expression that can be evaluated, accessing <see cref="Value"/> property.
	/// </summary>
    public sealed class Expression
    {
		private Expression(string text)
		{
			Text = text;
		}

		/// <summary>
		/// Creates an <see cref="ExpressionEngine.Expression" instance with specified string./>
		/// </summary>
		/// <param name='text'>Infix notation string to be evaluated.</param>
		public static Expression Create(string text)
		{
			return new Expression(text);
		}

		/// <summary>
		/// Gets the <see cref="System.String"/> with the mathematical expression of the current instance.
		/// </summary>
		/// <value>A <see cref="System.String"/> with the mathematical expression.</value>
		public string Text { get; private set; }

		public double Value
		{
			get
			{
				if (_value == null)
				{
					_value = Kernel.ParseString(Text).Evaluate();
				}
				return _value.Value;
			}
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="ExpressionEngine.Expression"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and
		/// data structures such as a hash table.</returns>
		public sealed override int GetHashCode()
		{
			if (_value != null)
			{
				return Text.GetHashCode() ^ _value.Value.GetHashCode();
			}
			return Text.GetHashCode();
		}

		private double? _value;
    }
}