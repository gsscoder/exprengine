using System;
using UnitTest = NUnit.Framework;
using Should.Fluent;
using ExpressionEngine.Internal;

namespace ExpressionEngine.Tests
{
    [UnitTest.TestFixture]
    public sealed class CacheFixture
    {
        [UnitTest.SetUp]
        public void CreateInstance()
        {
            _cache = new ValueCache<double>();
        }

        [UnitTest.TearDown]
        public void DestroyInstance()
        {
            _cache = null;
        }

        [UnitTest.Test]
        [UnitTest.ExpectedException(typeof(ArgumentException))]
        public void UpdateNonExistentItemThrowsException()
        {
            _cache["(1 + 2)"] = 3D;
        }

        [UnitTest.Test]
        [UnitTest.ExpectedException(typeof(ArgumentNullException))]
        public void UpdateUsingNullKeyThrowsException()
        {
            _cache[null] = .333D;
        }

        [UnitTest.Test]
        [UnitTest.ExpectedException(typeof(ArgumentException))]
        public void ReadNonExistentItemThrowsException()
        {
            var value = _cache["(1 + 2)"];
        }

        [UnitTest.Test]
        [UnitTest.ExpectedException(typeof(ArgumentNullException))]
        public void ReadUsingNullKeyThrowsException()
        {
            var value = _cache[null];
        }

        [UnitTest.Test]
        public void AddNewItem()
        {
            _cache.Contains("1+2+3").Should().Be.False();
            _cache.Add("1+2+3", 6D);
            _cache.Contains("1+2+3").Should().Be.True();
        }

        [UnitTest.Test]
        public void UpdateExistingItem()
        {
            _cache.Contains("1+2+3+myfunc(x)").Should().Be.False();
            _cache.Add("1+2+3+myfunc(x)", 12.999D);
            _cache.Contains("1+2+3+myfunc(x)").Should().Be.True();
            // myfunc(x) changes...
            _cache["1+2+3+myfunc(x)"] = 19.122D;
            _cache["1+2+3+myfunc(x)"].Should().Equal(19.122D);
        }

        private ValueCache<double> _cache;
    }
}
