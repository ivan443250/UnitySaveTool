using System;
using System.Linq;
using System.Reflection;

namespace UnitySaveTool.Tools
{
    public struct AttributeTypeFinder
    {
        public static Type[] GetTypesWithAttribute<TAttribute>(Assembly assembly = null) where TAttribute : Attribute
        {
            assembly ??= Assembly.GetExecutingAssembly();

            return assembly.GetTypes()
                .Where(t => t.GetCustomAttributes(typeof(TAttribute), false).Length > 0)
                .ToArray();
        }
    }
}