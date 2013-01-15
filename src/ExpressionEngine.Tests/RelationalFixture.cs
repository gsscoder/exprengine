using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Should.Fluent;

namespace ExpressionEngine.Tests
{
    [TestFixture]
    public sealed class RelationalFixture
    {
        [Test]
        public void Equality()
        {
            new ExpressionEvaluator().Evaluate("10 == 10").Should().Equal(true);
        }

        [Test]
        public void Inequality()
        {
            new ExpressionEvaluator().Evaluate("10 != (5 + 5)").Should().Equal(false);
        }

        [Test]
        public void LesserThan()
        {
            new ExpressionEvaluator().Evaluate("pow(10, 2) < 9.9").Should().Equal(false);
        }

        [Test]
        public void GreaterThan()
        {
            new ExpressionEvaluator().Evaluate("pow(10, 2) > 9.9").Should().Equal(true);
        }

        [Test]
        public void LesserThanOrEqual()
        {
            new ExpressionEvaluator().Evaluate("pi <= pi").Should().Equal(true);
        }

        [Test]
        public void GreaterThanOrEqual()
        {
            new ExpressionEvaluator().Evaluate("pi >= (pi + 0.0001)").Should().Equal(false);
        }

        [Test]
        public void Concat()
        {
            new ExpressionEvaluator().Evaluate("(10 == 1) == (1 == 10)").Should().Equal(true);
        }

        [Test]
        public void Concat_2()
        {
            new ExpressionEvaluator().Evaluate("(10 == 1) != (10 == 1)").Should().Equal(false);
        }

        [Test]
        public void True()
        {
            new ExpressionEvaluator().Evaluate("(10 == 10) == true").Should().Equal(true);
        }

        [Test]
        public void True_2()
        {
            new ExpressionEvaluator().Evaluate("true == true").Should().Equal(true);
        }

        [Test]
        public void False()
        {
            new ExpressionEvaluator().Evaluate("false != (10 != 10)").Should().Equal(false);
        }

        [Test]
        public void False_2()
        {
            new ExpressionEvaluator().Evaluate("false == false").Should().Equal(true);
        }

        [Test]
        public void SumOfNumberAndBoolean()
        {
            new ExpressionEvaluator().Evaluate("true + 1").Should().Equal(2D);
        }

        [Test]
        public void SumOfNumberAndBoolean_2()
        {
            new ExpressionEvaluator().Evaluate("false - 1").Should().Equal(-1D);
        }

        [Test]
        public void SumOfExprWithBoolean()
        {
            new ExpressionEvaluator().Evaluate("true - (1 + 2)").Should().Equal(-2D);
        }

        [Test]
        public void SumOfExprWithBoolean_2()
        {
            new ExpressionEvaluator().Evaluate("(10 == 10) + false").Should().Equal(1D);
        }

        [Test]
        public void Function_Boolean()
        {
            new ExpressionEvaluator().Evaluate("pow(10, true+1)").Should().Equal(100D);
        }

        [Test]
        public void Function_Boolean_2()
        {
            new ExpressionEvaluator().Evaluate("log(10, 10+false)").Should().Equal(1D);
        }
    }
}
