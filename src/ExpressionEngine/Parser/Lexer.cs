#region License
//
// Expression Engine Library: Scanner.cs
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
using System.Text;
#endregion

namespace ExpressionEngine.Internal
{
    /// <summary>
    /// Reads characters from <see cref="ExpressionEngine.Internal.Text"/> and returns instances
    /// of <see cref="ExpressionEngine.Internal.Token"/> derived types.
    /// </summary>
    sealed class Lexer : IDisposable
    {
        private Lexer() {}

        public Lexer(Text text)
        {
            _text = text;
            _buffer = new StringBuilder(64);
            _next = LexToken();
        }

        public Token NextToken()
        {
            _current = _next;
            _next = LexToken();
            return _current;
        }

        public Token PeekToken()
        {
            return _next;
        }      

        private Token LexToken()
        {
            SkipWhiteSpace();

            char c = _text.NextChar();

            switch (c)
            {
                // Punctuators
                case '(': return PunctuatorToken.LeftParenthesis;
                case ')': return PunctuatorToken.RightParenthesis;
                case '+': return PunctuatorToken.Plus;
                case '-': return PunctuatorToken.Minus;
                case '*': return PunctuatorToken.Multiply;
                case '/': return PunctuatorToken.Divide;
                case '^': return PunctuatorToken.Exponent;
                case '%': return PunctuatorToken.Modulo;
                case ',': return PunctuatorToken.Comma;
                // Numeric literals
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                //ParseNumber: {
                    bool seenDot = (c == '.');
                    _buffer.Length = 0;
                    _buffer.Append(c);
                    while (true) {
                        c = _text.PeekChar();
                        if (c == '.') {
                            if (seenDot) { throw new ExpressionException(_text.Column, "Bad numeric literal."); }
                            seenDot = true;
                        }
                        else if (!IsDigit(c) || c == '\0') {
                            break;
                        }
                        _buffer.Append(c);
                        _text.NextChar();
                    }
                    if (seenDot) {
                        return new LiteralToken(double.Parse(_buffer.ToString(), NumberStyles.AllowDecimalPoint, NumberFormatInfo));
                    } 
                    return new LiteralToken(long.Parse(_buffer.ToString(), NumberStyles.None));
                //}
                case '.':
                    if (IsDigit(_text.PeekChar()))
                    {
                        goto case '9';
                    }
                    throw new ExpressionException(_text.Column, "Bad numeric literal.");
                // Line terminators
                case '\xD':
                case '\xA':
                case '\x2028':
                case '\x2029':
                    throw new ExpressionException(_text.Column, "Line terminator is not allowed.");
                // EOF
                case '\0':
                    return null;
                // Identifier or Invalid token
                default:
                    if (IsIdentifierChar(c)) {
                        _buffer.Length = 0;
                        _buffer.Append(c);
                        while (true) {
                            c = _text.PeekChar();
                            if (!IsIdentifierChar(c) || c == '\0') {
                                break;
                            }
                            _buffer.Append(c);
                            _text.NextChar();
                        }
                        return new IdentifierToken(_buffer.ToString());
                    }
                    throw new ExpressionException(_text.Column, string.Format(CultureInfo.InvariantCulture, "Unexpected character '{0}'.", c));
            }
        }

        public static bool IsWhiteSpace(char ch)
        {
            switch (ch)
            {
                //// Line separators
                //case '\xD':
                //case '\xA':
                //case '\x2028':
                //case '\x2029':

                // Regular
                case '\f':
                case '\v':
                case ' ':
                case '\t':
                    return true;

                // Unicode
                default:
                    return (ch > 127 && char.IsWhiteSpace(ch));
            }
        }

        public bool IsEof()
        {
            return _next == null;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        public int Column { get { return _text.Column; } }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _text.Dispose();
                _disposed = true;
            }
        }

        private void SkipWhiteSpace()
        {
            while (IsWhiteSpace(_text.PeekChar()))
            {
                _text.NextChar();
            }
        }

        private static bool IsDigit(char ch)
        {
            return (ch >= '0') && (ch <= '9');
        }

        private static bool IsIdentifierChar(char ch)
        {
            return char.IsLetterOrDigit(ch) || (ch == '_');
        }

        ~Lexer()
        {
            Dispose(false);
        }

        private static readonly NumberFormatInfo NumberFormatInfo = CultureInfo.InvariantCulture.NumberFormat;
        private bool _disposed;
        private Token _current;
        private Token _next;
        private readonly StringBuilder _buffer;
        private readonly Text _text;
    }
}