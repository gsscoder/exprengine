using Should.Fluent;

namespace ExpressionEngine.Tests
{
    static class TokenUtil
    {
        public static void ShouldPunctuatorEqual(this Token token, TokenType type, string text)
        {
            token.Type.Should().Equal(type);
            token.Text.Should().Equal(text);
        }

        public static void ShouldLiteralEqual(this Token token, string text)
        {
            token.Type.Should().Equal(TokenType.Literal);
            token.Text.Should().Equal(text);
        }
    }
}
