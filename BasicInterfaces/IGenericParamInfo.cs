using System;
using System.Collections.Generic;
using System.Linq;

namespace NP.Utilities.BasicInterfaces
{
    public interface IGenericParamInfo
    {
        Type GenericParameterType { get; }

        string GenericParamTypeName => GenericParameterType.Name;

        bool IsResolved => PluggedInType != null;

        Type PluggedInType { get; set; }
    }

    public class GenericParamInfoBase : IGenericParamInfo
    {
        public Type GenericParameterType { get; }

        public string GenericParamTypeName => GenericParameterType.Name;

        public bool IsResolved => PluggedInType != null /* && PluggedInType.IsConcrete() */;

        public virtual Type PluggedInType { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is IGenericParamInfo paramObserver)
            {
                return GenericParameterType == paramObserver.GenericParameterType;
            }

            return false;
        }


        /// <summary>
        /// for test purpose only
        /// </summary>
        public void SetPluggedInTypeToInt()
        {
            PluggedInType = typeof(int);
        }

        public override int GetHashCode()
        {
            return GenericParameterType.GetHashCode();
        }

        public GenericParamInfoBase
        (
            Type genericParamType,
            Type pluggedInType = null)
        {
            GenericParameterType = genericParamType;
            PluggedInType = pluggedInType;
        }

        public void ResetConcreteType()
        {
            PluggedInType = null;
        }
    }


    public static class GenericParamsExtensions
    {
        public static bool AreAllResolved(this IEnumerable<IGenericParamInfo> genericParamInfos)
        {
            return genericParamInfos == null ? true : genericParamInfos.All(paramInfo => paramInfo.IsResolved);
        }

        public static Type[] GetConcreteTypes(this IEnumerable<IGenericParamInfo> genericParamInfos)
        {
            return genericParamInfos.Select(paramInfo => paramInfo.PluggedInType).ToArray();
        }

        public static IGenericParamInfo GetGenericParamInfo(this IEnumerable<IGenericParamInfo> genericParams, Type genericType)
        {
            return genericParams
                        .Single(genParamInfo => genParamInfo.GenericParameterType == genericType);
        }

        public static IGenericParamInfo[] GetFromGenericType
        (
            this Type genericType,
            IEnumerable<IGenericParamInfo> genericParamInfosToChooseFrom = null,
            Func<Type, IGenericParamInfo> factory = null)
        {
            var result =
                genericType.GetAllGenericTypeParams()
                 .Distinct()
                 .Select
                 (
                    genArgType => genericParamInfosToChooseFrom?.GetGenericParamInfo(genArgType) ??
                       factory?.Invoke(genArgType) ??
                       new GenericParamInfoBase(genArgType)
                 )
                 .ToArray();

            return result;
        }


        /// <summary>
        /// creates a more concrete type based on generiParamInfos. 
        /// For example, for a type Action<IEnumerable<T>, T>, if genericParamInfos consists of one member, with T mapped to typeof(int)
        /// the result will be Action<IEnumerable<int>, int>. 
        /// For a type Action<IEnumerable<T>, T, T2>, if the genericParamInfos consists of two members, the first one mapping T to typeof(int)
        /// and the second one not mapping T2 to anything, the resulting type will be Action<IEnumerable<int>, int, T2> still not concrete, 
        /// but more concrete than before. 
        /// Concrete types will be returned unchanged, while a Type Parameter will be replaced based on the PlugInType of the
        /// corresponding GenericParamInfo. If the PlugInType is null, then the generic type parameter will be returned.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericParamInfos"></param>
        /// <returns></returns>
        public static Type ConcretizeType(this Type type, IEnumerable<IGenericParamInfo> genericParamInfos)
        {
            if (type.IsConcrete())
                return type;

            if (type.IsGenericParameter)
            {
                IGenericParamInfo paramInfo = genericParamInfos.Single(pI => pI.GenericParameterType == type);

                return paramInfo.PluggedInType ?? type;
            }

            Type genericTypeDefinition = type.GetGenericTypeDefinition();

            Type[] args = type.GetGenericArguments().Select(t => t.ConcretizeType(genericParamInfos)).ToArray();

            return genericTypeDefinition.MakeGenericType(args);
        }
    }
}
