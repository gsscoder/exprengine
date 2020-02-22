abstract class Expression
{
    protected Expression()
    {
    }

    public abstract PrimitiveType ResultType { get; }

    public abstract void Accept(Visitor visitor);
}