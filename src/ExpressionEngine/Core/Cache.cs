using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressionEngine.Core
{
    //[Serializable]
    //class WeakReference<T> : WeakReference
    //    where T : class
    //{
    //    public WeakReference(T target)
    //        : base(target)
    //    {
    //    }

    //    public WeakReference(T target, bool trackResurrection)
    //        : base(target, trackResurrection)
    //    {
    //    }

    //    protected WeakReference(SerializationInfo info, StreamingContext context)
    //        : base(info, context)
    //    {
    //    }

    //    public new T Target
    //    {
    //        get { return (T) base.Target; }
    //        set { base.Target = value; }
    //    }

    //    public static implicit operator WeakReference<T>(T target)
    //    {
    //        if (target == null)
    //        {
    //            throw new ArgumentNullException("target");
    //        }
    //        return new WeakReference<T>(target);
    //    }

    //    public static implicit operator T(WeakReference<T> reference)
    //    {
    //        return reference != null ? reference.Target : null;
    //    }
    //}

    //interface ICache<TValue>
    //{
    //    TValue this[string key] { set; get; }
    //}

    //sealed class ObjectCache<TValue> : ICache<TValue>
    //    where TValue : class 
    //{
    //    public ObjectCache()
    //    {
    //        _cache = new Dictionary<string, WeakReference<TValue>>();
    //    }

    //    public TValue this[string key]
    //    {
    //        get
    //        {
    //            if (_cache.ContainsKey(key))
    //            {
    //                throw new ArgumentNullException("key");
    //            }
    //            return _cache[key].Target;
    //        }
    //        set
    //        {
    //            if (value == null)
    //            {
    //                throw new ArgumentNullException("value");
    //            }
    //            if (_cache.ContainsKey(key))
    //            {
    //                _cache[key] = new WeakReference<TValue>(value);
    //            }
    //            else
    //            {
    //                _cache.Add(key, new WeakReference<TValue>(value));
    //            }
    //        }
    //    }

    //    private readonly Dictionary<string, WeakReference<TValue>> _cache;
    //}

    sealed class ValueCache<TValue> //: ICache<TValue>
       where TValue : struct
    {
        public ValueCache()
        {
            _cache = new Dictionary<string, TValue>(32, StringComparer.Ordinal);
        }

        public TValue this[string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new ArgumentNullException("key");
                }
                if (!_cache.ContainsKey(key))
                {
                    throw new ArgumentException("key");
                }
                return _cache[key];
            }
            set
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new ArgumentNullException("key");
                }
                if (!_cache.ContainsKey(key))
                {
                    throw new ArgumentException("key");
                }
                _cache[key] = value;
            }
        }

        public void Add(string key, TValue value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            if (_cache.ContainsKey(key))
            {
                throw new ArgumentException("key");
            }
            _cache.Add(key, value);
        }

        public bool Contains(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            return _cache.ContainsKey(key);
        }

        private readonly Dictionary<string, TValue> _cache;
    }
}