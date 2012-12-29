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
        public void EvaluateSmallExpr()
        {
            Expression.Create("1 + 2 * 3 / 1").Value.Should().Equal(7D);
        }

        [Test]
        public void EvaluateSmallExprWithDecimal()
        {
            Expression.Create(".90 - 3 * 4 + 11.123").Value.Should().Equal(0.022999999999999687D);
        }

        [Test]
        public void EvaluateSmallExprWithSignes()
        {
            Expression.Create("-3 + -1").Value.Should().Equal(-4D);
        }

        [Test]
        public void EvaluateExpr()
        {
            Expression.Create("3 * 0.31 / 19 + 10 - .7").Value.Should().Equal(9.3489473684210527D);
        }

        [Test]
        public void EvaluateExprWithBrackets()
        {
            Expression.Create("1 + (2 - 1)").Value.Should().Equal(2D);
        }

        [Test]
        public void EvaluateExprWithBrackets_2()
        {
            Expression.Create("3 * 0.31 / ((19 + 10) - .7)").Value.Should().Equal(0.032862190812720848D);
        }

        [Test]
        public void UnaryBrackets()
        {
            Expression.Create("-(1 + 2)").Value.Should().Equal(-3D);
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
        [ExpectedException(typeof(ExpressionException), ExpectedMessage = "Unexpected end of input, but found token(s): 'NUMBER', '+', '-', '('.")]
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