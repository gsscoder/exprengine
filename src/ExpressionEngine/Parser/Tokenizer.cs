#region License
//
// Expression Engine Library: LiteralExpression.cs
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
using System.IO;
#endregion

namespace ExpressionEngine.Internal
{
    sealed class Tokenizer : IDisposable
    {
        public Tokenizer(TextReader reader)
        {
            _reader = reader;
            _column = -1;
            _line = -1;
        }

        public static Tokenizer OfString(string value)
        {
            return new Tokenizer(new StringReader(value));
        }

        public int Column { get { return _column; } }

        public int Line { get { return _line; } }

        public char NextChar()
        {
            var c = _reader.Read();
            if (c == -1)
            {
                return '\0';
            }
            _column++;
            if (_line < 0 || c == '\xA' || c == '\xD' || c == '\x2028' || c== '\x2029')
            {
                // Increments line count after first read or line terminator
                _line++;
            }
            return (char) c;
        }

        public char PeekChar()
        {
            var c = _reader.Peek();
            return c != -1 ? (char) c : '\0';
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                if (_reader != null)
                {
                    _reader.Dispose();
                }
                _disposed = true;
            }
        }

        ~Tokenizer()
        {
            Dispose(false);
        }

        private int _column;
        private int _line;
        private readonly TextReader _reader;
        private bool _disposed;
    }
}