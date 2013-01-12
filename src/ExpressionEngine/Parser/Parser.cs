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
using ExpressionEngine.Internal;
using Model = ExpressionEngine.Internal.Model;
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

        public Model.SyntaxTree Parse()
        {
            Model.Expression root = ParseExpression();
            if (_brackets != 0)
            {
                throw new ExpressionException(_scanner.Column, "Syntax error, odd number of brackets.");
            }
            return new Model.SyntaxTree(root, _userVariables, _userFunctions);
        }

        private Model.Expression ParseExpression(bool insideFunc = false)
        {
            if (!insideFunc)
            {
                Expect(InitialGroup);
            }
            else
            {
                Ensure(InitialGroupWithComma);
            }
            Model.Expression expr = ParseAdditiveBinary();
            return expr;
        }

        private Model.Expression ParseAdditiveBinary()
        {
            Model.Expression expr = ParseMultiplicativeBinary();
            while (!_scanner.IsEof() && (_current == PunctuatorToken.Plus || _current == PunctuatorToken.Minus))
            {
                var binaryAddSub = new Model.BinaryExpression
                    {
                        Operator = ((PunctuatorToken)_current).ToOperatorType(),
                        Left = expr
                    };
                Expect(MiddleGroupAdditiveExponent);
                binaryAddSub.Right = ParseMultiplicativeBinary();
                expr = binaryAddSub;
            }
            return expr;
        }

        private Model.Expression ParseMultiplicativeBinary()
        {
            Model.Expression expr = ParseExponentBinary();
            while (!_scanner.IsEof() && (_current == PunctuatorToken.Multiply ||
                _current == PunctuatorToken.Divide || _current == PunctuatorToken.Modulo))
            {
                var binaryMulDiv = new Model.BinaryExpression
                    {
                        Operator = ((PunctuatorToken)_current).ToOperatorType(),
                        Left = expr
                    };
                Expect(MiddleGroupMultiplicative);
                binaryMulDiv.Right = ParseExponentBinary();
                expr = binaryMulDiv;
            }
            return expr;
        }

        private Model.Expression ParseExponentBinary()
        {
            Model.Expression expr = ParseIdentifier();
            while (!_scanner.IsEof() && _current == PunctuatorToken.Exponent)
            {
                var binaryExp = new Model.BinaryExpression
                    {
                        Operator = Model.OperatorType.Exponent,
                        Left = expr
                    };
                Expect(MiddleGroupAdditiveExponent);
                binaryExp.Right = ParseIdentifier();
                expr = binaryExp;
            }
            return expr;
        }

        private Model.Expression ParseIdentifier()
        {
            if (_current == null)
            {
                throw new ExpressionException(_scanner.Column, "Expected function or expression.");
            }

            if (!(_current is IdentifierToken))
            {
                return ParseUnary();
            }

            if (_scanner.PeekToken() == null || (_scanner.PeekToken() != null &&
                _scanner.PeekToken() != PunctuatorToken.LeftParenthesis))
            {
                // variable
                var varExpr = new Model.VariableExpression(_current.Text);
                if (!Kernel.Instance.BuiltIn.IsBuiltInVariable(varExpr.Name)) { _userVariables++; }
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

            var expr = new Model.FunctionExpression(_current.Text);
            if (!Kernel.Instance.BuiltIn.IsBuiltInFunction(expr.Name)) { _userFunctions++; }
            Expect(PunctuatorToken.LeftParenthesis);
            while (!_scanner.IsEof())
            {
                Consume();
                expr.Arguments.Add(ParseExpression(true));
                if (_current == null)
                {
                    break;
                }
                if (_current == PunctuatorToken.Comma)
                {
                    continue;
                }
                else if (_current == PunctuatorToken.RightParenthesis)
                {
                    Consume();
                    break;
                }
                else
                {
                    throw new ExpressionException(_scanner.Column, "Expected comma or close bracket.");
                }
            }
            return expr;
        }

        private Model.Expression ParseUnary()
        {
            if (_current == null)
            {
                throw new ExpressionException(_scanner.Column, "Expected unary operator, literal or open bracket.");    
            }

            var unary = new Model.UnaryExpression();
            if (_current == PunctuatorToken.Minus)
            {
                unary.Operator = Model.OperatorType.UnaryMinus;
                Expect(MiddleGroupUnary);
            }
            else if (_current == PunctuatorToken.Plus)
            {
                unary.Operator = Model.OperatorType.UnaryPlus;
                Expect(MiddleGroupUnary);
            }

            if (_current is LiteralToken)
            {
                unary.Value = new Model.LiteralExpression(((LiteralToken)_current).Value);
                if (_scanner.PeekToken() != null && _scanner.PeekToken() is LiteralToken)
                {
                    throw new ExpressionException(_scanner.Column, "Expected expression.");
                }
            }
            else if (_current == PunctuatorToken.LeftParenthesis)
            {
                unary.Value = ParseExpression();
            }
            else if (_current is IdentifierToken)
            {
                unary.Value = ParseIdentifier();
            }
            else
            {
                throw new ExpressionException(_scanner.Column, "Expected literal or open bracket.");
            }
            Consume();
            return unary;
        }

        #region Token Stream Controllers
        private void Ensure(Token[] tokens, bool identifierAllowed = true)
        {
            if (_current == null)
            {
                throw new ExpressionException(_scanner.Column,
                    string.Format("Unexpected end of input, instead of token(s): {0}{1}.", identifierAllowed ? "'IDENT', " : "", Token.StringOf(tokens)));
            }
            if (!(_current is LiteralToken || (identifierAllowed && _current is IdentifierToken )
                || tokens.Contains(_current)))
            {
                throw new ExpressionException(_scanner.Column,
                    string.Format("Syntax error, expected token(s) 'LITERAL', 'IDENT', {0}; but found '{1}'.", Token.StringOf(tokens), _current.Text));
            }
        }

        private void Expect(Token[] tokens, bool identifierAllowed = true)
        {
            var next = _scanner.PeekToken();
            if (next == null)
            {
                throw new ExpressionException(_scanner.Column,
                    string.Format("Unexpected end of input, instead of token(s): 'LITERAL', {0}{1}.", identifierAllowed ? "'IDENT', " : "" ,Token.StringOf(tokens)));
            }
            if (next is LiteralToken || (identifierAllowed && next is IdentifierToken ) ||
                tokens.Contains(next))
            {
                Consume();
            }
            else
            {
                throw new ExpressionException(_scanner.Column,
                    string.Format("Syntax error, expected token(s) 'LITERAL', {0}{1}; but found '{2}'.", identifierAllowed ? "'IDENT', " : "", Token.StringOf(tokens), next.Text));
            }
        }

        private void Expect(Token token)
        {
            var next = _scanner.PeekToken();
            if (next == null)
            {
                throw new ExpressionException(_scanner.Column,
                    string.Format("Unexpected end of input, instead of token(s): 'LITERAL', '{0}'.", token.Text));
            }
            if (token is LiteralToken || token == next)
            {
                Consume();
            }
            else
            {
                throw new ExpressionException(_scanner.Column,
                    string.Format("Syntax error, expected token(s) 'LITERAL', '{0}'; but found '{1}'.", token.Text, next.Text));
            }
        }

        private void Consume()
        {
            _current = _scanner.NextToken();
            if (_current == null) { return; }

            if (_current == PunctuatorToken.LeftParenthesis) { _brackets++; }
            else if (_current == PunctuatorToken.RightParenthesis) { _brackets--; }
        }
        #endregion

        ~Parser()
        {
            Dispose(false);
        }

        #region Token Groups
        private static readonly Token[] InitialGroup = { PunctuatorToken.Plus, PunctuatorToken.Minus, PunctuatorToken.LeftParenthesis, PunctuatorToken.Exponent }; // + literal, ident
        private static readonly Token[] InitialGroupWithComma = { PunctuatorToken.Comma, PunctuatorToken.Plus, PunctuatorToken.Minus, PunctuatorToken.LeftParenthesis, PunctuatorToken.Exponent }; // + literal, ident
        private static readonly Token[] MiddleGroupAdditiveExponent = { PunctuatorToken.Plus, PunctuatorToken.Minus, PunctuatorToken.LeftParenthesis, PunctuatorToken.RightParenthesis }; // + literal, ident
        private static readonly Token[] MiddleGroupMultiplicative = { PunctuatorToken.Plus, PunctuatorToken.Minus, PunctuatorToken.LeftParenthesis }; // + literal, ident
        private static readonly Token[] MiddleGroupIdentifier = { PunctuatorToken.Plus, PunctuatorToken.Minus, PunctuatorToken.Multiply, PunctuatorToken.Divide, PunctuatorToken.Exponent, PunctuatorToken.RightParenthesis, PunctuatorToken.Comma }; // + literal
        private static readonly Token[] MiddleGroupUnary = { PunctuatorToken.LeftParenthesis }; // + literal, ident
        #endregion

        private int _userVariables;
        private int _userFunctions;
        private bool _disposed;
        private int _brackets;
        private Token _current;
        private readonly Lexer _scanner;
    }
}