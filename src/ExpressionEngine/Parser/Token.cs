using System;
using System.Text;

abstract class Token
{
    protected Token() {}

    protected Token(string text, TokenType type)
    {
        _text = text;
        _type = type;
    }

    //public static string StringOf(Token token)
    //{
    //    return token == null ? "end of input" : string.Format("'{0}'", token.Tokenizer);
    //}

    public static string StringOf(Token[] tokens)
    {
        var builder = new StringBuilder(4 * tokens.Length);
        foreach (var token in tokens)
        {
            builder.Append("'");
            builder.Append(StringOf(token));
            builder.Append("', ");
        }
        return builder.ToString(0, builder.Length - 2);
    }

    public static string StringOf(TokenType[] types)
    {
        var builder = new StringBuilder(4 * types.Length);
        foreach (var type in types)
        {
            builder.Append("'");
            builder.Append(StringOf(type));
            builder.Append("', ");
        }
        return builder.ToString(0, builder.Length - 2);
    }

    public static string StringOf(Token token)
    {
        if (token == null)
        {
            return "end of input";
        }
        if (token.Type == TokenType.Literal || token.Type == TokenType.Identifier)
        {
            return token.Text;
        }
        return StringOf(token.Type);
    }

    public static string StringOf(TokenType type)
    {
        switch (type)
        {
            case TokenType.LeftParenthesis:
                return "(";
            case TokenType.RightParenthesis:
                return ")";
            case TokenType.Comma:
                return ",";
            case TokenType.Plus:
                return "+";
            case TokenType.Minus:
                return "-";
            case TokenType.Multiply:
                return "*";
            case TokenType.Divide:
                return "/";
            case TokenType.Modulo:
                return "%";
            case TokenType.Equality:
                return "==";
            case TokenType.Inequality:
                return "!=";
            case TokenType.LessThan:
                return "<";
            case TokenType.GreaterThan:
                return ">";
            case TokenType.LessThanOrEqual:
                return "<=";
            case TokenType.GreaterThanOrEqual:
                return ">=";
            case TokenType.Literal:
                return "LITERAL";
            case TokenType.Identifier:
                return "IDENT";
            default:
                throw new InvalidOperationException();
        }
    }

    public virtual string Text
    {
        get { return _text; }
    }

    public TokenType Type
    {
        get { return _type; }
    }

    public override string ToString()
    {
        return "{0} [{1}]".FormatInvariant(Text, GetType().Name);
    }

    private readonly string _text;
    private readonly TokenType _type;
}