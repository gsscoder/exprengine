#region License
//
// Expression Engine Library: XEvalTool.cs
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
using System.IO;
using System.Text;
using CommandLine;
using CommandLine.Text;
#endregion

namespace ExpressionEngine
{
    internal sealed partial class XEvalTool
    {
        private XEvalTool() {}

        static XEvalTool() {}

        public static XEvalTool Instance { get { return Singleton; } }

        public static void Main(string[] args)
        {
            XEvalTool.Instance.Run(args);
        }

        private void Run(string[] args)
        {
            var parser = new CommandLineParser(new CommandLineParserSettings(false, true, Console.Error));
            if (!parser.ParseArguments(args, _options))
            {
                Environment.Exit(Failure);
            }
            if (_options.Version)
            {
                PrintHeading();
                Console.WriteLine("Embeds Command Line Parser Library, Version 1.9.3.34 Stable.");
                Environment.Exit(Success);
            }
            if (!_options.Validate())
            {
                Console.Error.WriteLine("Try '{0} --help' for more information.", ThisAssembly.Name);
                Environment.Exit(Failure);
            }
            var expression = _options.Expression;
            if (!string.IsNullOrEmpty(expression))
            {
                if (!DoEvaluate(expression))
                {
                    Environment.Exit(Failure);
                }
                Environment.Exit(Success);
            }
            if (_options.Interactive)
            {
                if (!_options.Quiet)
                {
                    PrintHeading();
                    Console.WriteLine("For exit type `quit`.");
                }
                DoInteractiveMode();
                Environment.Exit(Success);
            }
            else if (_options.FileMode)
            {
                DoReadFile();
                Environment.Exit(Success);
            }
            // Default, read standard input quietly
            string line;
            while ((line = Console.ReadLine()) != null)
            {
                DoEvaluate(line);
            }
            Environment.Exit(Success);
        }

        private void DoInteractiveMode()
        {
            Console.Write(">> ");
            string line;
            while ((line = Console.ReadLine()) != null)
            {
                if (line.Equals("quit", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
                DoEvaluate(line);
                Console.Write(">> ");
            }
        }

        private void DoReadFile()
        {
            using (var reader = new StreamReader(_options.FileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    DoEvaluate(line);
                }
            }
        }

        private bool DoEvaluate(string expression)
        {
            var se = SanitizeInput(expression).Trim();
            if (se.Length == 0) { return false; }
            try
            {
                var result = _evaluator.Evaluate(se);
                Console.WriteLine(result);
                return true;
            }
            catch (EvaluatorException e)
            {
                Console.WriteLine(FormatException(se, e));
            }
            return false;
        }

        private string SanitizeInput(string value)
        {
            if (value.Length > 2 && value[0] == '"')
            {
                // maybe that last character is non-printable char like 32 (SUB|CTRL-Z),
                // used to mark end of file; e.g.: 'echo "1 + 3" | xeval'
                if (value[value.Length - 2] == '"')
                {
                    return value.Substring(1, value.Length - 3);
                }
                else if (value[value.Length - 1] == '"')
                {
                    return value.Substring(1, value.Length - 2);
                }
            }
            return value;
        }

        private void PrintHeading()
        {
            Console.WriteLine(_heading.ToString());
            Console.WriteLine(ThisAssembly.Copyright);
            Console.WriteLine("Embeds {0}, Version {1} {2}.", ThisLibrary.ProductName, ThisLibrary.Version, ThisLibrary.ReleaseType);
        }

        private string FormatException(string expression, EvaluatorException e)
        {
            var builder = new StringBuilder((expression.Length + e.Message.Length + 16) * 2);
            if (e.ColumnNumber >= 0)
            {
                builder.AppendLine(expression);
                if (e.ColumnNumber > 0 && e.ColumnNumber < expression.Length)
                {
                    builder.Append('-'.Repeat(e.ColumnNumber - 1));
                }
                else
                {
                    builder.Append('-'.Repeat(expression.Length));
                }
                builder.Append("^ ");
            }
            builder.Append(e.Message);
            return builder.ToString();
        }

        private readonly Options _options = new Options();
        private readonly HeadingInfo _heading = new HeadingInfo(ThisAssembly.Name,
            ThisAssembly.InformationalVersion + " " + ThisAssembly.ReleaseType);
        private readonly ExpressionEvaluator _evaluator = new ExpressionEvaluator();
        #region Exit Code Constants
        private const int Success = 0;
        private const int Failure = 1;
        //private const int FailureCritical = 2;
        #endregion
        private static readonly XEvalTool Singleton = new XEvalTool();
    }
}
