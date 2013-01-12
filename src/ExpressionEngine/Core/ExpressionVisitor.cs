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
using ExpressionEngine.Internal.Model;
#endregion

namespace ExpressionEngine.Internal
{
    abstract class ExpressionVisitor
    {
        private ExpressionVisitor() {}

        protected ExpressionVisitor(IDictionary<string, object> variables, IDictionary<string, Func<object[], object>> functions)
        {
            InitializeVariables(variables);
            InitializeFunctions(functions);
        }

        public abstract void VisitLiteral(LiteralExpression expression);

        public abstract void VisitUnary(UnaryExpression expression);

        public abstract void VisitFunction(FunctionExpression expression);

        public abstract void VisitBinary(BinaryExpression expression);

        public abstract void VisitVariable(VariableExpression expression);

        public abstract object Result { get; }

        protected void InitializeVariables(IDictionary<string, object> variables)
        {
            if (variables == null || variables.Count == 0)
            {
                UserDefinedVariables = new Dictionary<string, object>();
                return;
            }
            UserDefinedVariables = new Dictionary<string, object>(variables);
        }

        protected void InitializeFunctions(IDictionary<string, Func<object[], object>> functions)
        {
            if (functions == null || functions.Count == 0)
            {
                UserDefinedFunctions = new Dictionary<string, Func<object[], object>>();
                return;
            }
            UserDefinedFunctions = new Dictionary<string, Func<object[], object>>(functions);
        }

        protected IDictionary<string, object> UserDefinedVariables { get; private set; }

        protected IDictionary<string, Func<object[], object>> UserDefinedFunctions { get; private set; } 

        public static ExpressionVisitor Create(IDictionary<string, object> variables, IDictionary<string, Func<object[], object>> functions)
        {
            return new EvaluatingExpressionVisitor(variables, functions);
        }
    }
}