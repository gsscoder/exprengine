#region License
//
// Expression Engine Library: ComparisonFixture.cs
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
using NUnit.Framework;
using Should.Fluent;
#endregion

namespace ExpressionEngine.Tests
{
    [TestFixture]
    public sealed class ComparisonFixture
    {
        [Test]
        public void SpacesMakeNoDifference()
        {
            var expr1 = Expression.Create(" ( 1 + 2 ) ^ 3 ");
            var expr2 = Expression.Create("(1+2)^3");
            expr1.Equals(expr2).Should().Be.True();
        }

        [Test]
        public void SpacesMakeNoDifferenceEvenForHashCode()
        {
            var expr1 = Expression.Create("sqrt(1000) - pi");
            var expr2 = Expression.Create("sqrt( 1000 )  -  pi");
            (expr1.GetHashCode() == expr2.GetHashCode()).Should().Be.True();
        }

        [Test]
        public void SpacesMakeNoDifferenceEvenWithEqualityOperator()
        {
            var expr1 = Expression.Create(" ( 1 + 2 ) * e ");
            var expr2 = Expression.Create("(  1  +  2  )  *  e");
            expr1.Equals(expr2).Should().Be.True();
            (expr1 == expr2).Should().Be.True();
        }
    }
}
