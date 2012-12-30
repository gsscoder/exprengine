using NUnit.Framework;
using Should.Fluent;

namespace ExpressionEngine.Tests
{
    [TestFixture]
    public sealed class ExpressionEvaluatorFixture
    {
        [Test]
        public void PlainAddition()
        {
            Expression.Create("1 + 2").Value.Should().Equal(3D);
        }

        [Test]
        public void PlainAddition_2()
        {
            Expression.Create("1 + 2 + 3").Value.Should().Equal(6D);
        }

        [Test]
        public void PlainAddition_3()
        {
            Expression.Create("1 + 2 + 3 + 4").Value.Should().Equal(10D);
        }

        [Test]
        public void PlainSubtraction()
        {
            Expression.Create("3 - 4").Value.Should().Equal(-1D);
        }

        [Test]
        public void PlainMultiplication()
        {
            Expression.Create("3 * 9").Value.Should().Equal(27D);
        }

        [Test]
        public void PlainDivision()
        {
            Expression.Create("16 / 4").Value.Should().Equal(4D);
        }

        [Test]
        public void SmallExpr()
        {
            Expression.Create("1 + 2 * 3 / 1").Value.Should().Equal(7D);
        }

        [Test]
        public void SmallExprWithDecimal()
        {
            Expression.Create(".90 - 3 * 4 + 11.123").Value.Should().Equal(0.022999999999999687D);
        }

        [Test]
        public void SmallExprWithSignes()
        {
            Expression.Create("-3 + -1").Value.Should().Equal(-4D);
        }

        [Test]
        public void SmallExprWithFunction()
        {
            Expression.Create("pow(10, 2) - 1").Value.Should().Equal(99D);
        }

        [Test]
        public void SmallExprWithFunction_2()
        {
            Expression.Create("sqrt(10000) / 2").Value.Should().Equal(50D);
        }

		[Test]
        public void SmallExprWithFunction_Unary()
        {
            Expression.Create("-pow(10, 2)").Value.Should().Equal(-100D);
        }

        [Test]
        public void Expr()
        {
            Expression.Create("3 * 0.31 / 19 + 10 - .7").Value.Should().Equal(9.3489473684210527D);
        }

        [Test]
        public void ExprWithBrackets()
        {
            Expression.Create("1 + (2 - 1)").Value.Should().Equal(2D);
        }

        [Test]
        public void ExprWithBrackets_2()
        {
            Expression.Create("3 * 0.31 / ((19 + 10) - .7)").Value.Should().Equal(0.032862190812720848D);
        }

        [Test]
        public void ExprWithBrackets_Functions()
        {
            Expression.Create("3 * 0.31 / ((19 + sqrt(1000.5 / 10)) - pow(.7, 2)) + 3").Value.Should().Equal(3.0326172734832215D);
        }

        [Test]
        public void UnaryBrackets()
        {
            Expression.Create("-(1 + 2)").Value.Should().Equal(-3D);
        }

        [Test]
        public void NestedFunctions()
        {
            Expression.Create("sqrt(pow(10, 3))").Value.Should().Equal(31.622776601683793D);
        }

        [Test]
        public void NestedFunctions_2()
        {
            Expression.Create("pow(.301, -sqrt(10))").Value.Should().Equal(44.55716209442361D);
        }

        [Test]
        public void NestedFunctions_3()
        {
            Expression.Create("pow(10, 3) * pow(10, sqrt(pow(.301, -sqrt(10))))").Value.Should().Equal(4732767141.9821711D);
        }

        [Test]
        public void FunctionsWithNestedExpr()
        {
            Expression.Create("pow((10 * 3), -(1 + 3))").Value.Should().Equal(0.0000012345679012345679D);
        }

        [Test]
        public void FunctionsWithNestedExpr_2()
        {
            Expression.Create("pow((10 * 3), -(1 + sqrt(10 * 10)))").Value.Should().Equal(0.00000000000000005645029269476762D);
        }

        [Test]
        public void NegativePositiveDecimalBracket()
        {
            Expression.Create(".2 / +9.99 * (-.123 + -2)").Value.Should().Equal(-0.042502502502502509D);
        }

        [Test]
        public void NegativePositiveDecimalBracket_2()
        {
            Expression.Create("-.23 + +.99 * ((-.123 + -2.1)--333)").Value.Should().Equal(327.23922999999996D);
        }


        [Test]
        [ExpectedException(typeof(ExpressionException), ExpectedMessage = "Unexpected end of input, but found token(s): 'NUMBER', '+', '-', '(', 'IDENT'.")]
        public void IncompleteExpr_ExpectedExpr()
        {
            var lost = Expression.Create("3 + 1 - (").Value;
        }

        [Test]
        [ExpectedException(typeof(ExpressionException), ExpectedMessage = "Expected expression.")]
        public void IncompleteExpr_ExpectedExpr_2()
        {
            var lost = Expression.Create("1 2").Value;
        }

        [Test]
        [ExpectedException(typeof(ExpressionException), ExpectedMessage = "Syntax error, odd number of brackets.")]
        public void IncompleteExpr_MissingParen()
        {
            var lost = Expression.Create("3 + (1 -").Value;
        }

        [Test]
        [ExpectedException(typeof(ExpressionException), ExpectedMessage = "Syntax error, odd number of brackets.")]
        public void IncompleteExpr_MissingParen_2()
        {
            var lost = Expression.Create("3 + 3 / (1").Value;
        }
    }
}