using System.Collections.Generic;

class Scope
{
    public Scope()
    {
        _dispatch = new Dictionary<string, object>(48);
    }

    public object this[string name]
    {
        get
        {
            if (_dispatch.ContainsKey(name))
            {
                return _dispatch[name];
            }
            return null;
        }
        set
        {
            _dispatch[name] = value;
        }
    }

    private readonly IDictionary<string, object> _dispatch;
}