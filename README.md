Expression Engine Library 1.0.3.3 Beta.
===
This project is a simple mathematical expression parser written in C#. It's the refactoring of an old project and uses an AST-evaluation algorithm. You can use built-in or user-defined [functions and variables](https://github.com/gsscoder/exprengine/blob/master/src/ExpressionEngine.Tests/MutableExpressionFixture.cs).
It was written primarily for fun and and as programming exercise; anyway if you anyone find it useful, please send feature requests or issues.

News:
---
  - Number parsing refactored in Core.Scanner class.
  - Public API should have reached its final shape.
  - Implemented user defined functions and variables.
  - Implemented modulo operator (%).
  - Parser performance improved.
  - Added support for built-in variables (pi->Math.PI, e->Math.E).
  - Added more functions (abs,atan,tan,tanh,asin,sinh,acos,cosh).
  - Evaluation logic separated from Model.* types by means of visitor pattern;
      this will allow a more clean implementation of expressions in-memory compiler.
  - Built-In function pow(x, y) replaced with caret operator x ^ y.
  - Currently supported Built-In functions: cos(x), sin(x), log(x[,y]), sqrt(x).
  - Expression class is now really immutable.
  - Public API / internal types refactored (this time should be very close to first beta).

To build:
---
MonoDevelop or Visual Studio.

At glance:
---
```csharp
var result = Expression.Create("-3 * 0.31 / ((19 + sqrt(1000)) - .7) + 5 * 2 ^ -log(1, pi)").Value;
```

Resources:
---
  - [CodePlex](http://exprengine.codeplex.com/)
  - [Wiki](https://github.com/gsscoder/exprengine/wiki)

Contacts:
---
Giacomo Stelluti Scala
  - gsscoder AT gmail DOT com
  - [Blog](http://gsscoder.blogspot.it)
  - [Twitter](http://twitter.com/gsscoder)
