using System;

sealed class BinaryExpression : OperatorExpression
{
    public Expression Left { get; set; }

    public Expression Right { get; set; }

    public override PrimitiveType ResultType
    {
        get
        {
            switch (Operator)
            {
                case OperatorType.Add:
                    if (Left.ResultType == PrimitiveType.String || Right.ResultType == PrimitiveType.String)
                    {
                        return PrimitiveType.String;
                    }
                    return PrimitiveType.Number;
                case OperatorType.Subtract:
                case OperatorType.Multiply:
                case OperatorType.Divide:
                case OperatorType.Modulo:
                    return PrimitiveType.Number;
                case OperatorType.Equality:
                case OperatorType.Inequality:
                case OperatorType.LessThan:
                case OperatorType.GreaterThan:
                case OperatorType.LessThanOrEqual:
                case OperatorType.GreaterThanOrEqual:
                    return PrimitiveType.Boolean;
            }
            throw new InvalidOperationException();
        }
    }

    public override void Accept(Visitor visitor)
    {
        visitor.Visit(this);
    }
}