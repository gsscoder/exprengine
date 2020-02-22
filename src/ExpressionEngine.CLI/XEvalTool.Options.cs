#region License
//
// Expression Engine Library: XEvalTool.Options.cs
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
using System.IO;
using CommandLine;
using CommandLine.Text;
#endregion

namespace ExpressionEngine
{
    partial class XEvalTool
    {
        private class Options : CommandLineOptionsBase
        {
            [Option("f", "filename", MutuallyExclusiveSet = SetModality,
                HelpText = "Input text file to be evaluated.")]
            public string FileName { get; set; }

            [Option("i", "interactive", MutuallyExclusiveSet = SetModality,
                HelpText = "Run in interactive mode.")]
            public bool Interactive { get; set; }

            [Option("q", "quiet",
                HelpText = "Print only the evaluation results.")]
            public bool Quiet { get; set; }

            [Option("v", "version",
                HelpText = "Print version information and exit.")]
            public bool Version { get; set; }

            [ValueList(typeof(List<string>), MaximumElements = 1)]
            public List<string> Values { get; set; }

            public string Expression { get { return (Values != null && Values.Count > 0) ? Values[0] : null; } }

            public bool FileMode { get { return !Interactive && !string.IsNullOrEmpty(FileName); } }

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this,
                    (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }

            public bool Validate()
            {
                var fileName = FileName;
                // Filename modality
                if (!Interactive && !string.IsNullOrEmpty(fileName))
                {
                    if (!File.Exists(fileName))
                    {
                        Console.Error.WriteLine("File '{0}' doesn't exits.", fileName);
                        return false;
                    }
                    if ((new FileInfo(fileName)).Length == 0)
                    {
                        Console.Error.WriteLine("File '{0}' is empty.", fileName);
                        return false;
                    }
                }
                // Both modalities
                if (Interactive || FileMode)
                {
                    if (!string.IsNullOrEmpty(Expression))
                    {
                        Console.Error.WriteLine("Expression as argument is allowed only when interactive mode is off and filename is not specified.");
                        return false;
                    }
                }
                return true;
            }

            private const string SetModality = "modality";
        }
    }
}