using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ExpressionEngine.Internal;

namespace ExpressionEngine.Core
{
    static class BuiltInService
    {
        #region ParameterInfo support struct
        private struct ParameterInfo
        {
            private ParameterInfo(byte min, byte max)
            {
                _min = min;
                _max = max;
            }
            public static ParameterInfo OneParameter() { return new ParameterInfo(1, 1); }
            public static ParameterInfo TwoParameter() { return new ParameterInfo(2, 2); }
            public static ParameterInfo OneTwoParameter() { return new ParameterInfo(1, 2); }
            public bool Match(int length)
            {
                if (_min == 1 && _max == 1) return length == 1;
                if (_min == 1 && _max == 2) return length == 1 || length == 2;
                throw new InvalidOperationException(); // Unreachable code
            }
            public override string ToString()
            {
                if (_min == 1 && _max == 1) { return "one parameter"; }
                if (_min == 2 && _max == 2) { return "two paramters"; }
                if (_min == 1 && _max == 2) { return "one or two parameters"; }
                throw new InvalidOperationException(); // Unreachable code
            }
            private readonly byte _min;
            private readonly byte _max;
        }
        #endregion

        public static object ExecuteBuiltInFunction(string name, object[] args)
        {
            var param = _funcsLookup[name];
            if (!param.Match(args.Length))
            {
                throw new ExpressionException(string.Format(CultureInfo.InvariantCulture, "Function '{0}' requires only {1}.", name, param.ToString()));
            }
            var typed = Array.ConvertAll(args, TypeService.ToReal);
            if (string.CompareOrdinal("log", name) == 0)
            {
                if (args.Length == 1) { return Math.Log(typed[0]); }
                else if (args.Length == 2) { return Math.Log(typed[0], typed[1]); }
            }
            if (string.CompareOrdinal("abs", name) == 0) { return Math.Abs(typed[0]); }
            if (string.CompareOrdinal("asin", name) == 0) { return Math.Asin(typed[0]); }
            if (string.CompareOrdinal("sin", name) == 0) { return Math.Sin(typed[0]); }
            if (string.CompareOrdinal("sinh", name) == 0) { return Math.Sinh(typed[0]); }
            if (string.CompareOrdinal("acos", name) == 0) { return Math.Acos(typed[0]); }
            if (string.CompareOrdinal("cos", name) == 0) { return Math.Cos(typed[0]); }
            if (string.CompareOrdinal("cosh", name) == 0) { return Math.Cosh(typed[0]); }
            if (string.CompareOrdinal("sqrt", name) == 0) { return Math.Sqrt(typed[0]); }
            if (string.CompareOrdinal("atan", name) == 0) { return Math.Atan(typed[0]); }
            if (string.CompareOrdinal("tan", name) == 0) { return Math.Tan(typed[0]); }
            if (string.CompareOrdinal("tanh", name) == 0) { return Math.Tanh(typed[0]); }

            throw new InvalidOperationException(); // Unreachable code
        }

        public static object GetBuiltInVariable(string name)
        {
            return _varsLookup[name];
        }

        public static bool IsBuiltInFunction(string name)
        {
            return _funcsLookup.ContainsKey(name);
        }

        public static bool IsBuiltInVariable(string name)
        {
            return _varsLookup.ContainsKey(name);
        }

        private static readonly Dictionary<string, ParameterInfo> _funcsLookup = new Dictionary<string, ParameterInfo>()
                {
                    {"log", ParameterInfo.OneTwoParameter()},
                    {"abs", ParameterInfo.OneParameter()},
                    {"asin", ParameterInfo.OneParameter()},
                    {"sin", ParameterInfo.OneParameter()},
                    {"sinh", ParameterInfo.OneParameter()},
                    {"acos", ParameterInfo.OneParameter()},
                    {"cos", ParameterInfo.OneParameter()},
                    {"cosh", ParameterInfo.OneParameter()},
                    {"sqrt", ParameterInfo.OneParameter()},
                    {"atan", ParameterInfo.OneParameter()},
                    {"tan", ParameterInfo.OneParameter()},
                    {"tanh", ParameterInfo.OneParameter()}
                };
        private static readonly Dictionary<string, double> _varsLookup = new Dictionary<string, double>()
                {
                    {"e", Math.E},
                    {"pi", Math.PI}
                };
    }
}