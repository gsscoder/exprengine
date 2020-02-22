using System;

namespace ExpressionEngine
{
    /// <summary>
    /// Provides methods for expressions evaluation.
    /// </summary>
    public sealed class Context
    {
        /// <summary>
        /// Creates an instance of <see cref="ExpressionEngine.Context"/>.
        /// </summary>
        public Context()
        {
            _global = new Scope();
            BuiltIn.IntializeScope(_global);
        }

        /// <summary>
        /// Evalutates an expression supplied as a <see cref="System.String"/>.
        /// </summary>
        /// <param name="expression">A <see cref="System.String"/> expression to evaluate.</param>
        /// <returns>An <see cref="System.Object"/> that represents the result of evaluation.</returns>
        public object Evaluate(string expression)
        {
            var tree = SyntaxTree.ParseString(expression);
            var visitor = Visitor.Create(_global);
            tree.Root.Accept(visitor);
            return visitor.Result;
        }

        /// <summary>
        /// Evalutates an expression supplied as a <see cref="System.String"/>.
        /// </summary>
        /// <typeparam name="T">The type specified for result.</typeparam>
        /// <param name="expression">A <see cref="System.String"/> expression to evaluate.</param>
        /// <returns>A <typeparamref name="T"/> that represents the result of evaluation.</returns>
        public T EvaluateAs<T>(string expression)
        {
            return (T) Evaluate(expression);
        }

        /// <summary>
        /// Define a function in context of global scope.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <param name="function">The lamda expression of the function.</param>
        /// <returns>An <see cref="ExpressionEngine.Context"/> instance with modified scope.</returns>
        public Context SetFunction(string name, Func<object[], object> function)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Can't define a function with null or empty name.", "name");
            if (function == null) throw new ArgumentNullException("name", "Can't define a function with null lamda.");

            _global[name] = new Function(name, function);
            return this;
        }

        /// <summary>
        /// Define a variable in context of global scope.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="variable">The value of the variable.</param>
        /// <returns>An <see cref="ExpressionEngine.Context"/> instance with modified scope.</returns>
        public Context SetVariable(string name, double variable)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Can't define a variable with null or empty name.", "name");

            _global[name] = variable;
            return this;
        }

        public Context SetObject(string name, object obj)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Can't define an object with null or empty name.", "name");

            _global[name] = obj;
            return this;
        }

        private readonly Scope _global;
    }
}