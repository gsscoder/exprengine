#region License
//
// Expression Engine Library: EvaluatorException.cs
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
using System.Runtime.Serialization;
#endregion

namespace ExpressionEngine
{
    [Serializable]
    public class EvaluatorException : Exception
    {
        public EvaluatorException(string message)
            : base(message)
        {
            ColumnNumber = -1;
        }

        public EvaluatorException(string message, Exception innerException)
            : base(message, innerException)
        {
            ColumnNumber = -1;
        }

        public EvaluatorException(int columnNumber, string message)
            : base(message)
        {
            ColumnNumber = columnNumber;
        }

        public EvaluatorException(int columnNumber, string message, Exception innerException)
            : base(message, innerException)
        {
            ColumnNumber = columnNumber;
        }

        protected EvaluatorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ColumnNumber = -1;
        }

        public int ColumnNumber
        {
            get { return (int) this.Data[DataColumnNumber];  }
            set { this.Data[DataColumnNumber] = value; }
        }

        private const string DataColumnNumber = "ColumnNumber";
    }
}