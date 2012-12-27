#region License
//
// Expression Engine Library: Scanner.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2012 Giacomo Stelluti Scala
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
using System.IO;
using System.Text;
#endregion

namespace ExpressionEngine
{
    sealed class Scanner : IDisposable
    {
        private Scanner() {}

        public Scanner(TextReader reader)
        {
            _reader = reader;
            ColumnNumber = 0;
            _position = -1;
            Scan();
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        public Token NextToken()
        {
            _current = _next;
            Scan();
            return _current;
        }

        public Token PeekToken()
        {
            return _next;
        }      

        private void Scan()
        {
            Token token;
            SkipWhiteSpace();

            Advance();

            //if (IsPunctuatorChar(_c))
            //{
            //    // '(', ')', '+', '-', '*', '/', '^'
            //    token = ScanPunctuator();
            //}
            //else if (IsLiteralChar(_c))
            //{
            //    // [+|-]0-9, 0-9.0-9, .0-9,
            //    token = ScanNumericLiteral();
            //}
            if (IsLiteralOrPunctuator(_c))
            {
                token = ScanLiteralOrPunctuator();
            }
            else if (IsLineTerminator(_c))
            {
                // {CR} + {LF}, {CR}, {LF}
                throw new EvaluatorException(ColumnNumber, "Line terminator is not allowed.");
            }
            else if (_c == -1)
            {
                // [EOF]
                token = null;
            }
            else
            {
                throw new EvaluatorException(ColumnNumber, string.Format(CultureInfo.InvariantCulture, "Unexpected character '{0}'.", (char) _c));                
            }
            _position++;
            _next = token;
        }

        public int ColumnNumber { get; private set; }

        public int Position
        {
            get
            {
                if (_current == null && _next == null)
                {
                    _position = -1;
                }
                return _position;
            }
        }

        public bool IsEof()
        {
            return _next == null;
        }

        private void Advance()
        {
            ColumnNumber++;
            _c = _reader.Read();
        }

        private int Peek()
        {
            return _reader.Peek();
        }

        #region Specialized Readers
        private Token ScanPunctuator()
        {
            return Token.Punctuator(_c);
        }

        //private Token ScanNumericLiteral()
        //{
        //    var numericText = new StringBuilder(new string((char) _c, 1));
        //    while (true)
        //    {
        //        int c = Peek();
        //        if (!IsLiteralChar(c) || c == -1)
        //        {
        //            break;
        //        }
        //        numericText.Append((char) c);
        //        Advance();
        //    }
        //    var number = numericText.ToString();
        //    if (number.StartsWith("."))
        //    {
        //        number = "0" + number;
        //    }
        //    return Token.Literal(number);
        //}

        private Token ScanLiteral()
        {
            var numericText = new StringBuilder(new string((char)_c, 1));
            while (true)
            {
                int c = Peek();
                if (!IsLiteralChar(c) || c == -1)
                {
                    break;
                }
                numericText.Append((char)c);
                Advance();
            }
            var number = numericText.ToString();
            if (number[0] == '.')
            {
                number = "0" + number;
            }
            else if (number[0] == '+' && number[1] == '.')
            {
                number = "+0" + number.Substring(1);
            }
            else if (number[0] == '-' && number[1] == '.')
            {
                number = "-0" + number.Substring(1);
            }
            return Token.Literal(number);
        }

        private Token ScanLiteralOrPunctuator()
        {
            int n = _reader.Peek();
            if (ColumnNumber == 1)
            {
                if (IsLiteralChar(_c) || (IsSign(_c) && IsLiteralChar(n)))
                {
                    return ScanLiteral();
                }
                else if (_c == '(')
                {
                    return ScanPunctuator();
                }
            }
            else
            {
                if (IsLiteralChar(_c) || (IsSign(_c) && IsLiteralChar(n)))
                {
                    return ScanLiteral();
                }
                else if (IsPunctuatorChar(_c))
                {
                    return ScanPunctuator();
                }
            }
            throw new EvaluatorException(ColumnNumber, string.Format(CultureInfo.InvariantCulture, "Unexpected character '{0}'.", (char)_c));
        }

        private void SkipWhiteSpace()
        {
            while (true)
            {
                if (!IsWhiteSpace(Peek()))
                {
                    break;
                }
                Advance();
            }
        }
        #endregion

        #region Char Helpers
        private static bool IsPunctuatorChar(int c)
        {
            return c == '(' || c == ')' || c == '+' || c == '-' ||
                   c == '*' || c == '/' || c == '^' || c == '%';
        }

        private static bool IsWhiteSpace(int c)
        {
            return c == 0x09 || c == 0x0B || c == 0x0C || c == 0x20 || c == 0xA0 ||
                c == 0x1680 || c == 0x180E || (c >= 8192 && c <= 8202) || c == 0x202F ||
                c == 0x205F || c == 0x3000 || c == 0xFEFF;
        }

        private static bool IsLineTerminator(int c)
        {
            return c == 0x0A || c == 0x0D || c == 0x2028 || c == 0x2029;
        }

        private static bool IsLiteralChar(int c)
        {
            return c == '.' || (c >= '0' && c <= '9');
        }

        private static bool IsLiteralOrPunctuator(int c)
        {
            return c == '(' || c == ')' || c == '+' || c == '-' ||
                   c == '*' || c == '/' || c == '^' || c == '%' ||
                   c == '.' || (c >= '0' && c <= '9');
        }

        private static bool IsSign(int c)
        {
            return c == '+' || c == '-';
        }
        #endregion

        private readonly TextReader _reader;
        private int _c;
        private Token _current;
        private Token _next;
        private int _position;
    }
}