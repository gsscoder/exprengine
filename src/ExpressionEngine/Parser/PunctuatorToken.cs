using System;
using System.Collections.Generic;
using ExpressionEngine.Internal.Ast;

namespace ExpressionEngine.Internal
{
    sealed class PunctuatorToken : Token
    {
        public PunctuatorToken(TokenType type)
            : base(null, type)
        {
        }

        //public static Token ValueOf(string text)
        //{
        //    PunctuatorToken token;
        //    return Lookup.TryGetValue(text, out token) ? token : null;
        //}

        public OperatorType ToOperatorType()
        {
            switch (Type)
            {
                case TokenType.Plus:
                    return OperatorType.Add;
                case TokenType.Minus:
                    return OperatorType.Subtract;
                case TokenType.Multiply:
                    return OperatorType.Multiply;
                case TokenType.Divide:
                    return OperatorType.Divide;
                case TokenType.Modulo:
                    return OperatorType.Modulo;
                case TokenType.Equality:
                    return OperatorType.Equality;
                case TokenType.Inequality:
                    return OperatorType.Inequality;
                case TokenType.LessThan:
                    return OperatorType.LessThan;
                case TokenType.GreaterThan:
                    return OperatorType.GreaterThan;
                case TokenType.LessThanOrEqual:
                    return OperatorType.LessThanOrEqual;
                case TokenType.GreaterThanOrEqual:
                    return OperatorType.GreaterThanOrEqual;
            }
            throw new EvaluatorException("Unexpected punctuator.");
        }

        public override string Text
        {
            get {
                return StringOf(Type);
            }
        }
    }
}