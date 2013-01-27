#region License
//
// Expression Engine Library: TextFixture.cs
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
using ExpressionEngine.Internal;
using Xunit;
using FluentAssertions;
#endregion

namespace ExpressionEngine.Tests
{
    public class TextFixture
    {
        [Fact]
        public void After_initialization_index_is_less_than_zero()
        {
            // Given when
            var text = Text.OfString("simple text");

            // Then
            text.Column.Should().Be(-1);
            text.Line.Should().Be(-1);
        }

        [Fact]
        public void Reading_past_end_gets_null_char()
        {
            // Given
            var text = Text.OfString("simple text!");

            // When
            for (var i = 0; i < 12; i++)
            {
                text.NextChar();
            }

            // Than
            text.PeekChar().Should().Be('\0');
            text.NextChar().Should().Be('\0');
        }

        [Fact]
        public void Line_end_increments_lines()
        {
            // Given
            var text = Text.OfString("simple\ntext");

            // When
            text.NextChar();
            while (text.PeekChar() != '\0')
            {
                text.NextChar();
            }

            // Than
            (text.Line == 1).Should().BeTrue();
        }
    }
}