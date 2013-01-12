Expression Engine Library 1.0.4.9 Beta.
===
This project is a simple mathematical expression parser written in C#. It's the refactoring of an old project and uses an AST-evaluation algorithm. You can use built-in or user-defined [functions and variables](https://github.com/gsscoder/exprengine/blob/master/src/ExpressionEngine.Tests/MutableExpressionFixture.cs).
It was written primarily for fun and and as programming exercise; anyway if you anyone find it useful, please send feature requests or issues; but please take not that *this is a work in progress*.

News:
---
  - Sources splitted, lexer refactored.
  - Heavy internal refactoring (with the main purpose of enhance expression type-system and open doors to non-numeric computations).
  - AST design was enhanced toward maintenability (and for a possible public exposure to increase library uses).
  - Added cache for immutable expressions.
  - Created synchronized wrapper for mutable expressions.
  - Public API should have reached its final shape.
  - Implemented user defined functions and variables.
  - Parser performance improved.
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
