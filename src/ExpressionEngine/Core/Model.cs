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

namespace ExpressionEngine.Core.Model
{
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
        protected Expression()
        {
        }

        public abstract PrimitiveType ResultType { get; }

        public abstract void Accept(ExpressionVisitor visitor);
    }

    sealed class LiteralExpression : Expression
    {
        private LiteralExpression() { }

        public LiteralExpression(object value)
        {
            Value = value;
        }

        public object Value { get; private set; } // defined object to open to non-number computations

        public override PrimitiveType ResultType
        {
            get { return Kernel.Instance.Primitives.ToPrimitiveType(Value.GetType()); }
        }

        public override void Accept(ExpressionVisitor visitor)
        {
            visitor.VisitLiteral(this);
        }
    }

    abstract class OperatorExpression : Expression
    {
        protected OperatorExpression()
        {
        }

        //protected OperatorExpression(OperatorType @operator)
        //{
        //    Operator = @operator;
        //}

        public OperatorType Operator { get; set; }
    }

    sealed class UnaryExpression : OperatorExpression
    {
        //public UnaryExpression(OperatorType @operator)
        //    : base(@operator)
        //{
        //}

        public Model.Expression Value { get; set; }

        public override PrimitiveType ResultType
        {
            get { return Value.ResultType; }
        }

        public override void Accept(ExpressionVisitor visitor)
        {
            visitor.VisitUnary(this);
        }
    }

    sealed class BinaryExpression : OperatorExpression
    {
        //public BinaryExpression(OperatorType @operator)
        //    : base(@operator)
        //{
        //}

        public Model.Expression Left { get; set; }

        public Model.Expression Right { get; set; }

        public override PrimitiveType ResultType
        {
            get
            {
                if (Left.ResultType == Right.ResultType)
                {
                    return Left.ResultType;
                }
                return PrimitiveType.Real;
            }
        }

        public override void Accept(ExpressionVisitor visitor)
        {
            visitor.VisitBinary(this);
        }
    }

    abstract class NameExpression : Expression
    {
        public string Name { get; protected set; }
    }

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