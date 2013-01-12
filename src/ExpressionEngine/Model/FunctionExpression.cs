using System.Collections.Generic;


namespace ExpressionEngine.Internal.Model
{
    sealed class FunctionExpression : NameExpression
    {
        public FunctionExpression(string name)
        {
            Name = name;
            Arguments = new List<Expression>();
        }

        public List<Expression> Arguments { get; private set; }

        public override PrimitiveType ResultType
        {
            get { return PrimitiveType.Real; }
        }

        public override void Accept(ExpressionVisitor visitor)
        {
            visitor.VisitFunction(this);
        }
    }
}