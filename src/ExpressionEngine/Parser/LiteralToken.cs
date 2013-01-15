using ExpressionEngine.Primitives;

namespace ExpressionEngine.Internal
{
    sealed class LiteralToken : Token
    {
        public LiteralToken(object value)
            : base(null, TokenType.Literal)
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

        public Instance ToPrimitiveType()
        {
            if (_value is double)
            {
                return new Number((double) Value);
            }
            throw new EvaluatorException("Type not supported.");
        }

        private readonly object _value;
    }
}