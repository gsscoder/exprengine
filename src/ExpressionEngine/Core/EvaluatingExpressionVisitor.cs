using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Model = ExpressionEngine.Internal.Model;

namespace ExpressionEngine.Internal
{
    sealed class EvaluatingExpressionVisitor : ExpressionVisitor
    {
        public EvaluatingExpressionVisitor(IDictionary<string, object> variables,
                                           IDictionary<string, Func<object[], object>> functions)
            : base(variables, functions)
        {

        }

        public override void VisitLiteral(Model.LiteralExpression expression)
        {
            _result = expression.Value;
        }

        public override void VisitUnary(Model.UnaryExpression expression)
        {
            expression.Value.Accept(this);
            switch (expression.Operator)
            {
                case Model.OperatorType.UnaryPlus:
                    break;
                case Model.OperatorType.UnaryMinus:
                    _result = Kernel.Instance.Primitives.ToReal(_result) * -1;
                    break;
                default:
                    throw new ExpressionException("Invalid unary operator type.");
            }
        }

        public override void VisitFunction(Model.FunctionExpression expression)
        {
            var name = expression.Name;
            var argsList = new List<object>(expression.Arguments.Count);
            expression.Arguments.ForEach(arg =>
            {
                arg.Accept(this);
                argsList.Add(_result);
            });
            var args = argsList.ToArray();
            if (Kernel.Instance.BuiltIn.IsBuiltInFunction(name))
            {
                _result = Kernel.Instance.BuiltIn.ExecuteBuiltInFunction(expression.Name, args);
            }
            else
            {
                _result = UserDefinedFunctions[name](args);
            }
        }

        public override void VisitBinary(Model.BinaryExpression expression)
        {
            Func<object> left = () =>
            {
                expression.Left.Accept(this);
                var leftValue = _result;
                return leftValue;
            };
            Func<object> right = () =>
            {
                expression.Right.Accept(this);
                var rightValue = _result;
                return rightValue;
            };
            var primitives = Kernel.Instance.Primitives;
            switch (expression.Operator)
            {
                case Model.OperatorType.Add:
                    _result = primitives.ToReal(left()) + primitives.ToReal(right());
                    break;
                case Model.OperatorType.Subtract:
                    _result = primitives.ToReal(left()) - primitives.ToReal(right());
                    break;
                case Model.OperatorType.Multiply:
                    _result = primitives.ToReal(left()) * primitives.ToReal(right());
                    break;
                case Model.OperatorType.Divide:
                    _result = primitives.ToReal(left()) / primitives.ToReal(right());
                    break;
                case Model.OperatorType.Modulo:
                    _result = primitives.ToReal(left()) % primitives.ToReal(right());
                    break;
                case Model.OperatorType.Exponent:
                    _result = Math.Pow(primitives.ToReal(left()), primitives.ToReal(right()));
                    break;
                default:
                    throw new ExpressionException("Invalid binary operator type.");
            }
        }

        public override void VisitVariable(Model.VariableExpression expression)
        {
            var name = expression.Name;
            if (Kernel.Instance.BuiltIn.IsBuiltInVariable(name))
            {
                _result = Kernel.Instance.BuiltIn.GetBuiltInVariable(name);
            }
            else
            {
                if (!UserDefinedVariables.ContainsKey(name))
                {
                    throw new ExpressionException(string.Format(CultureInfo.InvariantCulture, "Undefined variable: '{0}'.", name));
                }
                _result = UserDefinedVariables[name];
            }
        }

        public override object Result { get { return _result; } }

        private object _result;
    }
}
