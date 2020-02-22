#region License
//
// Expression Engine Library: AssemblyInfo.cs
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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Resources;
using CommandLine.Text;
#endregion

[assembly: NeutralResourcesLanguage("en-US")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyLicense(
    "This is free software. You may redistribute copies of it under the terms of",
    "the MIT License <http://www.opensource.org/licenses/mit-license.php>.")]
[assembly: AssemblyUsage(
    "Usage: xeval [EXPRESSION]",
    "       xeval --interactive",
    "       xeval -f [FILENAME]",
    "       cmdx | xeval")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
//[assembly: AssemblyCompany("")]
//[assembly: AssemblyTrademark("")]