using System;
using System.Collections.Generic;
using NUnit.Framework;
using Should.Fluent;

namespace ExpressionEngine.Tests
{
    [TestFixture]
    public class MutableExpressionFixture
    {
        [Test]
        public void ExpressionWithoutUserDefinedNamesIsMutable()
        {
            Expression.Create("1 + (fun(y) / 4) * (x - 1)").Should().Not.Be.OfType<Expression>();
        }

        [Test]
        public void UserDefinedNames()
        {
            var vars = new Dictionary<string, double>
                {
                    {"G", 6.67428},
                    {"earth_mass", Expression.Create("5.97219 * 10^24").Value}, //5.97219E+24 kg
                    {"lunar_mass", Expression.Create("7.34767309 * 10^22").Value}, //7.34767309E+22 kg
                    {"perigee_dist", 356700000} // moon-earth distance at perigee in m
                };
            var funcs = new Dictionary<string, Func<double[], double>>
                {
                    {"calc_force", (double[] args) => (args[0] * args[1]) / Math.Pow(args[2], 2)}
                };
            Expression.Create("G * calc_force(earth_mass, lunar_mass, perigee_dist)", vars, funcs).Value.Should().Equal(2.3018745174107073E+31);
        }
    }
}