using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System
{

    internal static class StringExtensions
    {
        public static string FormatInvariant(this string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

    }

    internal static class TypeExtensions
    {
        private static Type nullableType = typeof(Nullable<>);

        /// <summary>
        /// Returns the underlying type in case of a Nullable.
        /// </summary>
        /// <param name="thisType"></param>
        /// <returns></returns>
        public static Type UnwrapNullable(this Type thisType)
        {
            return thisType.IsGenericType && thisType.GetGenericTypeDefinition() == nullableType
                ?
                Nullable.GetUnderlyingType(thisType)
                :
                thisType;
        }


    }
}

namespace System.Reflection
{
    public static class RuntimeReflectionExtensions
    {

        public static MethodInfo GetRuntimeBaseDefinition(this MethodInfo method)
        {
            if (method == null) throw new ArgumentException("method");
            return method.GetBaseDefinition();
        }

        public static MethodInfo GetRuntimeMethod(this Type type, string name, Type[] parameters)
        {
            if (type == null) throw new ArgumentException("type");
            return type.GetMethod(name, parameters);
        }
    }
}