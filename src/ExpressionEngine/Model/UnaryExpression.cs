namespace ExpressionEngine.Internal.Model
{
    sealed class UnaryExpression : OperatorExpression
    {
        //public UnaryExpression(OperatorType @operator)
        //    : base(@operator)
        //{
        //}

        public Expression Value { get; set; }

        public override PrimitiveType ResultType
        {
            get { return Value.ResultType; }
        }

        public override void Accept(ExpressionVisitor visitor)
        {
            visitor.VisitUnary(this);
        }
    }
}