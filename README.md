Expression Engine Library 1.0.1.9 Alfa.
===
This project is a simple mathematical expression parser written in C#. It's the refactoring of an old project and uses an AST-evaluation algorithm.
It was written primarily for fun and currently doesn't support variables; but it will in near future, along with other features.

News:
---
  - Added support for functions; for now pow(x,y) and sqrt(x).
  - Expression class is now really immutable.
  - Public API / internal types refactored (this time should be very close to first beta).
  - Infix grammar strengthened at parser level.
  - Shunting-Yard algorithm replaced by AST evaluation.
  - Better support for unary operators.
  - Solution now opens in MonoDevelop without problems.
  - All line endings converted to unix form.

To build:
---
MonoDevelop or Visual Studio.

At glance:
---
```csharp
var result = Expression.Create("-3 * 0.31 / ((19 + sqrt(1000)) - .7) + pow(5 * 2, 2)").Value;
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
