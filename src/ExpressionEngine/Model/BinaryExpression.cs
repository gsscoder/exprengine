

namespace ExpressionEngine.Internal.Model
{
    sealed class BinaryExpression : OperatorExpression
    {
        //public BinaryExpression(OperatorType @operator)
        //    : base(@operator)
        //{
        //}

        public Expression Left { get; set; }

        public Expression Right { get; set; }

        public override PrimitiveType ResultType
        {
            get
            {
                if (Left.ResultType == Right.ResultType)
                {
                    return Left.ResultType;
                }
                return PrimitiveType.Real;
            }
        }

        public override void Accept(ExpressionVisitor visitor)
        {
            visitor.VisitBinary(this);
        }
    }
}