#region License
//
// Expression Engine Library: Kernel.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2012 Giacomo Stelluti Scala
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
using System.IO;
#endregion

namespace ExpressionEngine.Core
{
	static class Kernel
	{
		public static Model.Expression ParseString(string value)
		{
			using (var scanner = new Scanner(new StringReader(value)))
			{
				return new Parser(scanner).Parse();
			}
		}

        #region BuiltIns
        internal abstract class BuiltIn
        {
            //public class Pow : BuiltIn { public override double Execute(double[] args)
            //    {
            //        if (args.Length != 2) { throw new ExpressionException("Syntax error, pow() requires two arguments."); }
            //        return Math.Pow(args[0], args[1]);
            //    }
            //}
            public class Cos : BuiltIn { public override double Execute(double[] args)
                {
                    if (args.Length != 1) { throw new ExpressionException("Syntax error, cos() requires one arguments."); }
                    return Math.Cos(args[0]);
                }
            }
            public class Sin : BuiltIn { public override double Execute(double[] args)
                {
                    if (args.Length != 1) { throw new ExpressionException("Syntax error, sin() requires one arguments."); }
                    return Math.Sin(args[0]);
                }
            }
            public class Log : BuiltIn { public override double Execute(double[] args)
                {
                    if (args.Length < 1 && args.Length > 2) { throw new ExpressionException("Syntax error, log() requires one or two arguments."); }
                    return args.Length == 1 ? Math.Log(args[0]) : Math.Log(args[0], args[1]);
                }
            }
            public class Sqrt : BuiltIn { public override double Execute(double[] args)
                {
                    if (args.Length != 1) { throw new ExpressionException("Syntax error, sqrt() requires one argument."); }
                    return Math.Sqrt(args[0]);
                }
            }

            public static BuiltIn FromString(string name)
            {
                BuiltIn builtIn;
                if (Lookup.TryGetValue(name, out builtIn))
                {
                    return builtIn;
                }
                return null;
            }

            public abstract double Execute(double[] args);

            private static readonly Dictionary<string, BuiltIn> Lookup = new Dictionary<string, BuiltIn>()
	            {
                    //{"pow",  new Pow()},
                    {"log", new Log()},
                    {"sin", new Sin()},
                    {"cos", new Cos()},
	                {"sqrt", new Sqrt()}
	            };
        }
        #endregion
    }
}