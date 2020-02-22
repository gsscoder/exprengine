sealed class SyntaxTree
{
    private SyntaxTree() {}

    internal SyntaxTree(Expression root)//, int userVariables, int userFunctions)
    {
        Root = root;
        //HasUserDefinedVariables = userVariables > 0;
        //HasUserDefinedFunctions = userFunctions > 0;
        //HasUserDefinedNames = HasUserDefinedVariables || HasUserDefinedFunctions;
    }

    /// <summary>
    /// Convenience method for building an AST from a string, used internally.
    /// </summary>
    public static SyntaxTree ParseString(string value)
    {
        using (var scanner = new Lexer(Tokenizer.OfString(value)))
        {
            return new Parser(scanner).Parse();
        }
    }

    public Expression Root { get; private set; }

    //public bool HasUserDefinedNames { get; private set; }

    //public bool HasUserDefinedVariables { get; private set; }

    //public bool HasUserDefinedFunctions { get; private set; }
}