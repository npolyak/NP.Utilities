// (c) Nick Polyak 2018 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
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
    using NP.Utilities.BasicInterfaces;
    using System.Collections;
    using System.IO;
    using System.Threading.Tasks;
    using static NP.Utilities.StrUtils;
   
    public enum ParamKind
    {
        Plain,
        Ref,
        Out,
        Return
    }

    public static class ReflectionUtils
    {
        public static ParamKind GetParamKind(this ParameterInfo? paramInfo)
        {
            if (paramInfo.IsRetval)
            {
                return ParamKind.Return;
            }

            if (paramInfo.ParameterType.IsByRef)
            {
                if (paramInfo.IsOut)
                {
                    return ParamKind.Out;
                }
                else
                {
                    return ParamKind.Ref;
                }
            }

            return ParamKind.Plain;
        }

        public static string GetParamInfoStr(this ParameterInfo paramInfo)
        {
            return $"{paramInfo.Name}-{paramInfo.ParameterType.Name} 'Position:{paramInfo.Position}', 'Kind:{paramInfo.GetParamKind()}'";
        }

        public static MemberInfo[] GetMemberInfos
        (
            this Type type,
            string propName,
            bool includeNonPublic,
            bool includeStatic = false
        )
        {
            BindingFlags bindingFlags = GetBindingFlags(includeNonPublic, includeStatic);

            MemberInfo[] result =
                type.GetMember(propName, bindingFlags);

            return result;
        }

        static MemberInfo GetSingleMemberInfoImpl
        (
            this Type type,
            string memberName,
            bool includeNonPublic,
            bool includeStatic = false
        )
        {
            MemberInfo[] memberInfos = type.GetMemberInfos(memberName, includeNonPublic, includeStatic);

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

        public static MemberInfo 
            GetSingleMemberInfo
            (
            this Type type, 
            string memberName,
            bool includeNonPublic = false,
            bool includeStatic = false)
        {
            MemberInfo result = 
                type.GetSingleMemberInfoImpl
                (
                    memberName, 
                    includeNonPublic, 
                    includeStatic);

            if (result == null)
            {
                throw new Exception
                    ($"Error: No member of name {memberName} is found within {type.GetFullTypeStr()} class.");

            }

            return result;
        }

        private static BindingFlags GetBindingFlags(bool includeNonPublic, bool isStatic)
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
            bool includeNonPublic = true,
            bool includeStatic = false)
        {
            BindingFlags bindingFlags = GetBindingFlags(includeNonPublic, includeStatic);

            FieldInfo fieldInfo = type.GetField(fieldName, bindingFlags);

            return fieldInfo;
        }

        public static PropertyInfo GetPropInfoFromType
        (
            this Type type,
            string propName,
            bool includeNonPublic = false, 
            bool includeStatic = false)
        {
            BindingFlags bindingFlags = GetBindingFlags(includeNonPublic, includeStatic);

            PropertyInfo sourcePropInfo = type.GetProperty(propName, bindingFlags);

            return sourcePropInfo;
        }

        public static MethodInfo GetMethodInfoFromType
        (
            this Type type,
            string methodName,
            bool includeNonPublic = false,
            bool includeStatic = false
        )
        {
            BindingFlags bindingFlags = GetBindingFlags(includeNonPublic, includeStatic);

            MethodInfo methodInfo = type.GetMethod(methodName, bindingFlags);

            return methodInfo;
        }


        public static Type GetPropType
        (
            this Type type,
            string propName,
            bool includeNonPublic = false,
            bool includeStatic = false)
        {
            return type.GetPropInfoFromType(propName, includeNonPublic, includeStatic).PropertyType;
        }

        public static Type GetMethodArgType
        (
            this Type type,
            string methodName,
            int argIdx = 0,
            bool includeNonPublic = false,
            bool includeStatic = false
        )
        {
            MethodInfo methodInfo =
                type.GetMethodInfoFromType(methodName, includeNonPublic, includeStatic);

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

            return (T)fieldInfo.GetValue(obj);
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

        public static MethodInfo GetMethod(this object obj, string methodName, bool includeNonPublic, bool isStatic, params Type[] argTypes)
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

            var methodInfos = type.GetMember(methodName, bindingFlags).OfType<MethodInfo>().ToList();

            return methodInfos.FirstOrDefault(mInfo => mInfo.IsMethodCompatibleWithInputArgs(argTypes));
        }

        public static bool IsMethodCompatibleWithInputArgs(this MethodInfo method, params Type[] argRealTypes)
        {
            Type[] methodParamTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();

            if (methodParamTypes.Length != argRealTypes.Length)
                return false;

            return methodParamTypes.Zip(argRealTypes).All(item => item.First.IsAssignableFrom(item.Second));
        }

        public static object CallMethodExtras(this object obj, string methodName, bool includeNonPublic, bool isStatic, params object[] args)
        {
            MethodInfo methodInfo = obj.GetMethod(methodName, includeNonPublic, isStatic, args.Select(arg => arg.GetType()).ToArray());

            return methodInfo.Invoke(obj, args);
        }

        public static object CallMethod(this object obj, string methodName, params object[] args)
        {
            return CallMethodExtras(obj, methodName, false, false, args);
        }

        public static T GetCompoundPropValue<T>
        (
            this object obj, 
            string compoundPropName
        )           
            where T : class
        {
            (string firstLink, _, string remainder) =
                compoundPropName.BreakStrAtSeparator(PLAIN_PROP_PATH_LINK_SEPARATOR);

            object nextObj = obj.GetPropValue<T>(firstLink);

            if (remainder.IsNullOrEmpty())
                return (T) nextObj;

            return nextObj?.GetCompoundPropValue<T>(remainder);
        }

        public static void SetCompoundPropValue(this object obj, string compoundPropName, object val)
        {
            object nextObj = obj;

            (string firstLink, _, string remainder) =
                compoundPropName.BreakStrAtSeparator(PLAIN_PROP_PATH_LINK_SEPARATOR);

            if (remainder.IsNullOrEmpty())
            {
                obj.SetPropValue(firstLink, val);
                return;
            }

            nextObj = obj.GetPropValue(firstLink);

            nextObj?.SetCompoundPropValue(remainder, val);
        }

        public static PropertyInfo GetStaticPropInfo
        (
            this Type type, 
            string propName,
            bool includeNonPublic = false)
        {
            BindingFlags bindingFlags = GetBindingFlags(includeNonPublic, true);

            PropertyInfo propInfo = type.GetProperty(propName, bindingFlags);

            return propInfo;
        }

        public static object GetStaticPropValue
        (
            this Type type, 
            string propName,
            bool includeNonPublic = false)
        {
            PropertyInfo propInfo = type.GetStaticPropInfo(propName, includeNonPublic);

            object val = propInfo.GetValue(type);

            return val;
        }


        public static FieldInfo GetStaticFieldInfo
        (
            this Type type, 
            string propName,
            bool includeNonPublic = false)
        {
            BindingFlags bindingFlags = GetBindingFlags(includeNonPublic, true);

            FieldInfo fieldInfo = type.GetField(propName, bindingFlags);

            return fieldInfo;
        }

        public static object GetStaticFieldValue
        (
            this Type type, 
            string fieldName,
            bool includeNonPublic = false)
        {
            FieldInfo fieldInfo = type.GetStaticFieldInfo(fieldName, includeNonPublic);

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
                         .LastOrDefault(assembly => assembly.FullName == assemblyName.FullName);

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

        public static IEnumerable<Type> GetSelfSuperTypesAndInterfaces(this Type type)
        {
            if (type == null)
                yield break;

            yield return type;

            if (type.BaseType != null)
            {
                foreach(var superType in type.BaseType.GetSelfSuperTypesAndInterfaces())
                {
                    yield return superType;
                }
            }

            foreach(var interfaceType in type.GetInterfaces())
            {
                foreach(var superInterface in interfaceType.GetSelfSuperTypesAndInterfaces())
                {
                    yield return superInterface;
                }
            }
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

        public static IEnumerable<Type> GetBaseTypeAndInterfaces(this Type type)
        {
            if (type.BaseType != typeof(object))
            {
                yield return type.BaseType;
            }

            foreach(var interfaceType in type.GetInterfaces())
            {
                yield return interfaceType;
            }
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
            //return _fullNameTypesCache.Get(str);

            return FindTypeByFullNameImpl(str);
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
            if (!type.IsCollection())
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

        public static bool IsVoid(this MethodInfo methodInfo)
        {
            return methodInfo.ReturnType.IsVoid();
        }

        public static object GetDefaultForType(this Type type)
        {
            if (type?.IsValueType == true)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        public static IEnumerable<Type> GetAllGenericTypeParams(this Type type)
        {
            if (type == null)
                yield break;

            if (type.IsGenericParameter)
                yield return type;

            if (type.IsGenericType)
            {
                foreach (var genericArg in type.GetGenericArguments())
                {
                    foreach (var genericSubTypeParam in genericArg.GetAllGenericTypeParams())
                    {
                        yield return genericSubTypeParam;
                    }
                }
            }
            else if (type.IsArray)
            {
                Type elementType = type.GetElementType();

                foreach(var genericSubTypeParam in elementType.GetAllGenericTypeParams())
                {
                    yield return genericSubTypeParam;
                }
            }
        }

        /// <summary>
        /// e.g. for IEnumerable&lt;int&gt; returns a collection of two items 
        /// containing typeof(IEnumerable<>) and typeof(int)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllGenericSubTypes(this Type type)
        {
            if (type == null)
                yield break;

            if (type.IsGenericParameter)
                yield return type;

            if (type.IsGenericType)
            {
                foreach (var genericArg in type.GetGenericArguments())
                {
                    foreach (var genericSubTypeParam in genericArg.GetAllGenericSubTypes())
                    {
                        yield return genericSubTypeParam;
                    }
                }
                yield return type.GetGenericTypeDefinition();
            }
            else
            {
                yield return type;
            }
        }

        public static bool IsConcrete(this Type type)
        {
            return (type != null) && !type.GetAllGenericTypeParams().Any();
        }

        public static bool CheckConcreteTypeSatisfiesGenericParamConstraints(this Type concreteType, Type genericParamType)
        {
            bool hasReferenceTypeConstraint = 
                genericParamType.GenericParameterAttributes
                                .HasFlag(GenericParameterAttributes.ReferenceTypeConstraint);

            bool hasNewConstraint =
                genericParamType.GenericParameterAttributes
                                .HasFlag(GenericParameterAttributes.DefaultConstructorConstraint);

            bool isNonNullable = 
                genericParamType.GenericParameterAttributes
                                .HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint);

            if (hasReferenceTypeConstraint)
            {
                if (concreteType.IsValueType)
                    return false;
            }
            else if (isNonNullable && !concreteType.IsValueType)
            {
                return true;
            }

            if (hasNewConstraint)
            {
                ConstructorInfo constrInfo = 
                    concreteType.GetConstructor(new Type[] { });

                if (constrInfo == null)
                    return false;
            }

            Type[] constraintTypes = 
                genericParamType.GetGenericParameterConstraints();

            if (constraintTypes == null)
                return true;

            foreach (Type constraintType in constraintTypes)
            {
                if ( !concreteType.ResolveType(constraintType, constraintType.GetFromGenericType()))
                {
                    return false;
                }
            }

            return true;
        }

        public static Type GetGenericMatchingSuperType(this Type typeToScan, Type typeToCompare)
        {
            var matchingSuperType =
                typeToScan.GetSelfSuperTypesAndInterfaces()
                    .Distinct()
                    .Where(arg => arg.IsGenericType)
                    .FirstOrDefault(arg => arg.GetGenericTypeDefinition() ==
                                            typeToCompare.GetGenericTypeDefinition());

            return matchingSuperType;
        }


        /// <summary>
        /// Checks if the argToResolveType can be used to resolve a
        /// genericArgType as a parameter in some
        /// method or class. E.g. for method static void DoSmth<TKey, TVal>(IDictionary<TKey, TVal> dict) where TVal : struct { }
        /// we can check if Dictionary<int, double> will fit to be passed as dict argument 
        ///     (answer is yes, so it will return true)
        /// or we can check if Dictionary<int, object> will fit 
        ///     (answer is no, because of the 'struct' constraint, so it will return false).
        /// If it can resolve all generic type parameters, it will also populate 
        /// the ConcreteType property of members of the collection 'typesToConcretize' with 
        /// the concrete types to replace the generic type parameters. In case of 
        /// Dict<int, double>, the Concrete types for the corresponding collection members will be 
        ///     typeof(int) and typeof(double). 
        /// The typesToConcretize collection should already contain the 
        /// entries corresponding to every generic parameter 
        /// that the genericArgType depends on (for the example above, it will contain 2 entries - 
        /// one for TKey and another for TVal).
        /// </summary>
        public static bool ResolveType
        (
            this Type sourceArgType,
            Type targetArgType,
            IEnumerable<IGenericParamInfo> genericTypeParamsToConcretize,
            bool resolveTypesFromSourceToTarget = true)
        {
            if (targetArgType == null)
                return true;

            if ((resolveTypesFromSourceToTarget && targetArgType.IsGenericParameter) ||
                 (!resolveTypesFromSourceToTarget && sourceArgType.IsGenericParameter))
            {
                if (!resolveTypesFromSourceToTarget)
                {
                    var saveTargetArgType = targetArgType;
                    targetArgType = sourceArgType;
                    sourceArgType = saveTargetArgType;
                }

                IGenericParamInfo foundParamInfo = genericTypeParamsToConcretize.Single(t => t.GenericParameterType == targetArgType);

                if (!sourceArgType.CheckConcreteTypeSatisfiesGenericParamConstraints(foundParamInfo.GenericParameterType))
                {
                    return false;
                }

                foundParamInfo.PluggedInType = sourceArgType;

                return true;
            }
            else if (targetArgType.IsGenericType)
            {
                var matchingSuperType =
                        sourceArgType.GetGenericMatchingSuperType(targetArgType);

                if (matchingSuperType == null)
                    return false;

                Type[] sourceArgSubTypes = matchingSuperType.GetGenericArguments();
                Type[] targetArgSubTypes = targetArgType.GetGenericArguments();

                foreach ((Type sourceArgSubType, Type targetArgSubType) in sourceArgSubTypes.Zip(targetArgSubTypes, (c, g) => (c, g)))
                {
                    if (!sourceArgSubType.ResolveType(targetArgSubType, genericTypeParamsToConcretize, resolveTypesFromSourceToTarget))
                        return false;
                }

                return true;
            }

            return targetArgType.IsAssignableFrom(sourceArgType);

        }

        public static bool IsTask(this Type type)
        {
            return typeof(Task).IsAssignableFrom(type);
        }

        public static string GetTypeAndMethodName(this MethodInfo methodInfo)
        {
            string fullTypeName = methodInfo.DeclaringType.FullName;
            string methodName = methodInfo.Name;

            return fullTypeName + ":" + methodName;
        }

        public static bool IsDelegateType(this Type type)
        {
            return typeof(Delegate).IsAssignableFrom(type);
        }

        public static bool IsVoidDelegate(this Type type)
        {
            if (!type.IsDelegateType())
            {
                return false;
            }

            return
                type.GetDelegateReturnType() == null;
        }

        public static Type GetDelegateReturnType(this Type type)
        {
            if (type.IsDelegateType())
            {
                Type delegateReturnType = type.GetMethod(nameof(Action.Invoke)).ReturnType;

                if (delegateReturnType.IsVoid())
                {
                    return null;
                }

                return delegateReturnType;
            }

            return null;
        }

        public static bool IsResultlessTask(this Type type)
        {
            if (!type.IsTask())
                return false;

            return !type.IsGenericType;
        }
    }
}
