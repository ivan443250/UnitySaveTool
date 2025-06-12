using System;
using System.Reflection;

namespace UnitySaveTool.Tools
{
    public struct GenericMethodInvoker
    {
        public static object Invoke(string methodName, object methodOwner, Type genericType, params object[] parameters)
        {
            MethodInfo methodInfo = methodOwner
                .GetType()
                .GetMethod(methodName)
                .MakeGenericMethod(genericType);

            return methodInfo.Invoke(methodOwner, parameters);
        }

        public static object Invoke(string methodName, object methodOwner, object parameter, Type[] genericTypes)
        {
            MethodInfo methodInfo = methodOwner
                .GetType()
                .GetMethod(methodName)
                .MakeGenericMethod(genericTypes);

            return methodInfo.Invoke(methodOwner, new object[] { parameter });
        }
    }
}