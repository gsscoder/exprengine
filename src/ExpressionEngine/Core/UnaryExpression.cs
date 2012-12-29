namespace ExpressionEngine.Model
{
    sealed class UnaryExpression : IExpression
    {
        public OperatorType Operator;

        public Model.IExpression Value;

        public double Evaluate()
        {
            switch (Operator)
            {
                case OperatorType.UnaryPlus:
                    return Value.Evaluate();
                case OperatorType.UnaryMinus:
                    return Value.Evaluate() * -1;
            }
            throw new ExpressionException("Invalid operator type.");
        }
    }
}
