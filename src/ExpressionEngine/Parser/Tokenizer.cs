using System;
using System.IO;
 
sealed class Tokenizer : IDisposable
{
    public Tokenizer(TextReader reader)
    {
        _reader = reader;
        _column = -1;
        _line = -1;
    }

    public static Tokenizer OfString(string value)
    {
        return new Tokenizer(new StringReader(value));
    }

    public int Column { get { return _column; } }

    public int Line { get { return _line; } }

    public char NextChar()
    {
        var c = _reader.Read();
        if (c == -1)
        {
            return '\0';
        }
        _column++;
        if (_line < 0 || c == '\xA' || c == '\xD' || c == '\x2028' || c== '\x2029')
        {
            // Increments line count after first read or line terminator
            _line++;
        }
        return (char) c;
    }

    public char PeekChar()
    {
        var c = _reader.Peek();
        return c != -1 ? (char) c : '\0';
    }

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }
        if (disposing)
        {
            if (_reader != null)
            {
                _reader.Dispose();
            }
            _disposed = true;
        }
    }

    ~Tokenizer()
    {
        Dispose(false);
    }

    private int _column;
    private int _line;
    private readonly TextReader _reader;
    private bool _disposed;
}