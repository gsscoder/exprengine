#region License
//
// Expression Engine Library: Parser.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2007 - 2012 Giacomo Stelluti Scala
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
using System.Globalization;
#endregion

namespace ExpressionEngine
{
    sealed class Parser : IDisposable
    {
        private Parser() {}

        public Parser(Scanner scanner)
        {
            _scanner = scanner;
            Advance();
        }

        public void Dispose()
        {
            _scanner.Dispose();
        }

        public Expression Parse()
        {
            Expression root = ParseExpression();
            return root;
        }

        private void Advance()
        {
            //_previous = _current;
            _current = _scanner.NextToken();
        }

        private Expression ParseExpression()
        {
            Expression expr = ParseBinaryAddSub();
            return expr;
        }

        private Expression ParseBinaryAddSub()
        {
            BinaryExpression binaryAddSub;
            Expression expr;

            expr = ParseBinaryMulDiv();
            while (!_scanner.IsEof() && (_current.IsPlus() || _current.IsMinus()))
            {
                binaryAddSub = new BinaryExpression();
                binaryAddSub.Operator = _current.IsPlus() ? OperatorType.Add : OperatorType.Subtract;
                binaryAddSub.Left = expr;
                Advance();
                binaryAddSub.Right = ParseBinaryMulDiv();
                expr = binaryAddSub;
            }
            return expr;
        }

        private Expression ParseBinaryMulDiv()
        {
            BinaryExpression binaryMulDiv;
            Expression expr;

            expr = ParseUnary();
            while (!_scanner.IsEof() && (_current.IsStar() || _current.IsSlash()))
            {
                binaryMulDiv = new BinaryExpression();
                binaryMulDiv.Operator = _current.IsStar() ? OperatorType.Multiply : OperatorType.Divide;
                binaryMulDiv.Left = expr;
                Advance();
                binaryMulDiv.Right = ParseUnary();
                expr = binaryMulDiv;
            }
            return expr;
        }

        private Expression ParseUnary()
        {
            if (_current == null)
            {
                throw new EvaluatorException("Expected unary operator, literal or open bracket.");    
            }

            var unary = new UnaryExpression();
            if (_current.IsMinus())
            {
               unary.Operator = OperatorType.UnaryMinus;
               Advance();
            }
            else if (_current.IsPlus())
            {
                unary.Operator = OperatorType.UnaryPlus;
                Advance();
            }

            if (_current.IsLiteral())
            {
                unary.Value = new LiteralExpression(Convert.ToDouble(_current.Text, CultureInfo.InvariantCulture));
                if (_scanner.PeekToken() != null && _scanner.PeekToken().IsLiteral())
                {
                    throw new EvaluatorException(_scanner.ColumnNumber, "Expected expression.");
                }
            }
            else if (_current.IsOpenBracket())
            {
                Advance();
                unary.Value = ParseExpression();
            }
            else
            {
                throw new EvaluatorException(_scanner.ColumnNumber, "Expected literal or open bracket.");
            }
            Advance();
            return unary;
        }

        #region Token Helpers
        private OperatorType GetOperator(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Plus:
                    return OperatorType.Add;
                case TokenType.Minus:
                    return OperatorType.Subtract;
                case TokenType.Star:
                    return OperatorType.Multiply;
                case TokenType.Slash:
                    return OperatorType.Divide;
            }
            throw new EvaluatorException(_scanner.ColumnNumber, "Expected binary operator.");
        }
        #endregion

        private Token _current;
        //private Token _previous;
        private readonly Scanner _scanner;
    }
}