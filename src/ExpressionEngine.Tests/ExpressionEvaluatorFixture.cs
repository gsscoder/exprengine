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
            new MathExpression("1 + 2").Evaluate().Should().Equal(3D);
        }

        [Test]
        public void PlainAddition_2()
        {
            new MathExpression("1 + 2 + 3").Evaluate().Should().Equal(6D);
        }

        [Test]
        public void PlainAddition_3()
        {
            new MathExpression("1 + 2 + 3 + 4").Evaluate().Should().Equal(10D);
        }

        [Test]
        public void PlainSubtraction()
        {
            new MathExpression("3 - 4").Evaluate().Should().Equal(-1D);
        }

        [Test]
        public void PlainMultiplication()
        {
            new MathExpression("3 * 9").Evaluate().Should().Equal(27D);
        }

        [Test]
        public void PlainDivision()
        {
            new MathExpression("16 / 4").Evaluate().Should().Equal(4D);
        }

        [Test]
        public void EvaluateSmallExpr()
        {
            new MathExpression("1 + 2 * 3 / 1").Evaluate().Should().Equal(7D);
        }

        [Test]
        public void EvaluateSmallExprWithDecimal()
        {
            new MathExpression(".90 - 3 * 4 + 11.123").Evaluate().Should().Equal(0.022999999999999687D);
        }

        [Test]
        public void EvaluateSmallExprWithSignes()
        {
            new MathExpression("-3 + -1").Evaluate().Should().Equal(-4D);
        }

        [Test]
        public void EvaluateExpr()
        {
            new MathExpression("3 * 0.31 / 19 + 10 - .7").Evaluate().Should().Equal(9.3489473684210527D);
        }

        [Test]
        public void EvaluateExprWithBrackets()
        {
            new MathExpression("1 + (2 - 1)").Evaluate().Should().Equal(2D);
        }

        [Test]
        public void EvaluateExprWithBrackets_2()
        {
            new MathExpression("3 * 0.31 / ((19 + 10) - .7)").Evaluate().Should().Equal(0.032862190812720848D);
        }

        [Test]
        public void UnaryBrackets()
        {
            new MathExpression("-(1 + 2)").Evaluate().Should().Equal(-3D);
        }

        [Test]
        public void NegativePositiveDecimalBracket()
        {
            new MathExpression(".2 / +9.99 * (-.123 + -2)").Evaluate().Should().Equal(-0.042502502502502509D);
        }

        [Test]
        public void NegativePositiveDecimalBracket_2()
        {
            new MathExpression("-.23 + +.99 * ((-.123 + -2.1)--333)").Evaluate().Should().Equal(327.23922999999996D);
        }


        [Test]
        [ExpectedException(typeof(EvaluatorException), ExpectedMessage = "Unexpected end of input, but found token(s): 'NUMBER', '+', '-', '('.")]
        public void IncompleteExpr_ExpectedExpr()
        {
            new MathExpression("3 + 1 - (").Evaluate();
        }

        [Test]
        [ExpectedException(typeof(EvaluatorException), ExpectedMessage = "Expected expression.")]
        public void IncompleteExpr_ExpectedExpr_2()
        {
            new MathExpression("1 2").Evaluate();
        }

        [Test]
        [ExpectedException(typeof(EvaluatorException), ExpectedMessage = "Syntax error, odd number of brackets.")]
        public void IncompleteExpr_MissingParen()
        {
            new MathExpression("3 + (1 -").Evaluate();
        }

        [Test]
        [ExpectedException(typeof(EvaluatorException), ExpectedMessage = "Syntax error, odd number of brackets.")]
        public void IncompleteExpr_MissingParen_2()
        {
            new MathExpression("3 + 3 / (1").Evaluate();
        }
    }
}