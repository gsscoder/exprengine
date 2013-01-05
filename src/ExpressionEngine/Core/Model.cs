#region License
//
// Expression Engine Library: Expressions.cs
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
using System.Collections.Generic;
using ExpressionEngine.Core;
#endregion

namespace ExpressionEngine.Model
{
    /*
     *  All abstract syntax tree nodes uses public fields instead of properties for two reasons:
     *   - Model.* types are part of private library interface not accessible to user code.
     *   - Field access is faster and those fields are primarly accessed inside loops.
     */

    sealed class Ast
    {
        private Ast() {}

        public Ast(Expression root, int userVariables, int userFunctions)
        {
            Root = root;
            HasUserDefinedVariables = userVariables > 0;
            HasUserDefinedFunctions = userFunctions > 0;
            HasUserDefinedNames = HasUserDefinedVariables || HasUserDefinedFunctions;
        }

        public Expression Root { get; private set; }

        public bool HasUserDefinedNames { get; private set; }

        public bool HasUserDefinedVariables { get; private set; }

        public bool HasUserDefinedFunctions { get; private set; }
    }

    enum OperatorType : byte
    {
        UnaryPlus,
        UnaryMinus,
        Exponent,
        Multiply,
        Divide,
        Modulo,
        Add,
        Subtract
    }

    abstract class Expression
    {
        public abstract void Accept(ExpressionVisitor visitor);
    }

    sealed class LiteralExpression : Expression
    {
        private LiteralExpression() { }

        public LiteralExpression(double value)
        {
            Value = value;
        }

        public double Value;

        public override void Accept(ExpressionVisitor visitor)
        {
            visitor.VisitLiteral(this);
        }
    }

    sealed class UnaryExpression : Expression
    {
        public OperatorType Operator;

        public Model.Expression Value;

        public override void Accept(ExpressionVisitor visitor)
        {
            visitor.VisitUnary(this);
        }
    }

    sealed class BinaryExpression : Expression
    {
        public OperatorType Operator;

        public Model.Expression Left;

        public Model.Expression Right;

        public override void Accept(ExpressionVisitor visitor)
        {
            visitor.VisitBinary(this);
        }
    }

    sealed class FunctionExpression : Expression
    {
        public FunctionExpression()
        {
            Arguments = new List<Expression>();
        }

        public string Name;

        public List<Expression> Arguments;

        public override void Accept(ExpressionVisitor visitor)
        {
            visitor.VisitFunction(this);
        }
    }

    class VariableExpression : Expression
    {
        public string Name;

        public override void Accept(ExpressionVisitor visitor)
        {
            visitor.VisitVariable(this);
        }
    }
}