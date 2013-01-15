using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionEngine.Internal
{
    enum TokenType : byte
    {
        // Punctuators
        LeftParenthesis,
        RightParenthesis,
        Comma,
        Plus,
        Minus,
        Multiply,
        Divide,
        Modulo,
        Exponent,
        // Others
        Literal,
        Identifier
    }
}
