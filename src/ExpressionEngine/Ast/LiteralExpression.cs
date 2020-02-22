sealed class LiteralExpression : Expression
{
    private LiteralExpression() {}

    public LiteralExpression(object value)
    {
        Value = value;
    }

    public object Value { get; private set; }

    public override PrimitiveType ResultType
    {
        get { return TypeConverter.ToPrimitiveType(Value.GetType()); }
    }

    public override void Accept(Visitor visitor)
    {
        visitor.Visit(this);
    }
}