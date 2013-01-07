#region License
//
// Expression Engine Library: Token.cs
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

#endregion

namespace ExpressionEngine.Core
{
    abstract class Token
    {
        protected Token() {}

        protected Token(string text)
        {
            _text = text;
        }

        public static string StringOf(Token token)
        {
            return token == null ? "end of input" : string.Format("'{0}'", token.Text);
        }

        public static string StringOf(Token[] tokens)
        {
            var builder = new StringBuilder(4 * tokens.Length);
            foreach (var token in tokens)
            {
                builder.Append("'");
                builder.Append(token.Text);
                builder.Append("', ");
            }
            return builder.ToString(0, builder.Length - 2);
        }

        public virtual string Text
        {
            get { return _text; }
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", this.Text, this.GetType().Name);
        }

        private readonly string _text;
    }

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

        public Model.OperatorType ToOperatorType()
        {
            if (this == PunctuatorToken.Plus)
            {
                return Model.OperatorType.Add;
            }
            if (this == PunctuatorToken.Minus)
            {
                return Model.OperatorType.Subtract;
            }
            if (this == PunctuatorToken.Multiply)
            {
                return Model.OperatorType.Multiply;
            }
            if (this == PunctuatorToken.Divide)
            {
                return Model.OperatorType.Divide;
            }
            if (this == PunctuatorToken.Modulo)
            {
                return Model.OperatorType.Modulo;
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

    sealed class LiteralToken : Token
    {
        public LiteralToken(object value)
        {
            _value = value;
        }

        public override string Text
        {
            get
            {
                return _value.ToString();
            }
        }

        public object Value
        {
            get { return _value; }
        }

        private readonly object _value;
    }

    sealed class IdentifierToken : Token
    {
        public IdentifierToken(string text)
            : base(text)
        {
        }
    }
}