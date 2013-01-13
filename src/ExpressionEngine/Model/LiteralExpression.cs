

using ExpressionEngine.Core;

namespace ExpressionEngine.Internal.Model
{
    sealed class LiteralExpression : Expression
    {
        private LiteralExpression() { }

        public LiteralExpression(object value)
        {
            Value = value;
        }

        public object Value { get; private set; } // defined object to open to non-number computations

        public override PrimitiveType ResultType
        {
            get { return TypeService.ToPrimitiveType(Value.GetType()); }
        }

        public override void Accept(ExpressionVisitor visitor)
        {
            visitor.VisitLiteral(this);
        }
    }
}