#region License
//
// Expression Engine Library: EvaluatingVisitor.cs
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
using ExpressionEngine.Primitives;
using ExpressionEngine.Internal.Model;
#endregion

namespace ExpressionEngine.Internal
{
    sealed class EvaluatingVisitor : Visitor
    {
        public EvaluatingVisitor(Scope global)
            : base(global)
        {
        }

        public override void Visit(LiteralExpression expression)
        {
            _result = expression.Value;
        }

        public override void Visit(UnaryExpression expression)
        {
            expression.Value.Accept(this);
            switch (expression.Operator)
            {
                case OperatorType.UnaryPlus:
                    break;
                case OperatorType.UnaryMinus:
                    _result = new Number(_result.ToNumber() * -1D);
                    break;
                default:
                    throw new EvaluatorException("Invalid unary operator type.");
            }
        }

        public override void Visit(FunctionCallExpression callExpression)
        {
            var name = callExpression.Name;
            var argsList = new List<object>(callExpression.Arguments.Count);
            callExpression.Arguments.ForEach(arg =>
            {
                arg.Accept(this);
                argsList.Add(_result.ToObject());
            });
            var args = argsList.ToArray();
            _result = new Number((double) ((Function) GlobalScope[name]).Delegate(args)); //Execute(args));
        }

        public override void Visit(BinaryExpression expression)
        {
            Func<Instance> left = () =>
            {
                expression.Left.Accept(this);
                var leftValue = _result;
                return leftValue;
            };
            Func<Instance> right = () =>
            {
                expression.Right.Accept(this);
                var rightValue = _result;
                return rightValue;
            };
            switch (expression.Operator)
            {
                case OperatorType.Add:
                    _result = new Number(left().ToNumber() + right().ToNumber());
                    break;
                case OperatorType.Subtract:
                    _result = new Number(left().ToNumber() - right().ToNumber());
                    break;
                case OperatorType.Multiply:
                    _result = new Number(left().ToNumber() * right().ToNumber());
                    break;
                case OperatorType.Divide:
                    _result = new Number(left().ToNumber() / right().ToNumber());
                    break;
                case OperatorType.Modulo:
                    _result = new Number(left().ToNumber() % right().ToNumber());
                    break;
                case OperatorType.Exponent:
                    _result = new Number(Math.Pow(left().ToNumber(), right().ToNumber()));
                    break;
                default:
                    throw new EvaluatorException("Invalid binary operator type.");
            }
        }

        public override void Visit(VariableExpression expression)
        {
            var name = expression.Name;
            var inst = GlobalScope[name] as Instance;
            if (inst == null)
            {
                throw new EvaluatorException(string.Format(CultureInfo.InvariantCulture, "Undefined variable: '{0}'.", name));
            }
            _result = inst;
        }

        public override Instance Result { get { return _result; } }

        private Instance _result;
    }
}