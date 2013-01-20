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
using System.Diagnostics;
using System.Globalization;
using System.Text;

#endregion

namespace ExpressionEngine.Internal
{
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
        //    return token == null ? "end of input" : string.Format("'{0}'", token.Text);
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
                //case TokenType.Exponent:
                //    return "^";
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
}