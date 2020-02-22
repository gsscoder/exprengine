abstract class Visitor
{
    private Visitor() {}

    protected Visitor(Scope globalScope)
    {
        GlobalScope = globalScope;
    }

    public abstract void Visit(LiteralExpression expression);

    public abstract void Visit(UnaryExpression expression);

    public abstract void Visit(FunctionCallExpression callExpression);

    public abstract void Visit(BinaryExpression expression);

    public abstract void Visit(VariableExpression expression);

    protected Scope GlobalScope { get; private set; }

    public abstract object Result { get; }

    public static Visitor Create(Scope global)
    {
        return new EvaluatingVisitor(global);
    }
}