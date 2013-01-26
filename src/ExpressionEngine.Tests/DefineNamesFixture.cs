#region License
//
// Expression Engine Library: DefineNamesFixture.cs
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
using NUnit.Framework;
using FluentAssertions;
#endregion

namespace ExpressionEngine.Tests
{
    [TestFixture]
    public class DefineNamesFixture
    {
        //[Test]
        //public void ExpressionWithoutUserDefinedNamesIsMutable()
        //{
        //    var expr = Expression.Create("1 + (fun(y) / 4) * (x - 1)");
        //    expr.Should().Not.Be.OfType<Expression>();
        //}

        //[Test]
        //public void UserDefinedNames()
        //{
        //    var vars = new Dictionary<string, object>
        //        {
        //            {"G", 6.67428D},
        //            {"earth_mass", Expression.Create("5.97219 * 10^24").Value}, // 5.97219E+24 kg
        //            {"lunar_mass", Expression.Create("7.34767309 * 10^22").Value}, // 7.34767309E+22 kg
        //            {"perigee_dist", 356700000D} // moon-earth distance at perigee in m
        //        };
        //    var funcs = new Dictionary<string, Func<object[], object>>
        //        {
        //            {"calc_force", (object[] args) => ((double) args[0] * (double) args[1]) / Math.Pow((double) args[2], 2)}
        //        };
        //    Expression.Create("G * calc_force(earth_mass, lunar_mass, perigee_dist)", vars, funcs).Value.Should().Be(2.3018745174107073E+31D);
        //}
        [Test]
        public void UserDefinedNames()
        {
            new Context()
                .SetVariable("G", 6.67428D)
                .SetVariable("earth_mass", Evaluator.Evaluate<double>("5.97219 * pow(10,24)")) // 5.97219E+24 kg
                .SetVariable("lunar_mass", Evaluator.Evaluate<double>("7.34767309 * pow(10,22)")) // 7.34767309E+22 kg
                .SetVariable("perigee_dist", 356700000D) // moon-earth distance at perigee in m
                .SetFunction("calc_force", args => (
                    TypeConverter.ToNumber(args[0]) * TypeConverter.ToNumber(args[1]) / Math.Pow(TypeConverter.ToNumber(args[2]), 2)))
                .EvaluateAs<double>("G * calc_force(earth_mass, lunar_mass, perigee_dist)").Should().Be(2.3018745174107073E+31D);
        }

        //[Test]
        //[ExpectedException(typeof(EeException), ExpectedMessage = "Immutable expressions are implicitly thread safe.")]
        //public void ImmutableExpressionCantBeSynchronized()
        //{
        //    // Following expression is immutable because it lacks user defined functions
        //    // or variables; pi is a built-in
        //    Expression.Synchronized(Expression.Create("1 + 2 * (3 / 4) - pi"));
        //}

        //[Test]
        //public void MutableExpressionCanBeSynchronized()
        //{
        //    // Following expression is mutable because it has a user defined variable,
        //    // named 'c'
        //    var mutable = Expression.Create("1 + 2 * (3 / 4) - c");
        //    mutable.Should().Not.Be.OfType<Expression>();
        //    var synchronized = Expression.Synchronized(mutable);
        //    synchronized.DefineVariable("c", 299792458D);
        //    synchronized.Value.Should().Be(-299792455.5D);
        //}
    }
}