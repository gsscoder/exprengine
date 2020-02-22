sealed class UnaryExpression : OperatorExpression
{
    public Expression Value { get; set; }

    public override PrimitiveType ResultType
    {
        get { return Value.ResultType; }
    }

    public override void Accept(Visitor visitor)
    {
        visitor.Visit(this);
    }
}