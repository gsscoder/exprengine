using System;
using System.Collections.Generic;
using ExpressionEngine.Internal.Model;

namespace ExpressionEngine.Internal
{
    sealed class PunctuatorToken : Token
    {
        public PunctuatorToken(TokenType type)
            : base(null, type)
        {
        }

        //public static Token ValueOf(string text)
        //{
        //    PunctuatorToken token;
        //    return Lookup.TryGetValue(text, out token) ? token : null;
        //}

        public OperatorType ToOperatorType()
        {
            if (Type == TokenType.Plus)
            {
                return OperatorType.Add;
            }
            if (Type == TokenType.Minus)
            {
                return OperatorType.Subtract;
            }
            if (Type == TokenType.Multiply)
            {
                return OperatorType.Multiply;
            }
            if (Type == TokenType.Divide)
            {
                return OperatorType.Divide;
            }
            if (Type == TokenType.Modulo)
            {
                return OperatorType.Modulo;
            }
            throw new EvaluatorException("Unexpected punctuator.");
        }

        public override string Text
        {
            get {
                return StringOf(Type);
            }
        }

        //public static readonly PunctuatorToken LeftParenthesis = new PunctuatorToken("(");
        //public static readonly PunctuatorToken RightParenthesis = new PunctuatorToken(")");
        //public static readonly PunctuatorToken Comma = new PunctuatorToken(",");
        //public static readonly PunctuatorToken Plus = new PunctuatorToken("+");
        //public static readonly PunctuatorToken Minus = new PunctuatorToken("-");
        //public static readonly PunctuatorToken Multiply = new PunctuatorToken("*");
        //public static readonly PunctuatorToken Divide = new PunctuatorToken("/");
        //public static readonly PunctuatorToken Modulo = new PunctuatorToken("%");
        //public static readonly PunctuatorToken Exponent = new PunctuatorToken("^");

        //private static readonly Dictionary<string, PunctuatorToken> Lookup = new Dictionary<string, PunctuatorToken>
        //    {
        //        {"(", LeftParenthesis},
        //        {")", RightParenthesis},
        //        {",", Comma},
        //        {"+", Plus},
        //        {"-", Minus},
        //        {"*", Multiply},
        //        {"/", Divide},
        //        {"%", Modulo},
        //        {"^", Exponent}
        //    };
    }
}