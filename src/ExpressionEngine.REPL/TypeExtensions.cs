using System;
using System.Linq;
using System.Reflection;

static class TypeExtensions
{
    public static string AssemblyVersion(this Type type) => type.Assembly.GetName().Version.ToString();
}