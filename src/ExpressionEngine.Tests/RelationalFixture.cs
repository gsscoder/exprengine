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
            new ExpressionEvaluator().EvaluateAs<bool>("10 == 10").Should().Equal(true);
        }

        [Test]
        public void Inequality()
        {
            new ExpressionEvaluator().EvaluateAs<bool>("10 != (5 + 5)").Should().Equal(false);
        }

        [Test]
        public void LesserThan()
        {
            new ExpressionEvaluator().EvaluateAs<bool>("pow(10, 2) < 9.9").Should().Equal(false);
        }

        [Test]
        public void GreaterThan()
        {
            new ExpressionEvaluator().EvaluateAs<bool>("pow(10, 2) > 9.9").Should().Equal(true);
        }

        [Test]
        public void LesserThanOrEqual()
        {
            new ExpressionEvaluator().EvaluateAs<bool>("pi <= pi").Should().Equal(true);
        }

        [Test]
        public void GreaterThanOrEqual()
        {
            new ExpressionEvaluator().EvaluateAs<bool>("pi >= (pi + 0.0001)").Should().Equal(false);
        }

        [Test]
        public void Concat()
        {
            new ExpressionEvaluator().EvaluateAs<bool>("(10 == 1) == (1 == 10)").Should().Equal(true);
        }

        [Test]
        public void Concat_2()
        {
            new ExpressionEvaluator().EvaluateAs<bool>("(10 == 1) != (10 == 1)").Should().Equal(false);
        }

        [Test]
        public void True()
        {
            new ExpressionEvaluator().EvaluateAs<bool>("(10 == 10) == true").Should().Equal(true);
        }

        [Test]
        public void True_2()
        {
            new ExpressionEvaluator().EvaluateAs<bool>("true == true").Should().Equal(true);
        }

        [Test]
        public void False()
        {
            new ExpressionEvaluator().EvaluateAs<bool>("false != (10 != 10)").Should().Equal(false);
        }

        [Test]
        public void False_2()
        {
            new ExpressionEvaluator().EvaluateAs<bool>("false == false").Should().Equal(true);
        }

        [Test]
        public void SumOfNumberAndBoolean()
        {
            new ExpressionEvaluator().EvaluateAs<double>("true + 1").Should().Equal(2D);
        }

        [Test]
        public void SumOfNumberAndBoolean_2()
        {
            new ExpressionEvaluator().EvaluateAs<double>("false - 1").Should().Equal(-1D);
        }

        [Test]
        public void SumOfExprWithBoolean()
        {
            new ExpressionEvaluator().EvaluateAs<double>("true - (1 + 2)").Should().Equal(-2D);
        }

        [Test]
        public void SumOfExprWithBoolean_2()
        {
            new ExpressionEvaluator().EvaluateAs<double>("(10 == 10) + false").Should().Equal(1D);
        }

        [Test]
        public void Function_Boolean()
        {
            new ExpressionEvaluator().EvaluateAs<double>("pow(10, true+1)").Should().Equal(100D);
        }

        [Test]
        public void Function_Boolean_2()
        {
            new ExpressionEvaluator().EvaluateAs<double>("log(10, 10+false)").Should().Equal(1D);
        }
    }
}
