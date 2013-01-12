

namespace ExpressionEngine.Internal.Model
{
    abstract class Expression
    {
        protected Expression()
        {
        }

        public abstract PrimitiveType ResultType { get; }

        public abstract void Accept(ExpressionVisitor visitor);
    }
}