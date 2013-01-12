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
using System.IO;
using UnitTest = NUnit.Framework;
using Should.Fluent;
using ExpressionEngine.Internal;
#endregion

namespace ExpressionEngine.Tests
{
    [UnitTest.TestFixture]
    public class ScannerFixture
    {
        [UnitTest.Test]
        public void Expr()
        {
            var scanner = new Lexer(Text.OfString("((10 + 1 - (3 - .234)) * 300)  /  10.1"));

            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.LeftParenthesis);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.LeftParenthesis);
            ((LiteralToken) scanner.NextToken()).Value.Should().Equal(10L);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(1L);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Minus);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.LeftParenthesis);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(3L);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Minus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(.234D);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.RightParenthesis);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.RightParenthesis);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Multiply);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(300L);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.RightParenthesis);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Divide);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(10.1D);
            scanner.NextToken().Should().Be.Null();
        }

        [UnitTest.Test]
        public void SmallExprUsingPeek()
        {
            var scanner = new Lexer(Text.OfString("1 + 3"));

            ((LiteralToken)scanner.PeekToken()).Value.Should().Equal(1L);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(1L);

            scanner.PeekToken().Should().Be.SameAs(PunctuatorToken.Plus);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Plus);

            ((LiteralToken)scanner.PeekToken()).Value.Should().Equal(3L);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(3L);

            scanner.PeekToken().Should().Be.Null();
            scanner.NextToken().Should().Be.Null();
        }

        [UnitTest.Test]
        public void SmallExprWithModulo()
        {
            var scanner = new Lexer(Text.OfString("10 % 2"));

            ((LiteralToken)scanner.PeekToken()).Value.Should().Equal(10L);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(10L);

            scanner.PeekToken().Should().Be.SameAs(PunctuatorToken.Modulo);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Modulo);

            ((LiteralToken)scanner.PeekToken()).Value.Should().Equal(2L);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(2L);

            scanner.PeekToken().Should().Be.Null();
            scanner.NextToken().Should().Be.Null();
        }

        [UnitTest.Test]
        public void SingleNumberExpr()
        {
            var scanner = new Lexer(Text.OfString("123"));

            ((LiteralToken)scanner.PeekToken()).Value.Should().Equal(123L);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(123L);

            scanner.PeekToken().Should().Be.Null();
            scanner.NextToken().Should().Be.Null();
        }

        [UnitTest.Test]
        public void SingleDecimalNumberExpr()
        {
            var scanner = new Lexer(Text.OfString(".123"));

            ((LiteralToken)scanner.PeekToken()).Value.Should().Equal(.123D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(.123D);

            scanner.PeekToken().Should().Be.Null();
            scanner.NextToken().Should().Be.Null();
        }

        [UnitTest.Test]
        public void SinglePositiveNumberExpr()
        {
            var scanner = new Lexer(Text.OfString("+123"));

            scanner.PeekToken().Should().Be.SameAs(PunctuatorToken.Plus);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Plus);

            ((LiteralToken)scanner.PeekToken()).Value.Should().Equal(123L);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(123L);

            scanner.PeekToken().Should().Be.Null();
            scanner.NextToken().Should().Be.Null();
        }

        [UnitTest.Test]
        public void SinglePositiveDecimalNumberExpr()
        {
            var scanner = new Lexer(Text.OfString("+.123"));

            scanner.PeekToken().Should().Be.SameAs(PunctuatorToken.Plus);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Plus);

            ((LiteralToken)scanner.PeekToken()).Value.Should().Equal(.123D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(.123D);

            scanner.PeekToken().Should().Be.Null();
            scanner.NextToken().Should().Be.Null();
        }

        [UnitTest.Test]
        public void SingleNegativeNumberExpr()
        {
            var scanner = new Lexer(Text.OfString("-123"));

            scanner.PeekToken().Should().Be.SameAs(PunctuatorToken.Minus);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Minus);

            ((LiteralToken)scanner.PeekToken()).Value.Should().Equal(123L);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(123L);
        }

        [UnitTest.Test]
        public void SingleNegativeDecimalNumberExpr()
        {
            var scanner = new Lexer(Text.OfString("-.123"));

            scanner.PeekToken().Should().Be.SameAs(PunctuatorToken.Minus);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Minus);

            ((LiteralToken)scanner.PeekToken()).Value.Should().Equal(.123D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(.123D);
        }

        //[UnitTest.Test]
        //public void NextTokenIncrementsPosition()
        //{
        //    var scanner = new Lexer(Text.OfString(".3 / 9.99"));
        //    scanner.Position.Should().Equal(0);

        //    ((LiteralToken)scanner.PeekToken()).Value.Should().Equal(.3D);
        //    scanner.Position.Should().Equal(0);
        //    ((LiteralToken)scanner.NextToken()).Value.Should().Equal(.3D);
        //    scanner.Position.Should().Equal(1);

        //    scanner.PeekToken().Should().Be.SameAs(PunctuatorToken.Divide);
        //    scanner.Position.Should().Equal(1);
        //    scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Divide);
        //    scanner.Position.Should().Equal(2);

        //    ((LiteralToken)scanner.PeekToken()).Value.Should().Equal(9.99D);
        //    scanner.Position.Should().Equal(2);
        //    ((LiteralToken)scanner.NextToken()).Value.Should().Equal(9.99D);
        //    scanner.Position.Should().Equal(3);

        //    scanner.PeekToken().Should().Be.Null();
        //    scanner.Position.Should().Equal(3);
        //    scanner.NextToken().Should().Be.Null();
        //    scanner.Position.Should().Equal(-1);
        //}

        [UnitTest.Test]
        public void NegativePositiveDecimalBracket()
        {
            var scanner = new Lexer(Text.OfString(".2 / +9.99 * (-.123 + -2)"));

            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(.2D);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Divide);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(9.99D);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Multiply);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.LeftParenthesis);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Minus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(.123D);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Plus);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Minus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(2L);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.RightParenthesis);

            scanner.NextToken().Should().Be.Null();
        }

        [UnitTest.Test]
        public void NegativePositiveDecimalBracket_2()
        {
            var scanner = new Lexer(Text.OfString("-.23 + +.99 * ((-.123 + -2.1)--333)"));

            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Minus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(.23D);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Plus);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(.99D);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Multiply);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.LeftParenthesis);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.LeftParenthesis);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Minus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(.123D);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Plus);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Minus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(2.1D);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.RightParenthesis);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Minus); ;
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Minus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(333L);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.RightParenthesis);

            scanner.NextToken().Should().Be.Null();
        }

        [UnitTest.Test]
        public void SmallExprWithFunction()
        {
            var scanner = new Lexer(Text.OfString("3 + pow(10, 2)"));

            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(3L);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Plus);
            ((IdentifierToken)scanner.NextToken()).Text.Should().Equal("pow");
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.LeftParenthesis);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(10L);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Comma);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(2L);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.RightParenthesis);

            scanner.NextToken().Should().Be.Null();
        }

        [UnitTest.Test]
        public void Caret()
        {
            var scanner = new Lexer(Text.OfString("10^2"));

            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(10L);
            scanner.NextToken().Should().Be.SameAs(PunctuatorToken.Exponent);
            ((LiteralToken)scanner.NextToken()).Value.Should().Equal(2L);

            scanner.NextToken().Should().Be.Null();
        }

        [UnitTest.Test]
        public void Underscore()
        {
            var scanner = new Lexer(Text.OfString("var_name"));

            ((IdentifierToken)scanner.NextToken()).Text.Should().Equal("var_name");
        }

        #region Expected Exceptions
        [UnitTest.Test]
        [UnitTest.ExpectedException(typeof(ExpressionException), ExpectedMessage = "Line terminator is not allowed.")]
        public void LineEndingThrowsException()
        {
            new Lexer(Text.OfString(string.Concat(new string(' ', 10), Environment.NewLine)));
        }
        #endregion
    }
}