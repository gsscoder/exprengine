using System;
using System.Globalization;
using System.Text;
using ExpressionEngine;

/// <summary>
/// Reads characters from <see cref="Tokenizer"/> and returns instances
/// of <see cref="ExpressionEngine.Internal.Token"/> derived types.
/// </summary>
sealed class Lexer : IDisposable
{
    Lexer() {}

    public Lexer(Tokenizer tokenizer)
    {
        _tokenizer = tokenizer;
        _buffer = new StringBuilder(64);
        _escapeBuffer = new StringBuilder(3);
        _next = LexToken();
    }

    public Token NextToken()
    {
        _current = _next;
        _next = LexToken();
        return _current;
    }

    public Token PeekToken() => _next;

    Token LexToken()
    {
        SkipWhiteSpace();

        var c = _tokenizer.NextChar();

        switch (c) {
            // Punctuators
            case '(': return new PunctuatorToken(TokenType.LeftParenthesis);
            case ')': return new PunctuatorToken(TokenType.RightParenthesis);
            case '+': return new PunctuatorToken(TokenType.Plus);
            case '-': return new PunctuatorToken(TokenType.Minus);
            case '*': return new PunctuatorToken(TokenType.Multiply);
            case '/': return new PunctuatorToken(TokenType.Divide);
            case '%': return new PunctuatorToken(TokenType.Modulo);
            case ',': return new PunctuatorToken(TokenType.Comma);
            case '=':
                if (_tokenizer.PeekChar() == '=') {
                    _tokenizer.NextChar();
                    return new PunctuatorToken(TokenType.Equality);
                }
                throw new EvaluatorException(_tokenizer.Line, "Unexpected token '='.");
            case '!':
                if (_tokenizer.PeekChar() == '=') {
                    _tokenizer.NextChar();
                    return new PunctuatorToken(TokenType.Inequality);
                }
                throw new EvaluatorException(_tokenizer.Line, "Unexpected token '!'.");
            case '<':
                if (_tokenizer.PeekChar() == '=') {
                    _tokenizer.NextChar();
                    return new PunctuatorToken(TokenType.LessThanOrEqual);
                }
                return new PunctuatorToken(TokenType.LessThan);
            case '>':
                if (_tokenizer.PeekChar() == '=') {
                    _tokenizer.NextChar();
                    return new PunctuatorToken(TokenType.GreaterThanOrEqual);
                }
                return new PunctuatorToken(TokenType.GreaterThan);
            // String literals
            case '"':
                _buffer.Length = 0;
                while (true) {
                    c = _tokenizer.NextChar();
                    if (c == '"') {
                        break;
                    }
                    if (c == '\0') { throw new EvaluatorException(_tokenizer.Column, "Unexpected end of input in string literal."); }
                    if (c == '\\') {
                        c = _tokenizer.NextChar();
                        switch (c) {
                            case '\\': // back slash
                                _buffer.Append('\x5C');
                                break;
                            case '"': // double quote
                                _buffer.Append('\x22');
                                break;
                            case 'n': // line feed
                                _buffer.Append('\x0A');
                                break;
                            case 'r': // carriage return
                                _buffer.Append('\x0D');
                                break;
                            case 't': // horizontal tab
                                _buffer.Append('\x09');
                                break;
                            case '0':
                            case '1':
                            case '2':
                                _buffer.Append(ScanDecimalEscapeSequence(c));
                                break;
                            default:
                                throw new EvaluatorException(_tokenizer.Column, "Invalid escape sequence.");
                        }
                    }
                    else {
                        _buffer.Append(c);
                    }
                }
                return new LiteralToken(_buffer.ToString());
            // Numeric literals
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                var seenDot = (c == '.');
                _buffer.Length = 0;
                _buffer.Append(c);
                while (true) {
                    c = _tokenizer.PeekChar();
                    if (c == '.') {
                        if (seenDot) { throw new EvaluatorException(_tokenizer.Column, "Bad numeric literal."); }
                        seenDot = true;
                    }
                    else if (c == 'e' || c == 'E') {
                        _buffer.Append(c);
                        _tokenizer.NextChar();
                        c = _tokenizer.PeekChar();
                        if (c == '+' || c == '-' || IsDigit(c)) {
                            _buffer.Append(c);
                            _tokenizer.NextChar();
                            while (true) {
                                c = _tokenizer.PeekChar();
                                if (!IsDigit(c) || c == '\0') {
                                    break;
                                }
                                _buffer.Append(c);
                                _tokenizer.NextChar();
                            }
                            return new LiteralToken(ParseNumber(_buffer, seenDot, true));
                        }
                        else {
                            throw new EvaluatorException(_tokenizer.Line, "Invalid character after numeric literal exponent.");
                        }
                    }
                    else if (!IsDigit(c) || c == '\0') {
                        break;
                    }
                    _buffer.Append(c);
                    _tokenizer.NextChar();
                }
                return new LiteralToken(ParseNumber(_buffer, seenDot, false));
            case '.':
                if (IsDigit(_tokenizer.PeekChar())) {
                    goto case '9';
                }
                throw new EvaluatorException(_tokenizer.Column, "Invalid numeric literal.");
            // Line terminators
            case '\xD':
            case '\xA':
            case '\x2028':
            case '\x2029':
                throw new EvaluatorException(_tokenizer.Column, "Line terminator is not allowed.");
            // EOF
            case '\0':
                return null;
            // Identifier, boolean literal or invalid token
            default:
                if (IsIdentifierChar(c)) {
                    _buffer.Length = 0;
                    _buffer.Append(c);
                    while (true) {
                        c = _tokenizer.PeekChar();
                        if (!IsIdentifierChar(c) || c == '\0') {
                            break;
                        }
                        _buffer.Append(c);
                        _tokenizer.NextChar();
                    }
                    var buf = _buffer.ToString();
                    if (string.CompareOrdinal(buf, "true") == 0) {
                        return new LiteralToken(true);
                    }
                    if (string.CompareOrdinal(buf, "false") == 0) {
                        return new LiteralToken(false);
                    }
                    return new IdentifierToken(buf);
                }
                throw new EvaluatorException(_tokenizer.Column, string.Format(CultureInfo.InvariantCulture, "Unexpected character '{0}'.", c));
        }
    }

    public static bool IsWhiteSpace(char c)
    {
        switch (c) {
            //// Line separators
            //case '\xD':
            //case '\xA':
            //case '\x2028':
            //case '\x2029':

            // Regular
            case '\f':
            case '\v':
            case ' ':
            case '\t':
                return true;

            // Unicode
            default:
                return (c > 127 && char.IsWhiteSpace(c));
        }
    }

    public bool IsEof() => _next == null;

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    public int Column { get { return _tokenizer.Column; } }

    void Dispose(bool disposing)
    {
        if (_disposed) {
            return;
        }
        if (disposing) {
            if (_tokenizer != null)
            {
                _tokenizer.Dispose();
            }
            _disposed = true;
        }
    }

    void SkipWhiteSpace()
    {
        while (IsWhiteSpace(_tokenizer.PeekChar())) {
            _tokenizer.NextChar();
        }
    }

    static bool IsDigit(char ch)
    {
        return (ch >= '0') && (ch <= '9');
    }

    static bool IsIdentifierChar(char ch)
    {
        return char.IsLetterOrDigit(ch) || (ch == '_');
    }

    static double ParseNumber(StringBuilder buf, bool seenDot, bool seenExp)
    {
        var styles = NumberStyles.Number;
        if (seenDot)
        {
            styles = NumberStyles.AllowDecimalPoint;
            if (seenExp)
            {
                styles |= NumberStyles.AllowExponent;
            }
        }
        else
        {
            styles = seenExp ? NumberStyles.AllowExponent : NumberStyles.None;
        }
        return double.Parse(buf.ToString(), styles, NumberFormatInfo);
    }

    char ScanDecimalEscapeSequence(char firstChar)
    {
        _escapeBuffer.Length = 0;
        _escapeBuffer.Append(new string(firstChar, 1));
        for (var i = 0; i < 2; i++)
        {
            var c = _tokenizer.PeekChar();
            if (!(c >= '0' && c <= '9'))
            {
                throw new EvaluatorException(_tokenizer.Line, "Invalid decimal escape sequence.");
            }
            _escapeBuffer.Append(c);
            _tokenizer.NextChar();
        }
        byte e;
        if (byte.TryParse(_escapeBuffer.ToString(), out e))
        {
            return Encoding.ASCII.GetChars(new byte[] {e})[0];
        }
        throw new EvaluatorException(_tokenizer.Line, "Invalid decimal escape sequence.");
    }

    ~Lexer()
    {
        Dispose(false);
    }

    static readonly NumberFormatInfo NumberFormatInfo = CultureInfo.InvariantCulture.NumberFormat;
    bool _disposed;
    Token _current;
    Token _next;
    readonly StringBuilder _escapeBuffer;
    readonly StringBuilder _buffer;
    readonly Tokenizer _tokenizer;
}