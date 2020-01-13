// (c) Nick Polyak 2018 - http://awebpros.com/
// License: Apache License 2.0 (http://www.apache.org/licenses/LICENSE-2.0.html)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author(s) of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NP.Utilities
{
    using System.Collections;
    using System.IO;
    using static NP.Utilities.StrUtils;

    public static class ReflectionUtils
    {
        public static MemberInfo[] GetMemberInfos
        (
            this Type type,
            string propName,
            bool includeNonPublic
        )
        {
            BindingFlags bindingFlags = BindingFlags.Public;

            if (includeNonPublic)
                bindingFlags |= BindingFlags.NonPublic;

            MemberInfo[] result =
                type.GetMember(propName, bindingFlags);

            return result;
        }

        static MemberInfo GetSingleMemberInfoImpl
        (
            this Type type,
            string memberName,
            bool includeNonPublic
        )
        {
            MemberInfo[] memberInfos = type.GetMemberInfos(memberName, includeNonPublic);

            if (memberInfos.Length == 1)
            {
                return memberInfos.First();
            }

            if (memberInfos.Length > 1)
            {
                throw new Exception
                (
                    $"Error: there is more than one instance of member {memberName} within type {type.GetFullTypeStr()}."
                );
            }

            // if no members found, return null
            return null;
        }

        public static MemberInfo GetSingleMemberInfo(this Type type, string memberName)
        {
            MemberInfo result = type.GetSingleMemberInfoImpl(memberName, false);

            if (result == null)
            {
                // if there are no public members found - try a non-public one
                result = type.GetSingleMemberInfoImpl(memberName, true);

                if (result == null)
                {
                    throw new Exception
                        ($"Error: No member of name {memberName} is found within {type.GetFullTypeStr()} class.");

                }
            }

            return result;
        }

        static BindingFlags GetBindingFlags(bool includeNonPublic, bool isStatic)
        {
            BindingFlags bindingFlags =
                BindingFlags.Public;

            if (includeNonPublic)
                bindingFlags |= BindingFlags.NonPublic;

            if (isStatic)
            {
                bindingFlags |= BindingFlags.Static;
            }
            else
            {
                bindingFlags |= BindingFlags.Instance;
            }

            return bindingFlags;
        }

        public static FieldInfo GetFieldInfoFromType
        (
            this Type type,
            string fieldName,
            bool includeNonPublic = true)
        {
            BindingFlags bindingFlags = GetBindingFlags(includeNonPublic, false);

            FieldInfo fieldInfo = type.GetField(fieldName, bindingFlags);

            return fieldInfo;
        }

        public static PropertyInfo GetPropInfoFromType
        (
            this Type type,
            string propName,
            bool includeNonPublic = false)
        {
            BindingFlags bindingFlags = GetBindingFlags(includeNonPublic, false);

            PropertyInfo sourcePropInfo = type.GetProperty(propName, bindingFlags);

            return sourcePropInfo;
        }

        public static MethodInfo GetMethodInfoFromType
        (
            this Type type,
            string methodName,
            bool includeNonPublic = false
        )
        {
            BindingFlags bindingFlags = GetBindingFlags(includeNonPublic, false);

            MethodInfo methodInfo = type.GetMethod(methodName, bindingFlags);

            return methodInfo;
        }


        public static Type GetPropType
        (
            this Type type,
            string propName,
            bool includeNonPublic = false)
        {
            return type.GetPropInfoFromType(propName, includeNonPublic).PropertyType;
        }

        public static Type GetMethodArgType
        (
            this Type type, 
            string methodName, 
            int argIdx = 0,
            bool includeNonPublic = false
        )
        {
            MethodInfo methodInfo = 
                type.GetMethodInfoFromType(methodName, includeNonPublic);

            ParameterInfo[] parameters = 
                methodInfo.GetParameters();

            if (argIdx >= parameters.Length)
            {
                throw new Exception($"Method '{type.Name}.{methodName}' " +
                                    $"does not have parameter with index '{argIdx}'");
            }

            return parameters[argIdx].ParameterType;
        }


        public static FieldInfo GetFieldInfo
        (
            this object obj,
            string fieldName,
            bool includeNonPublic = true, 
            Type realType = null)
        {
            if (realType == null)
                realType = obj.GetType();

            return realType.GetFieldInfoFromType(fieldName, includeNonPublic);
        }


        public static PropertyInfo GetPropInfo
        (
            this object obj, 
            string propName, 
            bool includeNonPublic = false)
        {
            PropertyInfo sourcePropInfo = obj.GetType().GetPropInfoFromType(propName, includeNonPublic);

            return sourcePropInfo;
        }

        public static T GetFieldValue<T>
        (
            this object obj,
            string fieldName,
            bool includeNonPublic = true,
            Type realType = null)
        {
            FieldInfo fieldInfo = obj.GetFieldInfo(fieldName, includeNonPublic, realType);

            return (T) fieldInfo.GetValue(obj);
        }

        public static object GetPropValue
        (
            this object obj, 
            string propName, 
            bool includeNonPublic = false)
        {
            PropertyInfo propInfo = obj.GetPropInfo(propName, includeNonPublic);

            return propInfo.GetValue(obj, null);
        }

        public static T GetPropValue<T>
        (
            this object obj, 
            string propName,
            bool includeNonPublic = false)
        {
            return (T)obj.GetPropValue(propName, includeNonPublic);
        }

        public static void SetFieldValue
        (
            this object obj, 
            string fieldName, 
            object val, 
            bool includeNonPublic = true,
            Type realType = null)
        {
            FieldInfo fieldInfo = GetFieldInfo(obj, fieldName, includeNonPublic, realType);

            fieldInfo.SetValue(obj, val);
        }

        public static void SetPropValue(this object obj, string propName, object val, bool includeNonPublic = false)
        {
            PropertyInfo propInfo = GetPropInfo(obj, propName, includeNonPublic);

            propInfo.SetValue(obj, val, null);
        }


        public static object CallMethod(this object obj, string methodName, bool includeNonPublic, bool isStatic, params object[] args)
        {
            BindingFlags bindingFlags = GetBindingFlags(includeNonPublic, isStatic);

            Type type = null;
            if (isStatic)
            {
                type = (Type)obj;
            }
            else
            {
                type = obj.GetType();
            }

            MethodInfo methodInfo = type.GetMethod(methodName, bindingFlags);

            return methodInfo.Invoke(obj, args);
        }


        public static T GetCompoundPropValue<T>
        (
            this object obj, 
            string compoundPropName
        )           
            where T : class
        {
            (string firstLink, string remainder) =
                compoundPropName.BreakStrAtSeparator(PLAIN_PROP_PATH_LINK_SEPARATOR);

            object nextObj = obj.GetPropValue<T>(firstLink);

            if (remainder == null)
                return (T) nextObj;

            return nextObj?.GetCompoundPropValue<T>(remainder);
        }

        public static void SetCompoundPropValue(this object obj, string compoundPropName, object val)
        {
            object nextObj = obj;

            (string firstLink, string remainder) =
                compoundPropName.BreakStrAtSeparator(PLAIN_PROP_PATH_LINK_SEPARATOR);

            if (remainder == null)
            {
                obj.SetPropValue(firstLink, val);
                return;
            }

            nextObj = obj.GetPropValue(firstLink);

            nextObj?.SetCompoundPropValue(remainder, val);
        }

        public static PropertyInfo GetStaticPropInfo(this Type type, string propName)
        {
            PropertyInfo propInfo = type.GetProperty(propName, BindingFlags.Static | BindingFlags.Public);

            return propInfo;
        }

        public static object GetStaticPropValue(this Type type, string propName)
        {
            PropertyInfo propInfo = type.GetStaticPropInfo(propName);

            object val = propInfo.GetValue(type);

            return val;
        }


        public static FieldInfo GetStaticFieldInfo(this Type type, string propName)
        {
            FieldInfo fieldInfo = type.GetField(propName, BindingFlags.Static | BindingFlags.Public);

            return fieldInfo;
        }

        public static object GetStaticFieldValue(this Type type, string fieldName)
        {
            FieldInfo fieldInfo = type.GetStaticFieldInfo(fieldName);

            object val = fieldInfo.GetValue(type);

            return val;
        }


        public static ConstructorInfo GetDefaultConstructor(this Type type)
        {
            ConstructorInfo result = type.GetConstructor(Type.EmptyTypes);

            return result;
        }

        public static bool HasDefaultConstructor(this Type type)
        {
            ConstructorInfo constructorInfo = type.GetConstructor(Type.EmptyTypes);

            return (constructorInfo != null);
        }

        public static Type GetTypeFromTypeName(this string typeName)
        {
            Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in allAssemblies)
            {
                Type type = assembly.GetType(typeName);

                if (type != null)
                    return type;
            }

            return null;
        }

        public static Assembly FindOrLoadAssembly(this AssemblyName assemblyName)
        {
            Assembly result =
                AppDomain.CurrentDomain
                         .GetAssemblies()
                         .FirstOrDefault(assembly => assembly.FullName == assemblyName.FullName);

            if (result == null)
                result = Assembly.Load(assemblyName);

            return result;
        }

        public static IEnumerable<Assembly> AssemblyNamesToAssembly(this IEnumerable<AssemblyName> assemblyNames)
        {
            var result = assemblyNames.Select(assemblyName => assemblyName.FindOrLoadAssembly()).ToList();

            return result;
        }

        public static IEnumerable<Assembly> GetAssembliesReferencedByAssembly(this Assembly assembly)
        {
            return assembly.GetReferencedAssemblies().AssemblyNamesToAssembly();
        }

        public static IEnumerable<Assembly> GetAssemblyAndReferencedAssemblies(this Assembly assembly)
        {
            return (new[] { assembly }).Union(assembly.GetAssembliesReferencedByAssembly()).ToList();
        }

        public static IEnumerable<Assembly> GetAndLoadAssemblyAndAssembliesItDependsOn(this Assembly assembly)
        {
            IEnumerable<Assembly> result = 
                assembly
                    .GetReferencedAssemblies()
                    .Select(assemblyName => Assembly.Load(assemblyName))
                    .Concat(new[] { assembly });

            return result;
        }

        public static bool HasGetter(this PropertyInfo propInfo)
        {
            return propInfo.GetGetMethod() != null;
        }

        public static bool HasSetter(this PropertyInfo propInfo)
        {
            return propInfo.GetSetMethod() != null;
        }

        public static bool HasGetterAndSetter(this PropertyInfo propInfo)
        {
            return propInfo.HasGetter() && propInfo.HasSetter();
        }


        public static IEnumerable<PropertyInfo> GetTypePropsWithGetterAndSetter(this Type type)
        {
            return
                type
                    .GetProperties()
                    .Where (propInfo => propInfo.HasGetterAndSetter());
        }

        public static IEnumerable<PropertyInfo> GetPropsWithGettersAndSetter(this object obj)
        {
            return obj.GetType().GetTypePropsWithGetterAndSetter();
        }

        public static string GetFullGenericTypeName(this Type type)
        {
            return type.FullName.SubstrFromTo(null, "[");
        }

        public static string GetFullTypeStr(this Type type)
        {
            return type.Namespace + "." + type.Name;
        }

        public static void SetStaticPropValue
        (
            this Type type, 
            string propName, 
            object valueToSet)
        {
            PropertyInfo propInfo = 
                type.GetProperty(propName, BindingFlags.Static | BindingFlags.Public);

            propInfo.SetValue(null, valueToSet);
        }

        public static IEnumerable<T> GetAttrs<T>(this ICustomAttributeProvider memberInfo)
            where T : Attribute
        {
            return memberInfo.GetCustomAttributes(typeof(T), false) as IEnumerable<T>;
        }

        public static TAttr GetAttr<TAttr>(this ICustomAttributeProvider memberInfo)
            where TAttr : Attribute
        {
            return memberInfo.GetAttrs<TAttr>().FirstOrDefault();
        }

        public static bool ContainsAttr<TAttr>(this ICustomAttributeProvider memberInfo)
            where TAttr : Attribute
        {
            return memberInfo.GetAttr<TAttr>() != null;
        }

        public static Type GetBaseTypeOrFirstInterface(this Type type)
        {
            Type result = type.BaseType;

            if (result == typeof(object))
            {
                result = type.GetInterfaces().FirstOrDefault();
            }

            return result;
        }

        public static string GetCurrentExecutablePath()
        {
            string currentExecutablePath =
                Assembly.GetEntryAssembly().Location;

            return Path.GetDirectoryName(currentExecutablePath);
        }

        private static Type FindTypeByFullNameImpl(string str)
        {
            return AppDomain.CurrentDomain
                            .GetAssemblies()
                            .SelectMany(a => a.GetTypes())
                            .FirstOrDefault(t => t.FullName == str);
        }

        public static Type RestoreType(string str)
        {
            return FindTypeByFullNameImpl(str);
        }

        private static Cache<string, Type> _fullNameTypesCache = 
            new Cache<string, Type>(RestoreType);

        public static Type FindTypeByFullName(this string str)
        {
            return _fullNameTypesCache.Get(str);
        }

        public static Type GetTypeByAssemblyQualifiedName(this string assemblyQualifiedTypeName)
        {
            static Assembly FindAssembly(AssemblyName assemblyName)
            {
                return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == assemblyName.Name);
            }

            static Type FindType(Assembly assembly, string typeName, bool ignoreCase)
            {
                return assembly.GetType(typeName);
            }

            Type type = Type.GetType(assemblyQualifiedTypeName, FindAssembly, FindType);

            return type;
        }

        public static Type GetCellTypeFromCollectionType(this Type collectionType)
        {
            return collectionType
                    ?.GetMethod("GetEnumerator")
                    ?.ReturnType
                    ?.GetProperty("Current")
                    ?.PropertyType;
        }

        public static (bool IsCollection, Type CellType) GetIsCollectionInfo(this Type type)
        {
            if (type == null || !typeof(IEnumerable).IsAssignableFrom(type))
            {
                return (false, null);
            }

            return (true, type.GetCellTypeFromCollectionType());
        }

        public static string GetAssemblyNameFromAssemblyResolveArgs(this ResolveEventArgs args)
        {
            return args.Name.SubstrFromTo(null, ",");
        }

        public static bool IsVoid(this Type type)
        {
            return type == null || type.FullName == "System.Void";
        }

        public static object GetDefaultForType(this Type type)
        {
            if (type?.IsValueType == true)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }
    }
}
