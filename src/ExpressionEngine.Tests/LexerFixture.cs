#region License
//
// Expression Engine Library: ScannerFixture.cs
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
using UnitTest = NUnit.Framework; // avoid name conflict with local Text type
using FluentAssertions;
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

            scanner.NextToken().Type.Should().Be(TokenType.LeftParenthesis);
            scanner.NextToken().Type.Should().Be(TokenType.LeftParenthesis);
            ((LiteralToken) scanner.NextToken()).Value.Should().Be(10D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(1D);
            scanner.NextToken().Type.Should().Be(TokenType.Minus);
            scanner.NextToken().Type.Should().Be(TokenType.LeftParenthesis);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(3D);
            scanner.NextToken().Type.Should().Be(TokenType.Minus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(.234D);
            scanner.NextToken().Type.Should().Be(TokenType.RightParenthesis);
            scanner.NextToken().Type.Should().Be(TokenType.RightParenthesis);
            scanner.NextToken().Type.Should().Be(TokenType.Multiply);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(300D);
            scanner.NextToken().Type.Should().Be(TokenType.RightParenthesis);
            scanner.NextToken().Type.Should().Be(TokenType.Divide);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(10.1D);
            scanner.NextToken().Should().BeNull();
        }

        [UnitTest.Test]
        public void SmallExprUsingPeek()
        {
            var scanner = new Lexer(Text.OfString("1 + 3"));

            ((LiteralToken)scanner.PeekToken()).Value.Should().Be(1D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(1D);

            scanner.PeekToken().Type.Should().Be(TokenType.Plus);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);

            ((LiteralToken)scanner.PeekToken()).Value.Should().Be(3D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(3D);

            scanner.PeekToken().Should().BeNull();
            scanner.NextToken().Should().BeNull();
        }

        [UnitTest.Test]
        public void SmallExprWithModulo()
        {
            var scanner = new Lexer(Text.OfString("10 % 2"));

            ((LiteralToken)scanner.PeekToken()).Value.Should().Be(10D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(10D);

            scanner.PeekToken().Type.Should().Be(TokenType.Modulo);
            scanner.NextToken().Type.Should().Be(TokenType.Modulo);

            ((LiteralToken)scanner.PeekToken()).Value.Should().Be(2D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(2D);

            scanner.PeekToken().Should().BeNull();
            scanner.NextToken().Should().BeNull();
        }

        [UnitTest.Test]
        public void SingleNumberExpr()
        {
            var scanner = new Lexer(Text.OfString("123"));

            ((LiteralToken)scanner.PeekToken()).Value.Should().Be(123D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(123D);

            scanner.PeekToken().Should().BeNull();
            scanner.NextToken().Should().BeNull();
        }

        [UnitTest.Test]
        public void SingleDecimalNumberExpr()
        {
            var scanner = new Lexer(Text.OfString(".123"));

            ((LiteralToken)scanner.PeekToken()).Value.Should().Be(.123D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(.123D);

            scanner.PeekToken().Should().BeNull();
            scanner.NextToken().Should().BeNull();
        }

        [UnitTest.Test]
        public void SinglePositiveNumberExpr()
        {
            var scanner = new Lexer(Text.OfString("+123"));

            scanner.PeekToken().Type.Should().Be(TokenType.Plus);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);

            ((LiteralToken)scanner.PeekToken()).Value.Should().Be(123D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(123D);

            scanner.PeekToken().Should().BeNull();
            scanner.NextToken().Should().BeNull();
        }

        [UnitTest.Test]
        public void SinglePositiveDecimalNumberExpr()
        {
            var scanner = new Lexer(Text.OfString("+.123"));

            scanner.PeekToken().Type.Should().Be(TokenType.Plus);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);

            ((LiteralToken)scanner.PeekToken()).Value.Should().Be(.123D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(.123D);

            scanner.PeekToken().Should().BeNull();
            scanner.NextToken().Should().BeNull();
        }

        [UnitTest.Test]
        public void SingleNegativeNumberExpr()
        {
            var scanner = new Lexer(Text.OfString("-123"));

            scanner.PeekToken().Type.Should().Be(TokenType.Minus);
            scanner.NextToken().Type.Should().Be(TokenType.Minus);

            ((LiteralToken)scanner.PeekToken()).Value.Should().Be(123D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(123D);
        }

        [UnitTest.Test]
        public void SingleNegativeDecimalNumberExpr()
        {
            var scanner = new Lexer(Text.OfString("-.123"));

            scanner.PeekToken().Type.Should().Be(TokenType.Minus);
            scanner.NextToken().Type.Should().Be(TokenType.Minus);

            ((LiteralToken)scanner.PeekToken()).Value.Should().Be(.123D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(.123D);
        }

        //[UnitTest.Test]
        //public void NextTokenIncrementsPosition()
        //{
        //    var scanner = new Lexer(Text.OfString(".3 / 9.99"));
        //    scanner.Position.Should().Be(0);

        //    ((LiteralToken)scanner.PeekToken()).Value.Should().Be(.3D);
        //    scanner.Position.Should().Be(0);
        //    ((LiteralToken)scanner.NextToken()).Value.Should().Be(.3D);
        //    scanner.Position.Should().Be(1);

        //    scanner.PeekToken().Type.Should().Be(TokenType.Divide);
        //    scanner.Position.Should().Be(1);
        //    scanner.NextToken().Type.Should().Be(TokenType.Divide);
        //    scanner.Position.Should().Be(2);

        //    ((LiteralToken)scanner.PeekToken()).Value.Should().Be(9.99D);
        //    scanner.Position.Should().Be(2);
        //    ((LiteralToken)scanner.NextToken()).Value.Should().Be(9.99D);
        //    scanner.Position.Should().Be(3);

        //    scanner.PeekToken().Should().BeNull();
        //    scanner.Position.Should().Be(3);
        //    scanner.NextToken().Should().BeNull();
        //    scanner.Position.Should().Be(-1);
        //}

        [UnitTest.Test]
        public void NegativePositiveDecimalBracket()
        {
            var scanner = new Lexer(Text.OfString(".2 / +9.99 * (-.123 + -2)"));

            ((LiteralToken)scanner.NextToken()).Value.Should().Be(.2D);
            scanner.NextToken().Type.Should().Be(TokenType.Divide);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(9.99D);
            scanner.NextToken().Type.Should().Be(TokenType.Multiply);
            scanner.NextToken().Type.Should().Be(TokenType.LeftParenthesis);
            scanner.NextToken().Type.Should().Be(TokenType.Minus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(.123D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            scanner.NextToken().Type.Should().Be(TokenType.Minus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(2D);
            scanner.NextToken().Type.Should().Be(TokenType.RightParenthesis);

            scanner.NextToken().Should().BeNull();
        }

        [UnitTest.Test]
        public void NegativePositiveDecimalBracket_2()
        {
            var scanner = new Lexer(Text.OfString("-.23 + +.99 * ((-.123 + -2.1)--333)"));

            scanner.NextToken().Type.Should().Be(TokenType.Minus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(.23D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(.99D);
            scanner.NextToken().Type.Should().Be(TokenType.Multiply);
            scanner.NextToken().Type.Should().Be(TokenType.LeftParenthesis);
            scanner.NextToken().Type.Should().Be(TokenType.LeftParenthesis);
            scanner.NextToken().Type.Should().Be(TokenType.Minus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(.123D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            scanner.NextToken().Type.Should().Be(TokenType.Minus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(2.1D);
            scanner.NextToken().Type.Should().Be(TokenType.RightParenthesis);
            scanner.NextToken().Type.Should().Be(TokenType.Minus); ;
            scanner.NextToken().Type.Should().Be(TokenType.Minus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(333D);
            scanner.NextToken().Type.Should().Be(TokenType.RightParenthesis);

            scanner.NextToken().Should().BeNull();
        }

        [UnitTest.Test]
        public void SmallExprWithFunction()
        {
            var scanner = new Lexer(Text.OfString("3 + pow(10, 2)"));

            ((LiteralToken)scanner.NextToken()).Value.Should().Be(3D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((IdentifierToken)scanner.NextToken()).Text.Should().Be("pow");
            scanner.NextToken().Type.Should().Be(TokenType.LeftParenthesis);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(10D);
            scanner.NextToken().Type.Should().Be(TokenType.Comma);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(2D);
            scanner.NextToken().Type.Should().Be(TokenType.RightParenthesis);

            scanner.NextToken().Should().BeNull();
        }

        //[UnitTest.Test]
        //public void Caret()
        //{
        //    var scanner = new Lexer(Text.OfString("10^2"));

        //    ((LiteralToken)scanner.NextToken()).Value.Should().Be(10D);
        //    scanner.NextToken().Type.Should().Be(TokenType.Exponent);
        //    ((LiteralToken)scanner.NextToken()).Value.Should().Be(2D);

        //    scanner.NextToken().Should().BeNull();
        //}

        [UnitTest.Test]
        public void Underscore()
        {
            var scanner = new Lexer(Text.OfString("var_name"));

            ((IdentifierToken)scanner.NextToken()).Text.Should().Be("var_name");
        }

        [UnitTest.Test]
        public void Equality()
        {
            var scanner = new Lexer(Text.OfString("var1 == 10.3"));

            ((IdentifierToken)scanner.NextToken()).Text.Should().Be("var1");
            scanner.NextToken().Type.Should().Be(TokenType.Equality);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(10.3D);
        }

        [UnitTest.Test]
        public void Inequality()
        {
            var scanner = new Lexer(Text.OfString(".3 != 0.003003"));

            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0.3D);
            scanner.NextToken().Type.Should().Be(TokenType.Inequality);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0.003003D);
        }

        [UnitTest.Test]
        public void LessThan()
        {
            var scanner = new Lexer(Text.OfString("10 < (10 * 2.2)"));

            ((LiteralToken)scanner.NextToken()).Value.Should().Be(10D);
            scanner.NextToken().Type.Should().Be(TokenType.LessThan);
            scanner.NextToken().Type.Should().Be(TokenType.LeftParenthesis);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(10D);
            scanner.NextToken().Type.Should().Be(TokenType.Multiply);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(2.2D);
            scanner.NextToken().Type.Should().Be(TokenType.RightParenthesis);
        }

        [UnitTest.Test]
        public void LessThanOrEqual()
        {
            var scanner = new Lexer(Text.OfString(".3 <= .9"));

            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0.3D);
            scanner.NextToken().Type.Should().Be(TokenType.LessThanOrEqual);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0.9D);
        }

        [UnitTest.Test]
        public void GreaterThan()
        {
            var scanner = new Lexer(Text.OfString("10 > (10 * 2.2)"));

            ((LiteralToken)scanner.NextToken()).Value.Should().Be(10D);
            scanner.NextToken().Type.Should().Be(TokenType.GreaterThan);
            scanner.NextToken().Type.Should().Be(TokenType.LeftParenthesis);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(10D);
            scanner.NextToken().Type.Should().Be(TokenType.Multiply);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(2.2D);
            scanner.NextToken().Type.Should().Be(TokenType.RightParenthesis);
        }

        [UnitTest.Test]
        public void GreaterThanOrEqual()
        {
            var scanner = new Lexer(Text.OfString("0.1 >= +5.9"));

            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0.1D);
            scanner.NextToken().Type.Should().Be(TokenType.GreaterThanOrEqual);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(5.9D);
        }

        [UnitTest.Test]
        public void True()
        {
            var scanner = new Lexer(Text.OfString("true == 1"));

            ((LiteralToken)scanner.NextToken()).Value.Should().Be(true);
            scanner.NextToken().Type.Should().Be(TokenType.Equality);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(1D);
        }

        [UnitTest.Test]
        public void False()
        {
            var scanner = new Lexer(Text.OfString("false != .01"));

            ((LiteralToken)scanner.NextToken()).Value.Should().Be(false);
            scanner.NextToken().Type.Should().Be(TokenType.Inequality);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0.01D);
        }

        [UnitTest.Test]
        public void LiteralWithExponent()
        {
            var scanner = new Lexer(Text.OfString("1 + 10E2 + 3"));

            ((LiteralToken)scanner.NextToken()).Value.Should().Be(1D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(1000D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(3D);
        }

        [UnitTest.Test]
        public void LiteralWithExponentUsingPlusSign()
        {
            var scanner = new Lexer(Text.OfString("1 + 10E+2"));

            ((LiteralToken)scanner.NextToken()).Value.Should().Be(1D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(1000D);
        }

        [UnitTest.Test]
        public void LiteralWithLowerCaseExponent()
        {
            var scanner = new Lexer(Text.OfString("10e2 + 4"));

            ((LiteralToken)scanner.NextToken()).Value.Should().Be(1000D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(4D);
        }

        [UnitTest.Test]
        public void LiteralWithLowerCaseExponentUsingPlusSign()
        {
            var scanner = new Lexer(Text.OfString("1 + 10e+2 + 4"));

            ((LiteralToken)scanner.NextToken()).Value.Should().Be(1D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(1000D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(4D);
        }

        [UnitTest.Test]
        public void LiteralsWithNegativeExponent()
        {
            var scanner = new Lexer(Text.OfString("10E-11 + 10e-11"));

            ((LiteralToken) scanner.NextToken()).Value.Should().Be(0.0000000001D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0.0000000001D);
        }

        [UnitTest.Test]
        public void StringLiteral()
        {
            var scanner = new Lexer(Text.OfString("\"hello, csharp\""));

            ((LiteralToken)scanner.NextToken()).Value.Should().Be("hello, csharp");
        }

        [UnitTest.Test]
        public void StringLiteral_2()
        {
            var scanner = new Lexer(Text.OfString(".3 + \"hello, fsharp\" + 0.999"));

            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0.3D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be("hello, fsharp");
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0.999D);
        }

        [UnitTest.Test]
        public void StringLiteralWithEscapeCharacters()
        {
            var scanner = new Lexer(Text.OfString(@"0 + ""\t \n \r"" + 0"));

            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be("\t \n \r");
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0D);
        }

        [UnitTest.Test]
        public void StringLiteralWithDecimalEscape()
        {
            var scanner = new Lexer(Text.OfString(@" ""\048\048\055"" "));

            ((LiteralToken)scanner.NextToken()).Value.Should().Be("007");
        }

        [UnitTest.Test]
        public void StringLiteralWithDoubleQuoteEscapeCharacter()
        {
            var scanner = new Lexer(Text.OfString(@" ""a\""bc"" "));

            ((LiteralToken)scanner.NextToken()).Value.Should().Be("a\"bc");
        }

        #region Expected Exceptions
        [UnitTest.Test]
        [UnitTest.ExpectedException(typeof(EvaluatorException), ExpectedMessage = "Line terminator is not allowed.")]
        public void LineEndingThrowsException()
        {
            new Lexer(Text.OfString(string.Concat(new string(' ', 10), Environment.NewLine)));
        }
        #endregion
    }
}