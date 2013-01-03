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
using System.Globalization;
#endregion

namespace ExpressionEngine
{
    enum TokenType : byte
    {
        Plus,           /*  +  */
        Minus,          /*  -  */
        Star,           /*  *  */
        Slash,          /*  /  */
        Percent,        /*  %  */
        OpenBracket,    /*  (  */
        CloseBracket,   /*  )  */
        Comma,          /*  ,  */
        Caret,          /*  ^  */
        Literal,
        Identifier
    }

    sealed class Token
    {
        private Token(string text)
        {
            Text = text;
        }

        private Token(string text, TokenType type)
        {
            Text = text;
            Type = type;
        }

        private Token(string text, double value)
        {
            Type = TokenType.Literal;
            Text = text;
            Value = value;
        }

        public string Text;

        public double Value = double.NaN;

        public TokenType Type;

        public bool IsPlus()
        {
            return Type == TokenType.Plus;
        }

        public bool IsMinus()
        {
            return Type == TokenType.Minus;
        }

        public bool IsStar()
        {
            return Type == TokenType.Star;
        }

        public bool IsSlash()
        {
            return Type == TokenType.Slash;
        }

        public bool IsOperator()
        {
            return Type == TokenType.Plus || Type == TokenType.Minus ||
                   Type == TokenType.Star || Type == TokenType.Slash;
        }

        public bool IsOpenBracket()
        {
            return Type == TokenType.OpenBracket;
        }

        public bool IsCloseBracket()
        {
            return Type == TokenType.CloseBracket;
        }

        public bool IsComma()
        {
            return Type == TokenType.Comma;
        }

        public bool IsLiteral()
        {
            return Type == TokenType.Literal;
        }

        public bool IsIdentifier()
        {
            return Type == TokenType.Identifier;
        }

        public bool IsCaret()
        {
            return Type == TokenType.Caret;
        }

        public bool IsPercent()
        {
            return Type == TokenType.Percent;
        }

        public static Token Punctuator(int @char)
        {
            var token = new Token(new string((char) @char, 1));
            switch (token.Text)
            {
                case "+":
                    token.Type = TokenType.Plus;
                    break;
                case "-":
                    token.Type = TokenType.Minus;
                    break;
                case "*":
                    token.Type = TokenType.Star;
                    break;
                case "/":
                    token.Type = TokenType.Slash;
                    break;
                case "%":
                    token.Type = TokenType.Percent;
                    break;
                case "(":
                    token.Type = TokenType.OpenBracket;
                    break;
                case ")":
                    token.Type = TokenType.CloseBracket;
                    break;
                case ",":
                    token.Type = TokenType.Comma;
                    break;
                case "^":
                    token.Type = TokenType.Caret;
                    break;
                default:
                    throw new ExpressionException("Invalid token.");
            }
            return token;
        }

        public double ToDouble()
        {
            return Convert.ToDouble(Text, CultureInfo.InvariantCulture);
        }

        public Model.OperatorType GetAdditiveOperator()
        {
            switch (Type)
            {
                case TokenType.Plus:
                    return Model.OperatorType.Add;
                case TokenType.Minus:
                    return Model.OperatorType.Subtract;
            }
            throw new ExpressionException("Expected additive (+, -) binary operator.");
        }

        public Model.OperatorType GetMultiplicativeOperator()
        {
            switch (Type)
            {
                case TokenType.Star:
                    return Model.OperatorType.Multiply;
                case TokenType.Slash:
                    return Model.OperatorType.Divide;
                case TokenType.Percent:
                    return Model.OperatorType.Modulo;
            }
            throw new ExpressionException("Expected multiplicative (*, /, %) binary operator.");
        }

        public static Token Literal(string text, double value)
        {
            return new Token(text, value);
        }

        public static Token Identifier(string text)
        {
            return new Token(text, TokenType.Identifier);
        }
    }
}