using System;
using UnitTest = NUnit.Framework;
using Should.Fluent;
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
            text.Column.Should().Equal(-1);
            text.Line.Should().Equal(-1);
            text.PeekChar();
            text.Column.Should().Equal(-1);
            text.Line.Should().Equal(-1);
            text.NextChar().Should().Equal('s');
            text.Column.Should().Equal(0);
            text.Line.Should().Equal(0);
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
            text.Column.Should().Equal(11);
            text.Line.Should().Equal(0);
            last.Should().Equal('!');
            text.PeekChar().Should().Equal('\0');
            text.NextChar().Should().Equal('\0');
        }

        [UnitTest.Test]
        public void LineEndIncrementsLines()
        {
            var text = Text.OfString("simple\ntext");
            text.NextChar();
            (text.Line == 0).Should().Be.True();
            while (text.PeekChar() != '\0')
            {
                text.NextChar();
            }
            (text.Line == 1).Should().Be.True();
        }
    }
}