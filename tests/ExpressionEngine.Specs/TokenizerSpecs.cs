using Xunit;
using FluentAssertions;

public class TokenizerSpecs
{
    [Fact]
    public void After_initialization_index_is_less_than_zero()
    {
        // Given when
        var text = Tokenizer.OfString("simple text");

        // Then
        text.Column.Should().Be(-1);
        text.Line.Should().Be(-1);
    }

    [Fact]
    public void Reading_past_end_gets_null_char()
    {
        // Given
        var text = Tokenizer.OfString("simple text!");

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
        var text = Tokenizer.OfString("simple\ntext");

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