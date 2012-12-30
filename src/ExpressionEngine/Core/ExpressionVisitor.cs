using System;
using System.Collections.Generic;

namespace ExpressionEngine.Core
{
	abstract class ExpressionVisitor
	{
		public abstract void Visit(Model.LiteralExpression expression);

		public abstract void Visit(Model.UnaryExpression expression);

		public abstract void Visit(Model.FunctionExpression expression);

		public abstract void Visit(Model.BinaryExpression expression);

		public abstract object Result { get; }

		public class EvaluatingExpressionVisitor : ExpressionVisitor
		{
			public override void Visit(Model.LiteralExpression expression)
			{
				_result = expression.Value;
			}

			public override void Visit(Model.UnaryExpression expression)
			{
				expression.Value.Accept(this);
				switch (expression.Operator)
				{
					case Model.OperatorType.UnaryPlus:
						//_result = _result;
						break;
					case Model.OperatorType.UnaryMinus:
						_result = _result * -1;
						break;
					default:
						throw new ExpressionException("Invalid unary operator type.");
				}
			}

			public override void Visit(Model.FunctionExpression expression)
			{
				List<double> argsList = new List<double>(expression.Arguments.Count);
				expression.Arguments.ForEach(arg => {
					arg.Accept(this);
					argsList.Add(_result);
				});
				var args = argsList.ToArray();
				var builtIn = Kernel.BuiltIn.FromString(expression.Name);
            	if (builtIn == null)
            	{
                	throw new ExpressionException("Undefined function.");
            	}
				_result = builtIn.Execute(args);
			}

			public override void Visit(Model.BinaryExpression expression)
			{
				Func<double> left = () => {
					expression.Left.Accept(this);
					var leftValue = _result;
					return leftValue;
				};
				Func<double> right = () => {
					expression.Right.Accept(this);
					var rightValue = _result;
					return rightValue;
				};
	            switch (expression.Operator)
	            {
	                case Model.OperatorType.Add:
	                    _result = left() + right();
						break;
	                case Model.OperatorType.Subtract:
	                    _result =  left() - right();
						break;
	                case Model.OperatorType.Multiply:
	                    _result =  left() * right();
						break;
	                case Model.OperatorType.Divide:
						_result =  left() / right();
						break;
	                case Model.OperatorType.Exponent:
	                    _result =  Math.Pow(left(), right());
						break;
					default:
						throw new ExpressionException("Invalid binary operator type.");
				}
			}

			public override object Result
			{
				get
				{
					return _result;
				}
			}

			private double _result;
		}

		public static ExpressionVisitor Create()
		{
			return new EvaluatingExpressionVisitor();
		}
	}
}