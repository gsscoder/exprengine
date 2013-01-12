using System.Collections.Generic;
using ExpressionEngine.Internal.Model;

namespace ExpressionEngine.Internal
{
    sealed class PunctuatorToken : Token
    {
        private PunctuatorToken(string text)
            : base(text)
        {
        }

        public static Token ValueOf(string text)
        {
            PunctuatorToken token;
            return Lookup.TryGetValue(text, out token) ? token : null;
        }

        public OperatorType ToOperatorType()
        {
            if (this == PunctuatorToken.Plus)
            {
                return OperatorType.Add;
            }
            if (this == PunctuatorToken.Minus)
            {
                return OperatorType.Subtract;
            }
            if (this == PunctuatorToken.Multiply)
            {
                return OperatorType.Multiply;
            }
            if (this == PunctuatorToken.Divide)
            {
                return OperatorType.Divide;
            }
            if (this == PunctuatorToken.Modulo)
            {
                return OperatorType.Modulo;
            }
            throw new ExpressionException("Unexpected punctuator.");
        }

        public static readonly PunctuatorToken LeftParenthesis = new PunctuatorToken("(");
        public static readonly PunctuatorToken RightParenthesis = new PunctuatorToken(")");
        public static readonly PunctuatorToken Comma = new PunctuatorToken(",");
        public static readonly PunctuatorToken Plus = new PunctuatorToken("+");
        public static readonly PunctuatorToken Minus = new PunctuatorToken("-");
        public static readonly PunctuatorToken Multiply = new PunctuatorToken("*");
        public static readonly PunctuatorToken Divide = new PunctuatorToken("/");
        public static readonly PunctuatorToken Modulo = new PunctuatorToken("%");
        public static readonly PunctuatorToken Exponent = new PunctuatorToken("^");

        private static readonly Dictionary<string, PunctuatorToken> Lookup = new Dictionary<string, PunctuatorToken>
            {
                {"(", LeftParenthesis},
                {")", RightParenthesis},
                {",", Comma},
                {"+", Plus},
                {"-", Minus},
                {"*", Multiply},
                {"/", Divide},
                {"%", Modulo},
                {"^", Exponent}
            };
    }
}