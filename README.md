# Expression Engine

This project is a simple mathematical expression parser written in C#. It's the refactoring of a previous project and uses an AST-evaluation algorithm. You can use built-in or bind your functions and variables to global scope.

## Documentation

Documentation is available in the project [Wiki](https://github.com/gsscoder/exprengine/wiki).

## Targets

- .NET Standard 2.0
- .NET Framework 4.0, 4.5, 4.6.1

## At a glance:

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

## REPL

```sh
$ dotnet build -c release
...
$ ./artifacts/ExpressionEngine.REPL/Release/netcoreapp3.1/xeval --help
xeval 1.0.0
Copyright © Giacomo Stelluti Scala, 2012-2020
This is free software. You may redistribute copies of it under the terms of
the MIT License <http://www.opensource.org/licenses/mit-license.php>.
Usage: xeval [EXPRESSION]
       xeval --interactive
       xeval -f [FILENAME]
       cmdx | xeval

  -f, --filename       Input text file to be evaluated.

  -i, --interactive    Run in interactive mode.

  -q, --quiet          Print only the evaluation results.

  -v, --version        Print version information and exit.

  --help               Display this help screen.
```