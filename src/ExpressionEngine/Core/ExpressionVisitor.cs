#region License
//
// Expression Engine Library: ExpressionVisitor.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2012 - 2013 Giacomo Stelluti Scala
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.Globalization;
#endregion

namespace ExpressionEngine.Core
{
	abstract class ExpressionVisitor
	{
        private ExpressionVisitor() {}

        protected ExpressionVisitor(IDictionary<string, double> variables, IDictionary<string, Func<double[], double>> functions)
        {
            InitializeVariables(variables);
            InitializeFunctions(functions);
        }

		public abstract void VisitLiteral(Model.LiteralExpression expression);

		public abstract void VisitUnary(Model.UnaryExpression expression);

		public abstract void VisitFunction(Model.FunctionExpression expression);

		public abstract void VisitBinary(Model.BinaryExpression expression);

	    public abstract void VisitVariable(Model.VariableExpression expression);

        // Result is an object rather than a dobule for support the compiler-visitor.
		public abstract object Result { get; }

        protected void InitializeVariables(IDictionary<string, double> variables)
        {
            if (variables == null || variables.Count == 0)
            {
                UserDefinedVariables = new Dictionary<string, double>();
                return;
            }
            UserDefinedVariables = new Dictionary<string, double>(variables);
        }

        protected void InitializeFunctions(IDictionary<string, Func<double[], double>> functions)
        {
            if (functions == null || functions.Count == 0)
            {
                UserDefinedFunctions = new Dictionary<string, Func<double[], double>>();
                return;
            }
            UserDefinedFunctions = new Dictionary<string, Func<double[], double>>(functions);
        }

        protected IDictionary<string, double> UserDefinedVariables { get; private set; }

        protected IDictionary<string, Func<double[], double>> UserDefinedFunctions { get; private set; } 

        public static ExpressionVisitor Create(IDictionary<string, double> variables, IDictionary<string, Func<double[], double>> functions)
        {
            return new EvaluatingExpressionVisitor(variables, functions);
        }

		public class EvaluatingExpressionVisitor : ExpressionVisitor
		{
            private EvaluatingExpressionVisitor() {}

            public EvaluatingExpressionVisitor(IDictionary<string, double> variables,
                                               IDictionary<string, Func<double[], double>> functions) : base(variables, functions)
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
						_result = _result * -1;
						break;
					default:
						throw new ExpressionException("Invalid unary operator type.");
				}
			}

			public override void VisitFunction(Model.FunctionExpression expression)
			{
			    var name = expression.Name;
				var argsList = new List<double>(expression.Arguments.Count);
				expression.Arguments.ForEach(arg => {
					arg.Accept(this);
					argsList.Add(_result);
				});
				var args = argsList.ToArray();
			    if (Kernel.BuiltIn.IsBuiltInFunction(name))
			    {
			        _result = Kernel.BuiltIn.Function(expression.Name, args);
			    }
			    else
			    {
			        _result = UserDefinedFunctions[name](args);
			    }
			}

			public override void VisitBinary(Model.BinaryExpression expression)
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
	                    _result = left() - right();
						break;
	                case Model.OperatorType.Multiply:
	                    _result = left() * right();
						break;
	                case Model.OperatorType.Divide:
						_result = left() / right();
						break;
                    case Model.OperatorType.Modulo:
	                    _result = left() % right();
                        break;
	                case Model.OperatorType.Exponent:
	                    _result =  Math.Pow(left(), right());
						break;
					default:
						throw new ExpressionException("Invalid binary operator type.");
				}
			}

            public override void VisitVariable(Model.VariableExpression expression)
            {
                var name = expression.Name;
                if (Kernel.BuiltIn.IsBuiltInVariable(name))
                {
                    _result = Kernel.BuiltIn.Variable(name);
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

			private double _result;
		}
	}
}