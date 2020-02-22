using System;
using System.Linq;
using System.Reflection;

static class TypeExtensions
{
    public static string AssemblyVersion(this Type type) =>
        ((AssemblyVersionAttribute)type.Assembly.GetCustomAttributes(
            typeof(AssemblyVersionAttribute)).Single()).Version;
}