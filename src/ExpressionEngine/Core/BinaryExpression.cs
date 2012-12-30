#region License
//
// Expression Engine Library: BinaryExpression.cs
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
using ExpressionEngine.Core;
#endregion

namespace ExpressionEngine.Model
{
    enum OperatorType
    {
        UnaryPlus,
        UnaryMinus,
        Exponent,
        Multiply,
        Divide,
        Add,
        Subtract
    }

    sealed class BinaryExpression : Expression
    {
        public OperatorType Operator;

        public Model.Expression Left;

        public Model.Expression Right;

		public override void Accept(ExpressionVisitor visitor)
		{
			visitor.Visit(this);
		}

		/*
        public override double Evaluate()
        {
            switch (Operator)
            {
                case OperatorType.Add:
                    return Left.Evaluate() + Right.Evaluate();
                case OperatorType.Subtract:
                    return Left.Evaluate() - Right.Evaluate();
                case OperatorType.Multiply:
                    return Left.Evaluate() * Right.Evaluate();
                case OperatorType.Divide:
                    return Left.Evaluate() / Right.Evaluate();
                case OperatorType.Exponent:
                    return Math.Pow(Left.Evaluate(), Right.Evaluate());
            }
            throw new ExpressionException("Invalid operator type.");
        }
		*/
    }
}