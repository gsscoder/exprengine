#region License
//
// Expression Engine Library: ExpressionEvaluator.cs
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
using System.Data;
using System.Globalization;
using System.Linq;
using System.IO;
#endregion

namespace ExpressionEngine
{
    public sealed class ExpressionEvaluator : IExpressionEvaluator
    {
        public double Evaluate(string expression)
        {
            ParseInfixToRpn(expression);
            return EvaluateRpn();
        }

        private void ParseInfixToRpn(string expression)
        {
            _operations.Clear();
            _rpn.Clear();

            using(var scanner = new Scanner(new StringReader(expression)))
			{
	            while (!scanner.IsEof())
	            {
	                var token = scanner.NextToken();
	                //if (token.IsSeparator())
	                //{
	                //    while (_operations.Any() && !_operations.First().IsOpenBracket())
	                //    {
	                //        _rpn.Push(_operations.Pop());
	                //    }
	                //}
	                if (token.IsOpenBracket())
	                {
	                    _operations.Push(token);
	                }
	                else if (token.IsCloseBracket())
	                {
	                    while (_operations.Any() && !_operations.First().IsOpenBracket())
	                    {
	                        _rpn.Push(_operations.Pop());
	                    }
	                    _operations.Pop();
	                    //if (_operations.Any() && _operations.First().IsFunction())
	                    //{
	                    //    _rpn.Push(_operations.Pop());
	                    //}
	                }
	                else if (token.IsLiteral())
	                {
	                    _rpn.Push(token);
	                }
	                else if (token.IsOperator())
	                {
	                    while (_operations.Any() && _operations.First().IsOperator()
	                           && token.Precedence <= _operations.First().Precedence)
	                    {
	                        _rpn.Push(_operations.Pop());
	                    }
	                    _operations.Push(token);
	                }
	                //else if (token.IsFunction())
	                //{
	                //    _operations.Push(token);
	                //}
	                else
	                {
	                    throw new EvaluatorException(scanner.ColumnNumber, "Invalid token.");
	                }
	            }
			}
            while (_operations.Any())
            {
                _rpn.Push(_operations.Pop());
            }
            // reverse stack
            _rpn = new Stack<Token>(_rpn);
        }

        private double EvaluateRpn()
        {
            if (!_rpn.Any())
            {
                return 0;
            }
            _result.Clear();

            while (_rpn.Any())
            {
                var token = _rpn.Pop();
                if (token.IsLiteral())
                {
                    _result.Push(token.ToDouble());
                }
                else if (token.IsOperator())
                {
                    double a;
                    double b;
                    try
                    {
                        a = _result.Pop();
                        b = _result.Pop();
                    }
                    catch (InvalidOperationException e)
                    {
                        throw new EvaluatorException("Expected expression.", e);
                    }
                    if (token.IsPlus())
                    {
                        _result.Push(b + a);
                    }
                    else if (token.IsMinus())
                    {
                        _result.Push(b - a);
                    }
                    else if (token.IsStar())
                    {
                        _result.Push(b * a);
                    }
                    else if (token.IsSlash())
                    {
                        _result.Push(b / a);
                    }
                }
            }
            if (_rpn.Any())
            {
                throw new EvaluatorException("Missing operator.");
            }
            return _result.Pop();
        }

        private readonly Stack<Token> _operations = new Stack<Token>();
        private Stack<Token> _rpn = new Stack<Token>();
        private readonly Stack<double> _result = new Stack<double>(); 
    }
}