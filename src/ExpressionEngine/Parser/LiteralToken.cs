

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

        private readonly object _value;
    }
}