using ExpressionEngine.Core;

namespace ExpressionEngine.Model
{
    class VariableExpression : Expression
    {
        public string Name;

        public override void Accept(ExpressionVisitor visitor)
        {
            visitor.VisitVariable(this);
        }
    }
}
