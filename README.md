# Expression Engine

This project is a simple mathematical expression parser written in C#. It's the refactoring of an old project and uses an AST-evaluation algorithm. You can use built-in or bind your functions and variables to global scope.

## Documentation

Documentation is available in the project [Wiki](https://github.com/gsscoder/exprengine/wiki).

## At a glance:
---
```csharp
var contex = new Context()
  .SetVariable("G", 6.67428D)
  .SetVariable("earth_mass", Evaluate.As<double>("5.97219 * pow(10,24)")) // 5.97219E+24 kg
  .SetVariable("lunar_mass", Evaluate.As<double>("7.34767309 * pow(10,22)")) // 7.34767309E+22 kg
  .SetVariable("perigee_dist", 356700000D) // moon-earth distance at perigee in m
  .SetFunction("calc_force", (object[] args) => ((double) args[0] * (double) args[1]) / Math.Pow((double) args[2], 2));
var result = contex.EvaluateAs<double>("G * calc_force(earth_mass, lunar_mass, perigee_dist)"); // 2.3018745174107073E+31
bool isGCorrect = contex.EvaluateAs<bool>("G == 6.67428");
```