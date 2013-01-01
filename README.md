Expression Engine Library 1.0.2.15 Alfa.
===
This project is a simple mathematical expression parser written in C#. It's the refactoring of an old project and uses an AST-evaluation algorithm.
It was written primarily for fun and currently doesn't support user defined variables/functions; but it will in near future, along with other features.

News:
---
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
