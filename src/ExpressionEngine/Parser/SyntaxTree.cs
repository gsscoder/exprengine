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
using Model = ExpressionEngine.Internal.Model;
#endregion

namespace ExpressionEngine.Internal
{
    sealed class SyntaxTree
    {
        private SyntaxTree() {}

        internal SyntaxTree(Model.Expression root, int userVariables, int userFunctions)
        {
            Root = root;
            HasUserDefinedVariables = userVariables > 0;
            HasUserDefinedFunctions = userFunctions > 0;
            HasUserDefinedNames = HasUserDefinedVariables || HasUserDefinedFunctions;
        }

        /// <summary>
        /// Convenience method for building an AST from a string, used internally.
        /// </summary>
        public static SyntaxTree ParseString(string value)
        {
            using (var scanner = new Lexer(Text.OfString(value)))
            {
                return new Parser(scanner).Parse();
            }
        }

        public Model.Expression Root { get; private set; }

        public bool HasUserDefinedNames { get; private set; }

        public bool HasUserDefinedVariables { get; private set; }

        public bool HasUserDefinedFunctions { get; private set; }
    }
}