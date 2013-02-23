using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionEngine.Internal.Ast
{
    class MemberAcessExpression : OperatorExpression 
    {
        public Expression Target { get; set; }

        public override PrimitiveType ResultType
        {
            get { return PrimitiveType.Object; }
        }

        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
