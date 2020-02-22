namespace ExpressionEngine
{
    /// <summary>
    /// Provides methods for contextless expressions evaluation, using a friendly readable syntax.
    /// </summary>
    public static class Evaluate
    {
        /// <summary>
        /// Evalutates an expression supplied as a <see cref="System.String"/> without persistence of context.
        /// </summary>
        /// <param name="expression">A <see cref="System.String"/> expression to evaluate.</param>
        /// <returns>An <see cref="System.Object"/> that represents the result of evaluation.</returns>
        public static object It(string expression)
        {
            var tree = SyntaxTree.ParseString(expression);
            var global = new Scope();
            BuiltIn.IntializeScope(global);
            var visitor = Visitor.Create(global);
            tree.Root.Accept(visitor);
            return visitor.Result;
        }

        /// <summary>
        /// Evalutates an expression supplied as a <see cref="System.String"/> without persistence of context.
        /// </summary>
        /// <typeparam name="T">The type specified for result.</typeparam>
        /// <param name="expression">A <see cref="System.String"/> expression to evaluate.</param>
        /// <returns>A <typeparamref name="T"/> that represents the result of evaluation.</returns>
        public static T As<T>(string expression)
        {
            return (T) It(expression);
        }
    }
}