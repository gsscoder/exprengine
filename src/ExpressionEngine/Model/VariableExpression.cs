namespace ExpressionEngine.Internal.Model
{
    class VariableExpression : NameExpression
    {
        public VariableExpression(string name)
        {
            Name = name;
        }

        public override PrimitiveType ResultType
        {
            get { return PrimitiveType.Real; }
        }

        public override void Accept(ExpressionVisitor visitor)
        {
            visitor.VisitVariable(this);
        }
    }
}