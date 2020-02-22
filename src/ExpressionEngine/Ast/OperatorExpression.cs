abstract class OperatorExpression : Expression
{
    protected OperatorExpression()
    {
    }

    public OperatorType Operator { get; set; }
}