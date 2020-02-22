sealed class IdentifierToken : Token
{
    public IdentifierToken(string text)
        : base(text, TokenType.Identifier)
    {
    }
}