using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FluentAssertions;

namespace ExpressionEngine.Tests
{
    [TestFixture]
    public sealed class RelationalFixture
    {
        [Test]
        public void Equality()
        {
            new Context().EvaluateAs<bool>("10 == 10").Should().Be(true);
        }

        [Test]
        public void Inequality()
        {
            new Context().EvaluateAs<bool>("10 != (5 + 5)").Should().Be(false);
        }

        [Test]
        public void LesserThan()
        {
            new Context().EvaluateAs<bool>("pow(10, 2) < 9.9").Should().Be(false);
        }

        [Test]
        public void GreaterThan()
        {
            new Context().EvaluateAs<bool>("pow(10, 2) > 9.9").Should().Be(true);
        }

        [Test]
        public void LesserThanOrEqual()
        {
            new Context().EvaluateAs<bool>("pi <= pi").Should().Be(true);
        }

        [Test]
        public void GreaterThanOrEqual()
        {
            new Context().EvaluateAs<bool>("pi >= (pi + 0.0001)").Should().Be(false);
        }

        [Test]
        public void Concat()
        {
            new Context().EvaluateAs<bool>("(10 == 1) == (1 == 10)").Should().Be(true);
        }

        [Test]
        public void Concat_2()
        {
            new Context().EvaluateAs<bool>("(10 == 1) != (10 == 1)").Should().Be(false);
        }

        [Test]
        public void True()
        {
            new Context().EvaluateAs<bool>("(10 == 10) == true").Should().Be(true);
        }

        [Test]
        public void True_2()
        {
            new Context().EvaluateAs<bool>("true == true").Should().Be(true);
        }

        [Test]
        public void False()
        {
            new Context().EvaluateAs<bool>("false != (10 != 10)").Should().Be(false);
        }

        [Test]
        public void False_2()
        {
            new Context().EvaluateAs<bool>("false == false").Should().Be(true);
        }

        [Test]
        public void SumOfNumberAndBoolean()
        {
            new Context().EvaluateAs<double>("true + 1").Should().Be(2D);
        }

        [Test]
        public void SumOfNumberAndBoolean_2()
        {
            new Context().EvaluateAs<double>("false - 1").Should().Be(-1D);
        }

        [Test]
        public void SumOfExprWithBoolean()
        {
            new Context().EvaluateAs<double>("true - (1 + 2)").Should().Be(-2D);
        }

        [Test]
        public void SumOfExprWithBoolean_2()
        {
            new Context().EvaluateAs<double>("(10 == 10) + false").Should().Be(1D);
        }

        [Test]
        public void Function_Boolean()
        {
            new Context().EvaluateAs<double>("pow(10, true+1)").Should().Be(100D);
        }

        [Test]
        public void Function_Boolean_2()
        {
            new Context().EvaluateAs<double>("log(10, 10+false)").Should().Be(1D);
        }
    }
}
