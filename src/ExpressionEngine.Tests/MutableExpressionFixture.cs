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
            var expr = Expression.Create("1 + (fun(y) / 4) * (x - 1)");
            expr.Should().Not.Be.OfType<Expression>();
        }

        [Test]
        public void UserDefinedNames()
        {
            var vars = new Dictionary<string, object>
                {
                    {"G", 6.67428},
                    {"earth_mass", Expression.Create("5.97219 * 10^24").Value}, // 5.97219E+24 kg
                    {"lunar_mass", Expression.Create("7.34767309 * 10^22").Value}, // 7.34767309E+22 kg
                    {"perigee_dist", 356700000D} // moon-earth distance at perigee in m
                };
            var funcs = new Dictionary<string, Func<object[], object>>
                {
                    {"calc_force", (object[] args) => ((double) args[0] * (double) args[1]) / Math.Pow((double) args[2], 2)}
                };
            Expression.Create("G * calc_force(earth_mass, lunar_mass, perigee_dist)", vars, funcs).Value.Should().Equal(2.3018745174107073E+31D);
        }

        [Test]
        [ExpectedException(typeof(ExpressionException), ExpectedMessage = "Immutable expressions are implicitly thread safe.")]
        public void ImmutableExpressionCantBeSynchronized()
        {
            // Following expression is immutable because it lacks user defined functions
            // or variables; pi is a built-in
            Expression.Synchronized(Expression.Create("1 + 2 * (3 / 4) - pi"));
        }

        [Test]
        public void MutableExpressionCanBeSynchronized()
        {
            // Following expression is mutable because it has a user defined variable,
            // named 'c'
            var mutable = Expression.Create("1 + 2 * (3 / 4) - c");
            mutable.Should().Not.Be.OfType<Expression>();
            var synchronized = Expression.Synchronized(mutable);
            synchronized.DefineVariable("c", 299792458D);
            synchronized.Value.Should().Equal(-299792455.5D);
        }
    }
}