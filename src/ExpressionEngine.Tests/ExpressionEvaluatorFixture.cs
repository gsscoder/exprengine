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
            new ExpressionEvaluator().Evaluate("1 + 2").Should().Equal(3D);
        }

        [Test]
        public void PlainAddition_2()
        {
            new ExpressionEvaluator().Evaluate("1 + 2 + 3").Should().Equal(6D);
        }

        [Test]
        public void PlainAddition_3()
        {
            new ExpressionEvaluator().Evaluate("1 + 2 + 3 + 4").Should().Equal(10D);
        }

        [Test]
        public void PlainSubtraction()
        {
            new ExpressionEvaluator().Evaluate("3 - 4").Should().Equal(-1D);
        }

        [Test]
        public void PlainMultiplication()
        {
            new ExpressionEvaluator().Evaluate("3 * 9").Should().Equal(27D);
        }

        [Test]
        public void PlainDivision()
        {
            new ExpressionEvaluator().Evaluate("16 / 4").Should().Equal(4D);
        }

        [Test]
        public void EvaluateSmallExpr()
        {
            new ExpressionEvaluator().Evaluate("1 + 2 * 3 / 1").Should().Equal(7D);
        }

        [Test]
        public void EvaluateSmallExprWithDecimal()
        {
            new ExpressionEvaluator().Evaluate(".90 - 3 * 4 + 11.123").Should().Equal(0.022999999999999687D);
        }

        [Test]
        public void EvaluateSmallExprWithSignes()
        {
            new ExpressionEvaluator().Evaluate("-3 + -1").Should().Equal(-4D);
        }

        [Test]
        public void EvaluateExpr()
        {
            new ExpressionEvaluator().Evaluate("3 * 0.31 / 19 + 10 - .7").Should().Equal(9.3489473684210527D);
        }

        [Test]
        public void EvaluateExprWithBrackets()
        {
            new ExpressionEvaluator().Evaluate("1 + (2 - 1)").Should().Equal(2D);
        }

        [Test]
        public void EvaluateExprWithBrackets_2()
        {
            new ExpressionEvaluator().Evaluate("3 * 0.31 / ((19 + 10) - .7)").Should().Equal(0.032862190812720848D);
        }

        [Test]
        public void UnaryBrackets()
        {
            new ExpressionEvaluator().Evaluate("-(1 + 2)").Should().Equal(-3D);
        }

        [Test]
        public void NegativePositiveDecimalBracket()
        {
            new ExpressionEvaluator().Evaluate(".2 / +9.99 * (-.123 + -2)").Should().Equal(-0.042502502502502509D);
        }

        [Test]
        public void NegativePositiveDecimalBracket_2()
        {
            new ExpressionEvaluator().Evaluate("-.23 + +.99 * ((-.123 + -2.1)--333)").Should().Equal(327.23922999999996D);
        }


        [Test]
        [ExpectedException(typeof(EvaluatorException), ExpectedMessage = "Expected unary operator, literal or open bracket.")]
        public void IncompleteExpr_ExpectedExpr()
        {
            new ExpressionEvaluator().Evaluate("3 + 1 - (");
        }

        [Test]
        [ExpectedException(typeof(EvaluatorException), ExpectedMessage = "Expected expression.")]
        public void IncompleteExpr_ExpectedExpr_2()
        {
            new ExpressionEvaluator().Evaluate("1 2");
        }

        //[Test]
        //[ExpectedException(typeof(EvaluatorException), ExpectedMessage = "Expected expression.")]
        //public void IncompleteExpr_MissingParen()
        //{
        //    new ExpressionEvaluator().Evaluate("3 + (1 -");
        //}

        //[Test]
        //[ExpectedException(typeof(EvaluatorException), ExpectedMessage = "Expected expression.")]
        //public void IncompleteExpr_MissingParen_2()
        //{
        //    new ExpressionEvaluator().Evaluate("3 + 3 / (1");
        //}
    }
}