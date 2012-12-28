Expression Engine Library 1.0.0.3 Alfa.
===
This project is a simple mathematical expression parser written in C#. It's the refactoring of an old project and now uses shunting-yard algorithm.
It was written primarily for fun and currently doesn't support variables and functions; but it will in near future, along with other features.

News:
---
  - Solution now opens in MonoDevelop without problems.
  - All line endings converted to unix form.

To build:
---
MonoDevelop or Visual Studio.

At glance:
---
```csharp
var result = new ExpressionEvaluator().Evaluate("-3 * 0.31 / ((19 + 10) - .7)");
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
