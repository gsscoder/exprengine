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
using System.IO;
using System.Text;
#endregion

namespace ExpressionEngine
{
    enum NumberFormats : byte
    {
        Integer,
        Decimal
        // hex and more in future..
    }

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

            if (IsPunctuatorChar(_c))
            {
                // '(', ')', '+', '-', '*', '/', '^', '%', ','
                token = ScanPunctuator();
            }
            else if (IsIdentifierStartChar(_c))
            {
                // [_|a|A-z|Z][_|a|A-z|Z|0-9]
                token = ScanIdentifier();
            }
            else if (IsLiteralChar(_c))
            {
                // 0-9, 0-9.0-9, .0-9
                token = ScanLiteral();
            }
            else if (IsLineTerminator(_c))
            {
                // {CR} + {LF}, {CR}, {LF}
                throw new ExpressionException(ColumnNumber, "Line terminator is not allowed.");
            }
            else if (_c == -1)
            {
                // [EOF]
                token = null;
            }
            else
            {
                throw new ExpressionException(ColumnNumber, string.Format(CultureInfo.InvariantCulture, "Unexpected character '{0}'.", (char) _c));                
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

		public void Dispose()
        {
			Dispose(true);

			GC.SuppressFinalize(this);
        }

		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				_reader.Dispose();
				_disposed = true;
			}	
		}

        private void Advance()
        {
            ColumnNumber++;
            _c = _reader.Read();
        }

        #region Specialized Readers
        private Token ScanPunctuator()
        {
            return Token.Punctuator(_c);
        }

        private Token ScanLiteral()
        {
            var hasDot = _c == '.';
            var number = new StringBuilder(new string((char) _c, 1));
            while (true)
            {
                int c = _reader.Peek();
                if (!IsLiteralChar(c) || c == -1)
                {
                    break;
                }
                if (c == '.') { hasDot = true; }
                number.Append((char) c);
                Advance();
            }
            var text = number.ToString();
            return Token.Literal(text, ParseNumber(text, hasDot ? NumberFormats.Decimal : NumberFormats.Integer));
        }

        private Token ScanIdentifier()
        {
            var identText = new StringBuilder(new string((char) _c, 1));
            while (true)
            {
                int c = _reader.Peek();
                if (!IsIdentifierChar((char) c) || c == -1)  //if (!char.IsLetter((char) c) || c == -1)
                {
                    break;
                }
                identText.Append((char) c);
                Advance();
            }
            return Token.Identifier(identText.ToString());
        }

        private void SkipWhiteSpace()
        {
            while (true)
            {
                if (!IsWhiteSpace(_reader.Peek()))
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
                   c == '*' || c == '/' || c == '^' || c == '%' ||
                   c == ',';
        }

        public static bool IsWhiteSpace(int c)
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

        private static bool IsIdentifierStartChar(int c)
        {
            var cat = char.GetUnicodeCategory((char) c);
            return c == '_' ||
                   cat == UnicodeCategory.UppercaseLetter ||
                   cat == UnicodeCategory.LowercaseLetter ||
                   cat == UnicodeCategory.TitlecaseLetter;
        }

        private static bool IsIdentifierChar(int c)
        {
            var cat = char.GetUnicodeCategory((char)c);
            return c == '_' ||
                   cat == UnicodeCategory.UppercaseLetter ||
                   cat == UnicodeCategory.LowercaseLetter ||
                   cat == UnicodeCategory.TitlecaseLetter ||
                   cat == UnicodeCategory.DecimalDigitNumber;
        }
        #endregion

        private static double ParseNumber(string text, NumberFormats numberFormat)
        {
            if (numberFormat == NumberFormats.Integer)
            {
                return (double) int.Parse(text, NumberStyles.None);

            }
            return double.Parse(text, NumberStyles.AllowDecimalPoint, NumberFormatInfo);
        }

		~Scanner()
		{
			Dispose(false);
		}

        private static readonly NumberFormatInfo NumberFormatInfo = CultureInfo.InvariantCulture.NumberFormat;
		private bool _disposed;
        private readonly TextReader _reader;
        private int _c;
        private Token _current;
        private Token _next;
        private int _position;
    }
}