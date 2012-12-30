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
using System;
using System.IO;
using NUnit.Framework;
using Should.Fluent;
using ExpressionEngine.Core;
#endregion

namespace ExpressionEngine.Tests
{
    public class ScannerFixture
    {
        [Test]
        public void Expr()
        {
            var scanner = new Scanner(new StringReader("((10 + 1 - (3 - .234)) * 300)  /  10.1"));

            scanner.NextToken().ShouldPunctuatorEqual(TokenType.OpenBracket, "(");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.OpenBracket, "(");
            scanner.NextToken().ShouldLiteralEqual("10");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.Plus, "+");
            scanner.NextToken().ShouldLiteralEqual("1");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.Minus, "-");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.OpenBracket, "(");
            scanner.NextToken().ShouldLiteralEqual("3");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.Minus, "-");
            scanner.NextToken().ShouldLiteralEqual("0.234");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.CloseBracket, ")");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.CloseBracket, ")");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.Star, "*");
            scanner.NextToken().ShouldLiteralEqual("300");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.CloseBracket, ")");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.Slash, "/");
            scanner.NextToken().ShouldLiteralEqual("10.1");
            scanner.NextToken().Should().Be.Null();
        }

        [Test]
        public void SmallExprUsingPeek()
        {
            var scanner = new Scanner(new StringReader("1 + 3"));

            scanner.PeekToken().ShouldLiteralEqual("1");

            scanner.NextToken().ShouldLiteralEqual("1");

            scanner.PeekToken().ShouldPunctuatorEqual(TokenType.Plus, "+");

            scanner.NextToken().ShouldPunctuatorEqual(TokenType.Plus, "+");

            scanner.PeekToken().ShouldLiteralEqual("3");

            scanner.NextToken().ShouldLiteralEqual("3");

            scanner.PeekToken().Should().Be.Null();
            scanner.NextToken().Should().Be.Null();
        }

        [Test]
        public void SingleNumberExpr()
        {
            var scanner = new Scanner(new StringReader("123"));

            scanner.PeekToken().ShouldLiteralEqual("123");

            scanner.NextToken().ShouldLiteralEqual("123");

            scanner.PeekToken().Should().Be.Null();
            scanner.NextToken().Should().Be.Null();
        }

        [Test]
        public void SingleDecimalNumberExpr()
        {
            var scanner = new Scanner(new StringReader(".123"));

            scanner.PeekToken().ShouldLiteralEqual("0.123");

            scanner.NextToken().ShouldLiteralEqual("0.123");

            scanner.PeekToken().Should().Be.Null();
            scanner.NextToken().Should().Be.Null();
        }

        [Test]
        public void SinglePositiveNumberExpr()
        {
            var scanner = new Scanner(new StringReader("+123"));

			scanner.PeekToken().ShouldPunctuatorEqual(TokenType.Plus, "+");
			scanner.NextToken().ShouldPunctuatorEqual(TokenType.Plus, "+");

            scanner.PeekToken().ShouldLiteralEqual("123");
            scanner.NextToken().ShouldLiteralEqual("123");

            scanner.PeekToken().Should().Be.Null();
            scanner.NextToken().Should().Be.Null();
        }

        [Test]
        public void SinglePositiveDecimalNumberExpr()
        {
            var scanner = new Scanner(new StringReader("+.123"));

			scanner.PeekToken().ShouldPunctuatorEqual(TokenType.Plus, "+");
			scanner.NextToken().ShouldPunctuatorEqual(TokenType.Plus, "+");

            scanner.PeekToken().ShouldLiteralEqual("0.123");
            scanner.NextToken().ShouldLiteralEqual("0.123");

            scanner.PeekToken().Should().Be.Null();
            scanner.NextToken().Should().Be.Null();
        }

        [Test]
        public void SingleNegativeNumberExpr()
        {
            var scanner = new Scanner(new StringReader("-123"));

            scanner.NextToken().ShouldPunctuatorEqual(TokenType.Minus, "-");
            scanner.NextToken().ShouldLiteralEqual("123");
        }

        [Test]
        public void SingleNegativeDecimalNumberExpr()
        {
            var scanner = new Scanner(new StringReader("-.123"));

			scanner.NextToken().ShouldPunctuatorEqual(TokenType.Minus, "-");
            scanner.NextToken().ShouldLiteralEqual("0.123");
        }

        [Test]
        public void NextTokenIncrementsPosition()
        {
            var scanner = new Scanner(new StringReader(".3 / 9.99"));
            scanner.Position.Should().Equal(0);

            scanner.PeekToken().ShouldLiteralEqual("0.3");
            scanner.Position.Should().Equal(0);
            scanner.NextToken().ShouldLiteralEqual("0.3");
            scanner.Position.Should().Equal(1);

            scanner.PeekToken().ShouldPunctuatorEqual(TokenType.Slash, "/");
            scanner.Position.Should().Equal(1);
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.Slash, "/");
            scanner.Position.Should().Equal(2);

            scanner.PeekToken().ShouldLiteralEqual("9.99");
            scanner.Position.Should().Equal(2);
            scanner.NextToken().ShouldLiteralEqual("9.99");
            scanner.Position.Should().Equal(3);

            scanner.PeekToken().Should().Be.Null();
            scanner.Position.Should().Equal(3);
            scanner.NextToken().Should().Be.Null();
            scanner.Position.Should().Equal(-1);
        }

        [Test]
        public void NegativePositiveDecimalBracket()
        {
            var scanner = new Scanner(new StringReader(".2 / +9.99 * (-.123 + -2)"));

            scanner.NextToken().ShouldLiteralEqual("0.2");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.Slash, "/");
			scanner.NextToken().ShouldPunctuatorEqual(TokenType.Plus, "+");
            scanner.NextToken().ShouldLiteralEqual("9.99");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.Star, "*");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.OpenBracket, "(");
			scanner.NextToken().ShouldPunctuatorEqual(TokenType.Minus, "-");
            scanner.NextToken().ShouldLiteralEqual("0.123");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.Plus, "+");
			scanner.NextToken().ShouldPunctuatorEqual(TokenType.Minus, "-");
            scanner.NextToken().ShouldLiteralEqual("2");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.CloseBracket, ")");

            scanner.NextToken().Should().Be.Null();
        }

        [Test]
        public void NegativePositiveDecimalBracket_2()
        {
            var scanner = new Scanner(new StringReader("-.23 + +.99 * ((-.123 + -2.1)--333)"));

			scanner.NextToken().ShouldPunctuatorEqual(TokenType.Minus, "-");
            scanner.NextToken().ShouldLiteralEqual("0.23");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.Plus, "+");
			scanner.NextToken().ShouldPunctuatorEqual(TokenType.Plus, "+");
            scanner.NextToken().ShouldLiteralEqual("0.99");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.Star, "*");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.OpenBracket, "(");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.OpenBracket, "(");
			scanner.NextToken().ShouldPunctuatorEqual(TokenType.Minus, "-");
            scanner.NextToken().ShouldLiteralEqual("0.123");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.Plus, "+");
			scanner.NextToken().ShouldPunctuatorEqual(TokenType.Minus, "-");
            scanner.NextToken().ShouldLiteralEqual("2.1");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.CloseBracket, ")");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.Minus, "-");
			scanner.NextToken().ShouldPunctuatorEqual(TokenType.Minus, "-");
            scanner.NextToken().ShouldLiteralEqual("333");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.CloseBracket, ")");

            scanner.NextToken().Should().Be.Null();
        }

        [Test]
        public void SmallExprWithFunction()
        {
            var scanner = new Scanner(new StringReader("3 + pow(10, 2)"));

            scanner.NextToken().ShouldLiteralEqual("3");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.Plus, "+");
            scanner.NextToken().ShouldIdentifierEqual("pow");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.OpenBracket, "(");
            scanner.NextToken().ShouldLiteralEqual("10");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.Comma, ",");
            scanner.NextToken().ShouldLiteralEqual("2");
            scanner.NextToken().ShouldPunctuatorEqual(TokenType.CloseBracket, ")");

            scanner.NextToken().Should().Be.Null();
        }

		#region Expected Exceptions
		[Test]
		[ExpectedException(typeof(ExpressionException), ExpectedMessage="Line terminator is not allowed.")]
		public void LineEndingThrowsException()
		{
			new Scanner(new StringReader(string.Concat(new string(' ', 10), Environment.NewLine)));
		}
		#endregion
    }
}