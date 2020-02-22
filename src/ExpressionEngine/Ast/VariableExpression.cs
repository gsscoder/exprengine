class VariableExpression : NameExpression
{
    public VariableExpression(string name)
    {
        Name = name;
    }

    public override PrimitiveType ResultType
    {
        get { return PrimitiveType.Number; }
    }

    public override void Accept(Visitor visitor)
    {
        visitor.Visit(this);
    }
}