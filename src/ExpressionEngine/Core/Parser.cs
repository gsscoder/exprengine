#region License
//
// Expression Engine Library: Parser.cs
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
using System.Globalization;
using System.Linq;
using System.Text;
using ExpressionEngine.Model;

#endregion

namespace ExpressionEngine
{
    sealed class Parser : IDisposable
    {
        private Parser() {}

        public Parser(Scanner scanner)
        {
            _scanner = scanner;
        }

        public void Dispose()
        {
            _scanner.Dispose();
        }

        public Model.Expression Parse()
        {
            Model.Expression root = ParseExpression();
            if (_brackets != 0)
            {
                throw new ExpressionException(_scanner.ColumnNumber, "Syntax error, odd number of brackets.");
            }
            return root;
        }

        private void Ensure(params TokenType[] types)
        {
            if (_current == null)
            {
                throw new ExpressionException(_scanner.ColumnNumber,
                    string.Format("Unexpected end of input, but found token(s): {0}.", TokenTypeArrayToString(types)));
            }
            if (!types.Contains(_current.Type))
            {
                throw new ExpressionException(_scanner.ColumnNumber,
                    string.Format("Syntax error, expected token(s) {0}; but found '{1}'.", TokenTypeArrayToString(types), _current.Type));
            }
        }

		private void Expect(params TokenType[] types)
		{
			var next = _scanner.PeekToken();
            if (next == null)
            {
                throw new ExpressionException(_scanner.ColumnNumber,
                    string.Format("Unexpected end of input, but found token(s): {0}.", TokenTypeArrayToString(types)));
            }
			if (types.Contains(next.Type))
			{
				Consume();
			}
			else
			{
				throw new ExpressionException(_scanner.ColumnNumber,
					string.Format("Syntax error, expected token(s) {0}; but found '{1}'.", TokenTypeArrayToString(types), next.Type));
			}
		}

        private void Consume()
        {
            //_previous = _current;
            _current = _scanner.NextToken();
            if (_current != null)
            {
                if (_current.IsOpenBracket())
                {
                    _brackets++;
                }
                else if (_current.IsCloseBracket())
                {
                    _brackets--;
                }
            }
        }

        private Model.Expression ParseExpression(bool insideFunc = false)
        {
            if (!insideFunc)
            {
                Expect(TokenType.Literal, TokenType.Plus, TokenType.Minus, TokenType.OpenBracket, TokenType.Identifier, TokenType.Caret);
            }
            else // may allow ','
            {
                //Expect(TokenType.Comma, TokenType.Literal, TokenType.Plus, TokenType.Minus, TokenType.OpenBracket, TokenType.Identifier);
                Ensure(TokenType.Comma, TokenType.Literal, TokenType.Plus, TokenType.Minus, TokenType.OpenBracket, TokenType.Identifier, TokenType.Caret);
            }
            Model.Expression expr = ParseBinaryAddSub();
            return expr;
        }

        private Model.Expression ParseBinaryAddSub()
        {
            Model.Expression expr = ParseBinaryMulDiv();
            while (!_scanner.IsEof() && (_current.IsPlus() || _current.IsMinus()))
            {
                var binaryAddSub = new Model.BinaryExpression
                    {
                        Operator = _current.IsPlus() ? Model.OperatorType.Add : Model.OperatorType.Subtract,
                        Left = expr
                    };
				Expect(TokenType.Literal, TokenType.Plus, TokenType.Minus, TokenType.OpenBracket, TokenType.CloseBracket, TokenType.Identifier);
                binaryAddSub.Right = ParseBinaryMulDiv();
                expr = binaryAddSub;
            }
            return expr;
        }

        private Model.Expression ParseBinaryMulDiv()
        {
            Model.Expression expr = ParseBinaryExp(); //ParseFunction();
            while (!_scanner.IsEof() && (_current.IsStar() || _current.IsSlash()))
            {
                var binaryMulDiv = new Model.BinaryExpression
                    {
                        Operator = _current.IsStar() ? Model.OperatorType.Multiply : Model.OperatorType.Divide,
                        Left = expr
                    };
                // TODO: can remove ClsoeBracket?
                Expect(TokenType.Literal, TokenType.Plus, TokenType.Minus, TokenType.OpenBracket, TokenType.CloseBracket, TokenType.Identifier);
                binaryMulDiv.Right = ParseBinaryExp(); //ParseFunction();
                expr = binaryMulDiv;
            }
            return expr;
        }

        private Model.Expression ParseBinaryExp()
        {
            Model.Expression expr = ParseFunction();
            while (!_scanner.IsEof() && _current.IsCaret())
            {
                var binaryExp = new Model.BinaryExpression
                    {
                        Operator = Model.OperatorType.Exponent,
                        Left = expr
                    };
                Expect(TokenType.Literal, TokenType.Plus, TokenType.Minus, TokenType.OpenBracket, TokenType.CloseBracket, TokenType.Identifier);
                binaryExp.Right = ParseFunction();
                expr = binaryExp;
            }
            return expr;
        }

        private Model.Expression ParseFunction()
        {
            if (_current == null)
            {
                throw new ExpressionException(_scanner.ColumnNumber, "Expected function or expression.");
            }

            if (!_current.IsIdentifier())
            {
                return ParseUnary();
            }

            var expr = new Model.FunctionExpression() {Name = _current.Text};
            Expect(TokenType.OpenBracket);
            while (!_scanner.IsEof())
            {
                Consume();
                expr.Arguments.Add(ParseExpression(true));
                //Expect(TokenType.Comma, TokenType.CloseBracket);
                if (_current == null)
                {
                    break;
                }
                if (_current.IsComma())
                {
                    continue; //Consume();
                }
                else if (_current.IsCloseBracket())
                {
                    Consume();
                    break;
                }
                else
                {
                    throw new ExpressionException(_scanner.ColumnNumber, "Expected comma or close bracket.");
                }
            }
            return expr;
        }

        private Model.Expression ParseUnary()
        {
            if (_current == null)
            {
                throw new ExpressionException(_scanner.ColumnNumber, "Expected unary operator, literal or open bracket.");    
            }

            var unary = new Model.UnaryExpression();
            if (_current.IsMinus())
            {
               	unary.Operator = Model.OperatorType.UnaryMinus;
				Expect(TokenType.Literal, TokenType.OpenBracket, TokenType.Identifier);
            }
            else if (_current.IsPlus())
            {
                unary.Operator = Model.OperatorType.UnaryPlus;
                Expect(TokenType.Literal, TokenType.OpenBracket, TokenType.Identifier);
            }

            if (_current.IsLiteral())
            {
                unary.Value = new Model.LiteralExpression(Convert.ToDouble(_current.Text, CultureInfo.InvariantCulture));
                if (_scanner.PeekToken() != null && _scanner.PeekToken().IsLiteral())
                {
                    throw new ExpressionException(_scanner.ColumnNumber, "Expected expression.");
                }
            }
            else if (_current.IsOpenBracket())
            {
                unary.Value = ParseExpression();
            }
            else if (_current.IsIdentifier())
            {
                unary.Value = ParseFunction();
            }
            else
            {
                throw new ExpressionException(_scanner.ColumnNumber, "Expected literal or open bracket.");
            }
            Consume();
            return unary;
        }

		private string TokenTypeArrayToString(TokenType[] types)
		{
			var b = new StringBuilder(6 * types.Length);
			foreach (TokenType t in types)
			{
				b.Append("'");
				switch (t)
				{
					case TokenType.OpenBracket:
						b.Append("(");
						break;
					case TokenType.CloseBracket:
						b.Append(")");
						break;
					case TokenType.Plus:
						b.Append("+");
						break;
					case TokenType.Minus:
						b.Append("-");
						break;
					case TokenType.Star:
						b.Append("*");
						break;
					case TokenType.Slash:
						b.Append("/");
						break;
                    case TokenType.Comma:
				        b.Append(",");
                        break;
					case TokenType.Literal:
						b.Append("NUMBER");
						break;
                    case TokenType.Identifier:
				        b.Append("IDENT");
                        break;
                    case TokenType.Caret:
				        b.Append("^");
                        break;
				}
				b.Append("'");
				b.Append(", ");
			}
			return b.ToString(0, b.Length - 2);
		}

        private int _brackets;
        private Token _current;
        //private Token _previous;
        private readonly Scanner _scanner;
    }
}