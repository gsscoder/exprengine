using System;
using UnitTest = NUnit.Framework;
using FluentAssertions;
using ExpressionEngine.Internal;

namespace ExpressionEngine.Tests
{
    [UnitTest.TestFixture]
    public sealed class TextFixture
    {
        [UnitTest.Test]
        public void AfterInitializationIndexIsLessThanZero()
        {
            var text = Text.OfString("simple text");
            text.Column.Should().Be(-1);
            text.Line.Should().Be(-1);
            text.PeekChar();
            text.Column.Should().Be(-1);
            text.Line.Should().Be(-1);
            text.NextChar().Should().Be('s');
            text.Column.Should().Be(0);
            text.Line.Should().Be(0);
        }

        [UnitTest.Test]
        public void ReadingPastEndGetsNullChar()
        {
            var text = Text.OfString("simple text!");
            char last = text.PeekChar();
            for (int i = 0; i < 12; i++)
            {
                last = text.NextChar();
            }
            text.Column.Should().Be(11);
            text.Line.Should().Be(0);
            last.Should().Be('!');
            text.PeekChar().Should().Be('\0');
            text.NextChar().Should().Be('\0');
        }

        [UnitTest.Test]
        public void LineEndIncrementsLines()
        {
            var text = Text.OfString("simple\ntext");
            text.NextChar();
            (text.Line == 0).Should().BeTrue();
            while (text.PeekChar() != '\0')
            {
                text.NextChar();
            }
            (text.Line == 1).Should().BeTrue();
        }
    }
}