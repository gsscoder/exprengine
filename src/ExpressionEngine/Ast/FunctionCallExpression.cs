using System.Collections.Generic;

sealed class FunctionCallExpression : NameExpression
{
    public FunctionCallExpression(string name)
    {
        Name = name;
        Arguments = new List<Expression>();
    }

    public List<Expression> Arguments { get; private set; }

    public override PrimitiveType ResultType
    {
        get { return PrimitiveType.Number; }
    }

    public override void Accept(Visitor visitor)
    {
        visitor.Visit(this);
    }
}