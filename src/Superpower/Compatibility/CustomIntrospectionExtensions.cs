﻿// ReSharper disable once CheckNamespace
using System.Linq;

namespace System.Reflection
{
    /// <summary>
    /// https://github.com/castleproject/Core/blob/netcore/src/Castle.Core/Compatibility/IntrospectionExtensions.cs
    /// </summary>
	internal static class CustomIntrospectionExtensions
    {
#if NET35 || NET40
        // This allows us to use the new reflection API which separates Type and TypeInfo
        // while still supporting .NET 3.5 and 4.0. This class matches the API of the same
        // class in .NET 4.5+, and so is only needed on .NET Framework versions before that.
        //
        // Return the System.Type for now, we will probably need to create a TypeInfo class
        // which inherits from Type like .NET 4.5+ and implement the additional methods and
        // properties.
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }

        public static FieldInfo GetDeclaredField(this Type type, string name)
        {
            return type.GetFields().Where(m => m.Name == name).FirstOrDefault();
        }

        public static T GetCustomAttribute<T>(this FieldInfo type) where T : Attribute
        {
            return type.GetCustomAttributes(false).OfType<T>().FirstOrDefault();
        }
#endif
    }
}