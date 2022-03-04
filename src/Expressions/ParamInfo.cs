// (c) Nick Polyak 2018 - http://awebpros.com/
// License: MIT License https://opensource.org/licenses/MIT
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author(s) of this software if something goes wrong. 
// 
// Also as a courtesy, please, mention this software in any documentation for the 
// products that use it.

using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Xml.Serialization;

namespace NP.Utilities.Expressions
{
    public class ParamInfo : VMBase
    {
        Type _paramType;
        [XmlIgnore]
        public Type ParamType
        {
            get => _paramType;
            set
            {
                //if (_paramType.ObjEquals(value))
                //{
                //    return;
                //}

                _paramType = value;

                OnPropertyChanged(nameof(ParamType));
            }
        }

        string _paramName;
        [XmlAttribute]
        public string ParamName 
        {
            get => _paramName;
            
            set
            {
                if (_paramName.ObjEquals(value))
                {
                    return;
                }

                _paramName = value;

                OnPropertyChanged(nameof(ParamName));
            }
        }

        [XmlElement]
        public string ParamAssemblyQualifiedTypeName
        {
            get => ParamType?.AssemblyQualifiedName;

            set
            {
                ParamType = value?.GetTypeByAssemblyQualifiedName();
            }
        }

        public string ParamStr =>
            GetParamStr();

        public string GetParamStr(Func<Type, string> typeToStr = null)
        {
            return $"{ParamType?.GetFullTypeName(typeToStr)} {ParamName}";
        }

        public bool TypeMatches(ParamInfo paramInfo)
        {
            return this.ParamType == paramInfo.ParamType;
        }

        public bool Matches(ParamInfo paramInfo)
        {
            return this.TypeMatches(paramInfo) &&
                   this.ParamName == paramInfo.ParamName;
        }

        public string DisplayStr
        {
            get

            {
                return ParamType.Name.UnBox() + (ParamName.IsStrNullOrWhiteSpace() ? "" : $" {ParamName}");
            }
        }

        public ParamInfo()
        {

        }

        public ParamInfo(Type paramType, string paramName)
        {
            ParamType = paramType;
            ParamName = paramName;
        }

        public ParamInfo(ParamInfo paramInfo) : this(paramInfo.ParamType, paramInfo.ParamName)
        {

        }
    }

    public static class ParamInfoExtensions
    {
        public static bool CanServeAsInputs(this IEnumerable<Type> inputTypes, IEnumerable<Type> methodInputTypes)
        {
            if (inputTypes.IsNullOrEmpty())
                return methodInputTypes.IsNullOrEmpty();

            if (methodInputTypes == null)
                return false;

            if (inputTypes.Count() != methodInputTypes.Count())
                return false;

            return 
                inputTypes
                    .Zip(methodInputTypes, (input, methodInput) => (input, methodInput))
                    .All(pair => pair.methodInput.IsAssignableFrom(pair.input));
        }

        public static ParamInfo[] GetInputParamInfos(this MethodInfo methodInfo)
        {
            return methodInfo.GetParameters().Select(paramInfo => new ParamInfo(paramInfo.ParameterType, paramInfo.Name)).ToArray();
        }

        public static bool TypeMatches(this IEnumerable<ParamInfo> paramInfos, IEnumerable<ParamInfo> paramInfosToMatch)
        {
            if (paramInfos == null)
            {
                return paramInfosToMatch == null;
            }

            if (paramInfosToMatch == null)
                return false;

            return (paramInfos.Count() == paramInfosToMatch.Count()) &&
                paramInfos.Zip(paramInfosToMatch)
                          .All(paramInfosPair => paramInfosPair.First.TypeMatches(paramInfosPair.Second));
        }
    }
}
