namespace ExpressionEngine
{
    class UnaryExpression : Expression
    {
        public OperatorType Operator;

        public Expression Value;

        public override double Evaluate()
        {
            switch (Operator)
            {
                case OperatorType.UnaryPlus:
                    return Value.Evaluate();
                case OperatorType.UnaryMinus:
                    return Value.Evaluate() * -1;
            }
            throw new EvaluatorException("Invalid operator type.");
        }
    }
}
