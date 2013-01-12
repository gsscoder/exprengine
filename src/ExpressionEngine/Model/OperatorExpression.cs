namespace ExpressionEngine.Internal.Model
{
    abstract class OperatorExpression : Expression
    {
        protected OperatorExpression()
        {
        }

        //protected OperatorExpression(OperatorType @operator)
        //{
        //    Operator = @operator;
        //}

        public OperatorType Operator { get; set; }
    }
}