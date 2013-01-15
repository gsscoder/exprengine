using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ExpressionEngine.Primitives
{
    class Integer : Instance
    {
        public Integer(long value)
        {
            _value = value;
        }

        public override object Value
        {
            get { return _value; }
        }

        //public override string Type
        //{
        //    get { return Instance.TypeInteger; }
        //}
        public override PrimitiveType PrimitiveType
        {
            get { return PrimitiveType.Integer; }
        }

        public override double ToReal()
        {
            return _value;
        }

        public override long ToInteger()
        {
            return _value;
        }

        public override object ToObject()
        {
            return _value;
        }

        public override string ToString()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }

        private readonly long _value;
    }
}