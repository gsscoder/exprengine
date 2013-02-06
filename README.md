Expression Engine Library 0.6.0.5 alfa.
===
This project is a simple mathematical expression parser written in C#. It's the refactoring of an old project and uses an AST-evaluation algorithm. You can use built-in or add your functions and variables to global scope. It was written primarily for fun and and as programming exercise; anyway if you anyone find it useful, please send feature requests or issues; but please take note that __this is still a work in progress__.

At the moment [specification](https://docs.google.com/document/d/1Mu0DlKErOY0RQGUYe3xaoY_2jgYVKA4wRxI8xiiLS0k/edit) is just a draft.

News:
---
  - String literals fully implemented.
  - Version policy changed.
  - First changes to lexer for handle string literals.
  - Refactoring, ExpressionEvaluator splitted in ``Context`` (as the name, evaluation with context) and ``Evaluator`` (static helper, contextless evaluation).
  - Added support for exponent in numeric literal ``([E|e][+|-][DIGIT])``.
  - Parser class aligned to new grammar.
  - BNF grammar changed (see [Evaluator.grm](https://github.com/gsscoder/exprengine/blob/master/doc/Evaluator.grm)).
  - Operator ``^`` (exponent) removed. Will be used as bitwise operator.
  - Introduced relational operators ``(==, !=, <, >, <=, >=)``.
  - Introduced ``true`` and ``false`` literals.
  - Heavy internal refactoring toward a new design.
  - Evaluation provided via both instance and static methods.

To build:
---
MonoDevelop or Visual Studio.

At glance:
---
```csharp
var contex = new Context()
  .SetVariable("G", 6.67428D)
  .SetVariable("earth_mass", Evaluator.EvaluateAs<double>("5.97219 * pow(10,24)")) // 5.97219E+24 kg
  .SetVariable("lunar_mass", Evaluator.EvaluateAs<double>("7.34767309 * pow(10,22)")) // 7.34767309E+22 kg
  .SetVariable("perigee_dist", 356700000D) // moon-earth distance at perigee in m
  .SetFunction("calc_force", (object[] args) => ((double) args[0] * (double) args[1]) / Math.Pow((double) args[2], 2));
var result = contex.EvaluateAs<double>("G * calc_force(earth_mass, lunar_mass, perigee_dist)"); // 2.3018745174107073E+31
bool isGCorrect = contex.EvaluateAs<bool>("G == 6.67428");
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
