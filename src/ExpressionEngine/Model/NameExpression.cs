namespace ExpressionEngine.Internal.Model
{
    abstract class NameExpression : Expression
    {
        public string Name { get; protected set; }
    }
}