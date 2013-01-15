#region License
//
// Expression Engine Library: Parser.cs
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
using System.Linq;
using ExpressionEngine.Internal.Model;
#endregion

namespace ExpressionEngine.Internal
{
    sealed class Parser : IDisposable
    {
        private Parser() {}

        public Parser(Lexer scanner)
        {
            _scanner = scanner;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _scanner.Dispose();
                _disposed = true;
            }
        }

        public SyntaxTree Parse()
        {
            Expression root = ParseExpression();
            if (_brackets != 0)
            {
                throw new EvaluatorException(_scanner.Column, "Syntax error, odd number of brackets.");
            }
            return new SyntaxTree(root);
        }

        private Expression ParseExpression(bool insideFunc = false)
        {
            if (!insideFunc)
            {
                Expect(InitialGroup);
            }
            else
            {
                Ensure(InitialGroupWithComma);
            }
            Expression expr = ParseAdditiveBinary();
            return expr;
        }

        private Expression ParseAdditiveBinary()
        {
            Expression expr = ParseMultiplicativeBinary();
            while (!_scanner.IsEof() && (_current.Type == TokenType.Plus || _current.Type == TokenType.Minus))
            {
                var binaryAddSub = new BinaryExpression
                    {
                        Operator = ((PunctuatorToken)_current).ToOperatorType(),
                        Left = expr
                    };
                Expect(MiddleGroupAdditiveRelational);
                binaryAddSub.Right = ParseMultiplicativeBinary();
                expr = binaryAddSub;
            }
            return expr;
        }

        private Expression ParseMultiplicativeBinary()
        {
            Expression expr = ParseRelationalBinary();
            while (!_scanner.IsEof() && (_current.Type == TokenType.Multiply ||
                _current.Type == TokenType.Divide || _current.Type == TokenType.Modulo))
            {
                var binaryMulDiv = new BinaryExpression
                    {
                        Operator = ((PunctuatorToken)_current).ToOperatorType(),
                        Left = expr
                    };
                Expect(MiddleGroupMultiplicative);
                binaryMulDiv.Right = ParseRelationalBinary();
                expr = binaryMulDiv;
            }
            return expr;
        }

        private Expression ParseRelationalBinary()
        {
            Expression expr = ParseIdentifier();
            while (!_scanner.IsEof() && (_current.Type == TokenType.Equality || _current.Type == TokenType.Inequality ||
                _current.Type == TokenType.LessThan || _current.Type == TokenType.GreaterThan ||
                _current.Type == TokenType.LessThanOrEqual || _current.Type == TokenType.GreaterThanOrEqual))
            {
                var binaryExp = new BinaryExpression
                    {
                        Operator = ((PunctuatorToken) _current).ToOperatorType(),
                        Left = expr
                    };
                Expect(MiddleGroupAdditiveRelational);
                binaryExp.Right = ParseIdentifier();
                expr = binaryExp;
            }
            return expr;
        }

        private Expression ParseIdentifier()
        {
            if (_current == null)
            {
                throw new EvaluatorException(_scanner.Column, "Expected function or expression.");
            }

            if (_current.Type != TokenType.Identifier)
            {
                return ParseUnary();
            }

            if (_scanner.PeekToken() == null || (_scanner.PeekToken() != null &&
                _scanner.PeekToken().Type != TokenType.LeftParenthesis))
            {
                // variable
                var varExpr = new VariableExpression(_current.Text);
                if (_scanner.PeekToken() != null)
                {
                    Expect(MiddleGroupIdentifier);
                }
                else
                {
                    Consume(); // Reach eof
                }
                return varExpr;
            }

            var expr = new FunctionCallExpression(_current.Text);
            Expect(TokenType.LeftParenthesis);
            while (!_scanner.IsEof())
            {
                Consume();
                expr.Arguments.Add(ParseExpression(true));
                if (_current == null)
                {
                    break;
                }
                if (_current.Type == TokenType.Comma)
                {
                    continue;
                }
                else if (_current.Type == TokenType.RightParenthesis)
                {
                    Consume();
                    break;
                }
                else
                {
                    throw new EvaluatorException(_scanner.Column, "Expected comma or close bracket.");
                }
            }
            return expr;
        }

        private Expression ParseUnary()
        {
            if (_current == null)
            {
                throw new EvaluatorException(_scanner.Column, "Expected unary operator, literal or open bracket.");    
            }

            var unary = new UnaryExpression();
            if (_current.Type == TokenType.Minus)
            {
                unary.Operator = OperatorType.UnaryMinus;
                Expect(MiddleGroupUnary);
            }
            else if (_current.Type == TokenType.Plus)
            {
                unary.Operator = OperatorType.UnaryPlus;
                Expect(MiddleGroupUnary);
            }

            if (_current.Type == TokenType.Literal)
            {
                unary.Value = new LiteralExpression(((LiteralToken)_current).Value);
                if (_scanner.PeekToken() != null && _scanner.PeekToken().Type == TokenType.Literal)
                {
                    throw new EvaluatorException(_scanner.Column, "Expected expression.");
                }
            }
            else if (_current.Type == TokenType.LeftParenthesis)
            {
                unary.Value = ParseExpression();
            }
            else if (_current.Type == TokenType.Identifier)
            {
                unary.Value = ParseIdentifier();
            }
            else
            {
                throw new EvaluatorException(_scanner.Column, "Expected literal or open bracket.");
            }
            Consume();
            return unary;
        }

        #region Token Stream Controllers
        private void Ensure(TokenType[] types, bool identifierAllowed = true)
        {
            if (_current == null)
            {
                throw new EvaluatorException(_scanner.Column,
                    string.Format("Unexpected end of input, instead of token(s): {0}{1}.", identifierAllowed ? "'IDENT', " : "", Token.StringOf(types)));
            }
            if (!((identifierAllowed && _current.Type == TokenType.Identifier) ||
                types.Contains(_current.Type)))
            {
                throw new EvaluatorException(_scanner.Column,
                    string.Format("Syntax error, expected token(s): 'IDENT', {0}; but found '{1}'.", Token.StringOf(types), Token.StringOf(_current)));
            }
        }

        private void Expect(TokenType[] types, bool identifierAllowed = true)
        {
            var next = _scanner.PeekToken();
            if (next == null)
            {
                throw new EvaluatorException(_scanner.Column,
                    string.Format("Unexpected end of input, instead of token(s): {0}{1}.", identifierAllowed ? "'IDENT', " : "" ,Token.StringOf(types)));
            }
            if ((identifierAllowed && next.Type == TokenType.Identifier) ||
                types.Contains(next.Type))
            {
                Consume();
            }
            else
            {
                throw new EvaluatorException(_scanner.Column,
                    string.Format("Syntax error, expected token(s): {0}{1}; but found '{2}'.", identifierAllowed ? "'IDENT', " : "", Token.StringOf(types), Token.StringOf(next)));
            }
        }

        private void Expect(TokenType type)
        {
            var next = _scanner.PeekToken();
            if (next == null)
            {
                throw new EvaluatorException(_scanner.Column,
                    string.Format("Unexpected end of input, instead of token(s): 'LITERAL', '{0}'.", Token.StringOf(type)));
            }
            if (next.Type == type)
            {
                Consume();
            }
            else
            {
                throw new EvaluatorException(_scanner.Column,
                    string.Format("Syntax error, expected token(s) 'LITERAL', '{0}'; but found '{1}'.", Token.StringOf(type), Token.StringOf(next)));
            }
        }

        private void Consume()
        {
            _current = _scanner.NextToken();
            if (_current == null) { return; }

            if (_current.Type == TokenType.LeftParenthesis) { _brackets++; }
            else if (_current.Type == TokenType.RightParenthesis) { _brackets--; }
        }
        #endregion

        ~Parser()
        {
            Dispose(false);
        }

        #region Token Groups
        private static readonly TokenType[] InitialGroup = { TokenType.Literal, TokenType.Plus, TokenType.Minus, TokenType.LeftParenthesis, TokenType.Equality, TokenType.Inequality, TokenType.LessThan, TokenType.GreaterThan, TokenType.LessThanOrEqual, TokenType.GreaterThanOrEqual }; // + ident
        private static readonly TokenType[] InitialGroupWithComma = { TokenType.Literal, TokenType.Comma, TokenType.Plus, TokenType.Minus, TokenType.LeftParenthesis, TokenType.Equality, TokenType.Inequality, TokenType.LessThan, TokenType.GreaterThan, TokenType.LessThanOrEqual, TokenType.GreaterThanOrEqual }; // + ident
        private static readonly TokenType[] MiddleGroupAdditiveRelational = { TokenType.Literal, TokenType.Plus, TokenType.Minus, TokenType.LeftParenthesis, TokenType.RightParenthesis }; // + ident
        private static readonly TokenType[] MiddleGroupMultiplicative = { TokenType.Literal, TokenType.Plus, TokenType.Minus, TokenType.LeftParenthesis }; // + ident
        private static readonly TokenType[] MiddleGroupIdentifier = { TokenType.Literal, TokenType.Plus, TokenType.Minus, TokenType.Multiply, TokenType.Divide, TokenType.Equality, TokenType.Inequality, TokenType.LessThan, TokenType.GreaterThan, TokenType.LessThanOrEqual, TokenType.GreaterThanOrEqual, TokenType.RightParenthesis, TokenType.Comma };
        private static readonly TokenType[] MiddleGroupUnary = { TokenType.Literal, TokenType.LeftParenthesis }; // + ident
        #endregion

        private bool _disposed;
        private int _brackets;
        private Token _current;
        private readonly Lexer _scanner;
    }
}