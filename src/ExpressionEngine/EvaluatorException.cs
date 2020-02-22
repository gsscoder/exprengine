using System;
using System.Runtime.Serialization;

namespace ExpressionEngine
{
    /// <summary>
    /// The exception that is thrown when an expression can't be evaluated by <see cref="ExpressionEngine.Context"/>.
    /// </summary>
    [Serializable]
    public class EvaluatorException : Exception
    {
        public EvaluatorException()
        {
        }

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
            get { return (int) Data[DataColumnNumber];  }
            set { Data[DataColumnNumber] = value; }
        }

        private const string DataColumnNumber = "Column";
    }
}