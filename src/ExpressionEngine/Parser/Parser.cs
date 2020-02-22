using System;
using System.Linq;
using ExpressionEngine;

sealed class Parser : IDisposable
{
    public Parser(Lexer scanner)
    {
        _scanner = scanner;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing) {
            if (_scanner != null) _scanner.Dispose();
            _disposed = true;
        }
    }

    public SyntaxTree Parse()
    {
        Expression root = ParseExpression();
        if (_brackets != 0) {
            throw new EvaluatorException(_scanner.Column, "Syntax error, odd number of brackets.");
        }
        return new SyntaxTree(root);
    }

    Expression ParseExpression(bool insideFunc = false)
    {
        if (!insideFunc) Expect(InitialGroup);
        else Ensure(InitialGroupWithComma);
        Expression expr = ParseAdditiveBinary();
        return expr;
    }

    Expression ParseAdditiveBinary()
    {
        Expression expr = ParseMultiplicativeBinary();
        while (!_scanner.IsEof() && (_current.Type == TokenType.Plus ||
                                     _current.Type == TokenType.Minus)) {
            var binaryAddSub = new BinaryExpression
                {
                    Operator = ((PunctuatorToken)_current).ToOperatorType(),
                    Left = expr
                };
            Expect(MiddleGroupAdditiveRelational);
            binaryAddSub.Right = ParseMultiplicativeBinary();
            expr = binaryAddSub;
        }
        return expr;
    }

    Expression ParseMultiplicativeBinary()
    {
        Expression expr = ParseRelationalBinary();
        while (!_scanner.IsEof() && (_current.Type == TokenType.Multiply ||
                                     _current.Type == TokenType.Divide ||
                                     _current.Type == TokenType.Modulo)) {
            var binaryMulDiv = new BinaryExpression
                {
                    Operator = ((PunctuatorToken)_current).ToOperatorType(),
                    Left = expr
                };
            Expect(MiddleGroupMultiplicative);
            binaryMulDiv.Right = ParseRelationalBinary();
            expr = binaryMulDiv;
        }
        return expr;
    }

    Expression ParseRelationalBinary()
    {
        Expression expr = ParseIdentifier();
        while (!_scanner.IsEof() && (_current.Type == TokenType.Equality ||
                                     _current.Type == TokenType.Inequality ||
                                     _current.Type == TokenType.LessThan ||
                                     _current.Type == TokenType.GreaterThan ||
                                     _current.Type == TokenType.LessThanOrEqual ||
                                     _current.Type == TokenType.GreaterThanOrEqual)) {
            var binaryExp = new BinaryExpression
                {
                    Operator = ((PunctuatorToken) _current).ToOperatorType(),
                    Left = expr
                };
            Expect(MiddleGroupAdditiveRelational);
            binaryExp.Right = ParseIdentifier();
            expr = binaryExp;
        }
        return expr;
    }

    Expression ParseIdentifier()
    {
        if (_current == null) throw new EvaluatorException(_scanner.Column, "Expected function or expression.");
        if (_current.Type != TokenType.Identifier) return ParseUnary();

        if (_scanner.PeekToken() == null || (_scanner.PeekToken() != null &&
            _scanner.PeekToken().Type != TokenType.LeftParenthesis)) {
            // variable
            var varExpr = new VariableExpression(_current.Text);
            if (_scanner.PeekToken() != null) Expect(MiddleGroupIdentifier);
            else Consume(); // Reach eof
            return varExpr;
        }

        var expr = new FunctionCallExpression(_current.Text);
        Expect(TokenType.LeftParenthesis);
        while (!_scanner.IsEof()) {
            Consume();
            if (_current.Type == TokenType.RightParenthesis) {
                Consume();
                break;
            }
            expr.Arguments.Add(ParseExpression(true));
            if (_current == null) break;
            if (_current.Type == TokenType.Comma) continue;
            else if (_current.Type == TokenType.RightParenthesis) {
                Consume();
                break;
            }
            else {
                throw new EvaluatorException(_scanner.Column, "Expected comma or close bracket.");
            }
        }
        return expr;
    }

    Expression ParseUnary()
    {
        if (_current == null) throw new EvaluatorException(_scanner.Column,
            "Expected unary operator, literal or open bracket."); 

        if (_current.Type == TokenType.Literal) {
            var literal = ParseLiteral();
            Consume();
            return literal;
        }

        var unary = new UnaryExpression();
        if (_current.Type == TokenType.Minus) {
            unary.Operator = OperatorType.UnaryMinus;
            Expect(MiddleGroupUnary);
        }
        else if (_current.Type == TokenType.Plus) {
            unary.Operator = OperatorType.UnaryPlus;
            Expect(MiddleGroupUnary);
        }

        if (_current.Type == TokenType.Literal) unary.Value = ParseLiteral();
        else if (_current.Type == TokenType.LeftParenthesis) unary.Value = ParseExpression();
        else if (_current.Type == TokenType.Identifier) unary.Value = ParseIdentifier();
        else throw new EvaluatorException(_scanner.Column, "Expected literal or open bracket.");
        
        Consume();
        return unary;
    }

    LiteralExpression ParseLiteral()
    {
        var literal = new LiteralExpression(((LiteralToken)_current).Value);
        if (_scanner.PeekToken() != null && _scanner.PeekToken().Type == TokenType.Literal) {
            throw new EvaluatorException(_scanner.Column, "Expected expression.");
        }
        return literal;
    }

    #region Token Stream Controllers
    void Ensure(TokenType[] types, bool identifierAllowed = true)
    {
        if (_current == null) {
            throw new EvaluatorException(_scanner.Column,
                string.Format("Unexpected end of input, instead of token(s): {0}{1}.", identifierAllowed ? "'IDENT', " : "", Token.StringOf(types)));
        }
        if (!((identifierAllowed && _current.Type == TokenType.Identifier) ||
            types.Contains(_current.Type))) {
            throw new EvaluatorException(_scanner.Column,
                string.Format("Syntax error, expected token(s): 'IDENT', {0}; but found '{1}'.", Token.StringOf(types), Token.StringOf(_current)));
        }
    }

    void Expect(TokenType[] types, bool identifierAllowed = true)
    {
        var next = _scanner.PeekToken();
        if (next == null) {
            throw new EvaluatorException(_scanner.Column,
                string.Format("Unexpected end of input, instead of token(s): {0}{1}.", identifierAllowed ? "'IDENT', " : "" ,Token.StringOf(types)));
        }
        if ((identifierAllowed && next.Type == TokenType.Identifier) ||
            types.Contains(next.Type)) {
            Consume();
        }
        else {
            throw new EvaluatorException(_scanner.Column,
                string.Format("Syntax error, expected token(s): {0}{1}; but found '{2}'.", identifierAllowed ? "'IDENT', " : "", Token.StringOf(types), Token.StringOf(next)));
        }
    }

    void Expect(TokenType type)
    {
        var next = _scanner.PeekToken();
        if (next == null) {
            throw new EvaluatorException(_scanner.Column,
                string.Format("Unexpected end of input, instead of token(s): 'LITERAL', '{0}'.", Token.StringOf(type)));
        }
        if (next.Type == type) {
            Consume();
        }
        else {
            throw new EvaluatorException(_scanner.Column,
                string.Format("Syntax error, expected token(s) 'LITERAL', '{0}'; but found '{1}'.", Token.StringOf(type), Token.StringOf(next)));
        }
    }

    void Consume()
    {
        _current = _scanner.NextToken();
        if (_current == null) { return; }

        if (_current.Type == TokenType.LeftParenthesis) { _brackets++; }
        else if (_current.Type == TokenType.RightParenthesis) { _brackets--; }
    }
    #endregion

    ~Parser()
    {
        Dispose(false);
    }

    #region Token Groups
    static readonly TokenType[] InitialGroup = { TokenType.Literal, TokenType.Plus, TokenType.Minus, TokenType.LeftParenthesis, TokenType.Equality, TokenType.Inequality, TokenType.LessThan, TokenType.GreaterThan, TokenType.LessThanOrEqual, TokenType.GreaterThanOrEqual }; // + ident
    static readonly TokenType[] InitialGroupWithComma = { TokenType.Literal, TokenType.Comma, TokenType.Plus, TokenType.Minus, TokenType.LeftParenthesis, TokenType.Equality, TokenType.Inequality, TokenType.LessThan, TokenType.GreaterThan, TokenType.LessThanOrEqual, TokenType.GreaterThanOrEqual }; // + ident
    static readonly TokenType[] MiddleGroupAdditiveRelational = { TokenType.Literal, TokenType.Plus, TokenType.Minus, TokenType.LeftParenthesis, TokenType.RightParenthesis }; // + ident
    static readonly TokenType[] MiddleGroupMultiplicative = { TokenType.Literal, TokenType.Plus, TokenType.Minus, TokenType.LeftParenthesis }; // + ident
    static readonly TokenType[] MiddleGroupIdentifier = { TokenType.Literal, TokenType.Plus, TokenType.Minus, TokenType.Multiply, TokenType.Divide, TokenType.Equality, TokenType.Inequality, TokenType.LessThan, TokenType.GreaterThan, TokenType.LessThanOrEqual, TokenType.GreaterThanOrEqual, TokenType.RightParenthesis, TokenType.Comma };
    static readonly TokenType[] MiddleGroupUnary = { TokenType.Literal, TokenType.LeftParenthesis }; // + ident
    #endregion

    bool _disposed;
    int _brackets;
    Token _current;
    readonly Lexer _scanner;
}