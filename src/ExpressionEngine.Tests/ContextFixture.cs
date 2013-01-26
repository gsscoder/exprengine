#region License
//
// Expression Engine Library: ContextFixture.cs
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
using NUnit.Framework;
using FluentAssertions;
#endregion

namespace ExpressionEngine.Tests
{
    [TestFixture]
    public sealed class ContextFixture
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExpressionWithNullTextThrowsException()
        {
            new Context().Evaluate(null);
        }

        //[Test]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void ExpressionWithNullTextThrowsException_2()
        //{
        //    new EeEngine().Evaluate(null, variables: null);
        //}

        //[Test]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void ExpressionWithNullTextThrowsException_3()
        //{
        //    new EeEngine().Evaluate(null, functions: null);
        //}

        //[Test]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void ExpressionWithNullTextThrowsException_4()
        //{
        //    new EeEngine().Evaluate(null, null, null);
        //}

        //[Test]
        //public void ExpressionWithoutUserDefinedNamesIsImmutable()
        //{
        //    Expression.Create("1 + (3 / 4) * (2 - 1)").Should().Be.OfType<Expression>();
        //}

        [Test]
        public void PlainAddition()
        {
            new Context().Evaluate("1 + 2").Should().Be(3D);
        }

        [Test]
        public void PlainAddition_2()
        {
            new Context().Evaluate("1 + 2 + 3").Should().Be(6D);
        }

        [Test]
        public void PlainAddition_3()
        {
            new Context().Evaluate("1 + 2 + 3 + 4").Should().Be(10D);
        }

        [Test]
        public void PlainSubtraction()
        {
            new Context().Evaluate("3 - 4").Should().Be(-1D);
        }

        [Test]
        public void PlainMultiplication()
        {
            new Context().Evaluate("3 * 9").Should().Be(27D);
        }

        [Test]
        public void PlainDivision()
        {
            new Context().Evaluate("16 / 4").Should().Be(4D);
        }

        [Test]
        public void PlainModulo()
        {
            new Context().Evaluate("10 % 4").Should().Be(2D);
        }

        [Test]
        public void SmallExpr()
        {
            new Context().Evaluate("1 + 2 * 3 / 1").Should().Be(7D);
        }

        [Test]
        public void SmallExprWithDecimal()
        {
            new Context().Evaluate(".90 - 3 * 4 + 11.123").Should().Be(0.022999999999999687D);
        }

        [Test]
        public void SmallExprWithModulo()
        {
            new Context().Evaluate("+0.90 - 3 * 4 + 11 - (123 % 23)").Should().Be(-8.1D);
        }

        [Test]
        public void SmallExprWithSignes()
        {
            new Context().Evaluate("-3 + -1").Should().Be(-4D);
        }

        [Test]
        public void SmallExprWithFunction()
        {
            new Context().Evaluate("sqrt(10) - 1").Should().Be(2.1622776601683795D);
        }

        [Test]
        public void SmallExprWithFunction_2()
        {
            new Context().Evaluate("sqrt(10000) / 2").Should().Be(50D);
        }

        [Test]
        public void SmallExprWithFunction_3()
        {
            new Context().Evaluate("sqrt(1000) % 3").Should().Be(1.6227766016837926D);
        }

        [Test]
        public void SmallExprWithFunction_4()
        {
            new Context().Evaluate("cos(999) % .3").Should().Be(0.099649852980826459D);
        }

        [Test]
        public void SmallExprWithFunction_Unary()
        {
            new Context().Evaluate("-sqrt(100)").Should().Be(-10D);
        }

        [Test]
        public void Expr()
        {
            new Context().Evaluate("3 * 0.31 / 19 + 10 - .7").Should().Be(9.3489473684210527D);
        }

        [Test]
        public void ExprWithBrackets()
        {
            new Context().Evaluate("1 + (2 - 1)").Should().Be(2D);
        }

        [Test]
        public void ExprWithBrackets_2()
        {
            new Context().Evaluate("3 * 0.31 / ((19 + 10) - .7)").Should().Be(0.032862190812720848D);
        }

        [Test]
        public void ExprWithBrackets_3()
        {
            new Context().Evaluate("3 * 0.31 / 19 + 10 - (1.7 % 2)").Should().Be(8.3489473684210527D);
        }

        [Test]
        public void ExprWithBrackets_Functions()
        {
            new Context().Evaluate("3 * 0.31 / ((19 + sqrt(1000.5 / 10)) - pow(.7, 2)) + 3").Should().Be(3.0326172734832215D);
        }

        [Test]
        public void UnaryBrackets()
        {
            new Context().Evaluate("-(1 + 2)").Should().Be(-3D);
        }

        [Test]
        public void Function()
        {
            new Context().Evaluate("pow(2, cos(90))").Should().Be(0.73302097391658683D);
        }

        [Test]
        public void Function_2()
        {
            new Context().Evaluate("pow(2, -atan(-33))").Should().Be(2.9089582019337388);
        }

        [Test]
        public void NestedFunctions()
        {
            new Context().Evaluate("log(log(10, 3))").Should().Be(0.73998461763125689D);
        }

        [Test]
        public void NestedFunctions_2()
        {
            new Context().Evaluate("cos(-log(100))").Should().Be(-0.10701348355876977D);
        }

        [Test]
        public void NestedFunctions_3()
        {
            new Context().Evaluate("sin(10.3) * cos(sqrt(sin(301 - sqrt(10))))").Should().Be(-0.55707344143373561D);
        }

        [Test]
        public void FunctionsWithNestedExpr()
        {
            new Context().Evaluate("sqrt(sqrt(10 * 3) - sqrt(1 + 3))").Should().Be(1.8647320384043551D);
        }

        [Test]
        public void FunctionsWithNestedExpr_2()
        {
            new Context().Evaluate("pow((10 * 3), -(1 + log(10 * 10)))").Should().Be(0.0000000052539263674376282D);
        }

        [Test]
        public void NegativePositiveDecimalBracket()
        {
            new Context().Evaluate(".2 / +9.99 * (-.123 + -2)").Should().Be(-0.042502502502502509D);
        }

        [Test]
        public void NegativePositiveDecimalBracket_2()
        {
            new Context().Evaluate("-.23 + +.99 * ((-.123 + -2.1)--333)").Should().Be(327.23922999999996D);
        }

        [Test]
        public void Exponent()
        {
            new Context().Evaluate("pow(10, 2)").Should().Be(100D);
        }

        [Test]
        public void ExponentWithNestedExpr()
        {
            new Context().Evaluate("pow((10 * 3), -(1 + 3))").Should().Be(0.0000012345679012345679D);
        }

        [Test]
        public void Variable()
        {
            new Context().Evaluate("pi - 3").Should().Be(0.14159265358979312D);
        }

        [Test]
        public void Variable_2()
        {
            new Context().Evaluate("(e + pi) - 5.8598744820488378").Should().Be(0D);
        }

        [Test]
        public void Variable_3()
        {
            new Context().Evaluate("-e").Should().Be(-2.7182818284590451D);
        }

        [Test]
        public void Variable_Function()
        {
            new Context().Evaluate("10 - (pi * atan(10)) - -e").Should().Be(8.0965979343737935D);
        }

        //[Test]
        //public void CachingExpression()
        //{
        //    var expr = Expression.Create(".123 + .345");
        //    expr.IsValueCacheRetrieved.Should().Be.False();
        //    Expression.Create("1 + 2 + 3"); // anather expression...
        //    expr.IsValueCacheRetrieved.Should().Be.False();
        //    expr = null;
        //    var expr2 = Expression.Create(".123+.345"); // same as first, but with no spaces
        //    expr2.IsValueCacheRetrieved.Should().BeTrue();
        //}

        #region Expected Exceptions
        [Test]
        [ExpectedException(typeof(EvaluatorException))] //ExpectedMessage = "Unexpected end of input, instead of token(s): 'IDENT', 'LITERAL', '+', '-', '('.")]
        public void IncompleteExpressioThrowsException()
        {
            new Context().Evaluate("3 + 1 - (");
        }

        [Test]
        [ExpectedException(typeof(EvaluatorException), ExpectedMessage = "Expected expression.")]
        public void IncompleteExpressioThrowsException_2()
        {
            new Context().Evaluate("1 2");
        }

        [Test]
        [ExpectedException(typeof(EvaluatorException), ExpectedMessage = "Syntax error, odd number of brackets.")]
        public void MissingBrackethrowsException()
        {
            new Context().Evaluate("3 + (1 -");
        }

        [Test]
        [ExpectedException(typeof(EvaluatorException), ExpectedMessage = "Syntax error, odd number of brackets.")]
        public void MissingBrackethrowsException_2()
        {
            new Context().Evaluate("3 + 3 / (1");
        }
        #endregion
    }
}