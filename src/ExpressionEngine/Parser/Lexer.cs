#region License
//
// Expression Engine Library: Lexer.cs
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
            _escapeBuffer = new StringBuilder(3);
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

            var c = _text.NextChar();

            switch (c)
            {
                // Punctuators
                case '(': return new PunctuatorToken(TokenType.LeftParenthesis);
                case ')': return new PunctuatorToken(TokenType.RightParenthesis);
                case '+': return new PunctuatorToken(TokenType.Plus);
                case '-': return new PunctuatorToken(TokenType.Minus);
                case '*': return new PunctuatorToken(TokenType.Multiply);
                case '/': return new PunctuatorToken(TokenType.Divide);
                case '%': return new PunctuatorToken(TokenType.Modulo);
                case ',': return new PunctuatorToken(TokenType.Comma);
                case '=':
                    if (_text.PeekChar() == '=')
                    {
                        _text.NextChar();
                        return new PunctuatorToken(TokenType.Equality);
                    }
                    throw new EvaluatorException(_text.Line, "Unexpected token '='.");
                case '!':
                    if (_text.PeekChar() == '=')
                    {
                        _text.NextChar();
                        return new PunctuatorToken(TokenType.Inequality);
                    }
                    throw new EvaluatorException(_text.Line, "Unexpected token '!'.");
                case '<':
                    if (_text.PeekChar() == '=')
                    {
                        _text.NextChar();
                        return new PunctuatorToken(TokenType.LessThanOrEqual);
                    }
                    return new PunctuatorToken(TokenType.LessThan);
                case '>':
                    if (_text.PeekChar() == '=')
                    {
                        _text.NextChar();
                        return new PunctuatorToken(TokenType.GreaterThanOrEqual);
                    }
                    return new PunctuatorToken(TokenType.GreaterThan);
                // String literals
                case '"':
                    _buffer.Length = 0;
                    while (true) {
                        c = _text.NextChar();
                        if (c == '"') {
                            break;
                        }
                        if (c == '\0') { throw new EvaluatorException(_text.Column, "Unexpected end of input in string literal."); }
                        if (c == '\\')
                        {
                            c = _text.NextChar();
                            switch (c)
                            {
                                case '"': // double quote
                                    _buffer.Append('\x22');
                                    break;
                                case 'n': // line feed
                                    _buffer.Append('\x0A');
                                    break;
                                case 'r': // carriage return
                                    _buffer.Append('\x0D');
                                    break;
                                case 't': // horizontal tab
                                    _buffer.Append('\x09');
                                    break;
                                case '0':
                                case '1':
                                case '2':
                                    _buffer.Append(ScanDecimalEscapeSequence(c));
                                    break;
                                default:
                                    throw new EvaluatorException(_text.Column, "Invalid escape sequence.");
                            }
                        }
                        else {
                            _buffer.Append(c);
                        }
                    }
                    return new LiteralToken(_buffer.ToString());
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
                    var seenDot = (c == '.');
                    _buffer.Length = 0;
                    _buffer.Append(c);
                    while (true) {
                        c = _text.PeekChar();
                        if (c == '.') {
                            if (seenDot) { throw new EvaluatorException(_text.Column, "Bad numeric literal."); }
                            seenDot = true;
                        }
                        else if (c == 'e' || c == 'E') {
                            _buffer.Append(c);
                            _text.NextChar();
                            c = _text.PeekChar();
                            if (c == '+' || c == '-' || IsDigit(c)) {
                                _buffer.Append(c);
                                _text.NextChar();
                                while (true) {
                                    c = _text.PeekChar();
                                    if (!IsDigit(c) || c == '\0') {
                                        break;
                                    }
                                    _buffer.Append(c);
                                    _text.NextChar();
                                }
                                return new LiteralToken(ParseNumber(_buffer, seenDot, true));
                            }
                            else {
                                throw new EvaluatorException(_text.Line, "Invalid character after numeric literal exponent.");
                            }
                        }
                        else if (!IsDigit(c) || c == '\0') {
                            break;
                        }
                        _buffer.Append(c);
                        _text.NextChar();
                    }
                    return new LiteralToken(ParseNumber(_buffer, seenDot, false));
                case '.':
                    if (IsDigit(_text.PeekChar()))
                    {
                        goto case '9';
                    }
                    throw new EvaluatorException(_text.Column, "Invalid numeric literal.");
                // Line terminators
                case '\xD':
                case '\xA':
                case '\x2028':
                case '\x2029':
                    throw new EvaluatorException(_text.Column, "Line terminator is not allowed.");
                // EOF
                case '\0':
                    return null;
                // Identifier, boolean literal or invalid token
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
                        var buf = _buffer.ToString();
                        if (string.CompareOrdinal(buf, "true") == 0)
                        {
                            return new LiteralToken(true);
                        }
                        if (string.CompareOrdinal(buf, "false") == 0)
                        {
                            return new LiteralToken(false);
                        }
                        return new IdentifierToken(buf);
                    }
                    throw new EvaluatorException(_text.Column, string.Format(CultureInfo.InvariantCulture, "Unexpected character '{0}'.", c));
            }
        }

        public static bool IsWhiteSpace(char c)
        {
            switch (c)
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
                    return (c > 127 && char.IsWhiteSpace(c));
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
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                if (_text != null)
                {
                    _text.Dispose();
                }
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

        private static double ParseNumber(StringBuilder buf, bool seenDot, bool seenExp)
        {
            var styles = NumberStyles.Number;
            if (seenDot)
            {
                styles = NumberStyles.AllowDecimalPoint;
                if (seenExp)
                {
                    styles |= NumberStyles.AllowExponent;
                }
            }
            else
            {
                styles = seenExp ? NumberStyles.AllowExponent : NumberStyles.None;
            }
            return double.Parse(buf.ToString(), styles, NumberFormatInfo);
        }

        private char ScanDecimalEscapeSequence(char firstChar)
        {
            _escapeBuffer.Length = 0;
            _escapeBuffer.Append(new string(firstChar, 1));
            for (var i = 0; i < 2; i++)
            {
                var c = _text.PeekChar();
                if (!(c >= '0' && c <= '9'))
                {
                    throw new EvaluatorException(_text.Line, "Invalid decimal escape sequence.");
                }
                _escapeBuffer.Append(c);
                _text.NextChar();
            }
            byte e;
            if (byte.TryParse(_escapeBuffer.ToString(), out e))
            {
                return Encoding.ASCII.GetChars(new byte[] {e})[0];
            }
            throw new EvaluatorException(_text.Line, "Invalid decimal escape sequence.");
        }

        ~Lexer()
        {
            Dispose(false);
        }

        private static readonly NumberFormatInfo NumberFormatInfo = CultureInfo.InvariantCulture.NumberFormat;
        private bool _disposed;
        private Token _current;
        private Token _next;
        private readonly StringBuilder _escapeBuffer;
        private readonly StringBuilder _buffer;
        private readonly Text _text;
    }
}