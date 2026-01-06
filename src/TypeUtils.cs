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

using System.Collections;
using System.Reflection;

namespace NP.Utilities
{
    public static class TypeUtils
    {        
        // removes the 'apostrophy' and the number 
        // of generic args for a generic type
        public static string GetTypeName(string typeName)
        {
            return typeName.SubstrFromTo(null, "`");
        }

        // removes the 'apostrophy' and the number 
        // of generic args for a generic type
        public static string GetTypeName(this Type type)
        {
            return GetTypeName(type.Name);
        }

        public static string GetTypeNameWithNamespaceAndNumberGenericParams(this Type type)
        {
            return type.FullName.SubstrFromTo(null, "[");
        }

        public static string NestedTypeToName(this Type type)
        {
            string currentTypeName = type.GetTypeName();

            if (!type.IsNested)
                return currentTypeName;

            return type.DeclaringType.NestedTypeToName() + "_" + currentTypeName;
        }

        private static string UnBoxImpl(this string typeStr, bool unbox = true)
        {
            switch (typeStr)
            {
                case "Int32":
                    return "int";
                case "Int64":
                    return "long";
                case "String":
                    return "string";
                case "Double":
                    return "double";
                case "Decimal":
                    return "decimal";
                case "Boolean":
                    return "bool";
                case "Byte":
                    return "byte";
                case "Char":
                    return "char";
                case "Void":
                    return "void";
                case "Object":
                    return "object";
            }

            return typeStr;
        }

        public static string UnBox(this string typeStr, bool unboxIfNeeded = true)
        {
            if (unboxIfNeeded)
            {
                return typeStr.UnBoxImpl();
            }

            return typeStr;
        }

        public static string GetTypeNameWithUnboxing(this Type type)
        {
            if (type == typeof(object))
                return "object";

            return type?.Name;
        }

        // include the generic params
        public static string GetFullTypeName(this Type type, Func<Type, string> typeToStr = null)
        {
            if (type == null)
                return null;

            if(typeToStr == null)
            {
                typeToStr = (t) => t.GetTypeName();
            }
            string result = typeToStr(type).UnBoxImpl();

            if (type.IsGenericType)
            {
                result += "<";

                bool firstIteration = true;
                foreach(Type typeParam in type.GetGenericArguments())
                {
                    if (!firstIteration)
                    {
                        result += ", ";
                    }
                    else
                    {
                        firstIteration = false;
                    }

                    result += typeParam.GetFullTypeName(typeToStr).UnBoxImpl();
                }
                result += ">";
            }

            return result;
        }

        public static string GetFullTypeNameWithNamespaces(this Type type)
        {
            return GetFullTypeName(type, (t) => t.GetTypeNameWithNamespaceAndNumberGenericParams());
        }

        public static List<string> 
            GetNamespaces(this IEnumerable<Type> types)
        {
            return types
                .NullToEmpty()
                .Select(type => type.Namespace).Distinct().ToList();
        }

        public static List<string> GetLocations(this IEnumerable<Type> types)
        {
            return types
                .NullToEmpty()
                .Select(type => type.Assembly.Location)
                .Distinct()
                .ToList();
        }

        public static bool IsNullable(this Type type)
        {
            if (!type.IsValueType)
                return true;

            if (Nullable.GetUnderlyingType(type) != null)
                return true;

            return false;
        }

        public static object StrToType(this Type type, string str)
        {
            if (type == typeof(double))
            {
                return Convert.ToDouble(str);
            }
            if (type == typeof(bool))
            {
                return Convert.ToBoolean(str);
            }
            if (type == typeof(decimal))
            {
                return Convert.ToDecimal(str);
            }
            if (type == typeof(int))
            {
                return Convert.ToInt32(str);
            }
            if (type == typeof(DateTime))
            {
                return Convert.ToDateTime(str);
            }
            if (type == typeof(long))
            {
                return Convert.ToInt64(str);
            }
            if (type == typeof(float))
            {
                return Convert.ToSingle(str);
            }
            if (type == typeof(byte))
            {
                return Convert.ToByte(str);
            }
            if (type == typeof(char))
            {
                return Convert.ToChar(str);
            }
            if (type == typeof(uint))
            {
                return Convert.ToUInt32(str);
            }
            if (type == typeof(ulong))
            {
                return Convert.ToUInt64(str);
            }
            if (type == typeof(ushort))
            {
                return Convert.ToUInt16(str);
            }
            if (type.IsEnum)
            {
                return Enum.Parse(type, str);
            }

            return str;
        }

        public static bool IsCollection(this Type type)
        {
            return (type != null) && typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static Type GetChildType(this Type type, string propName)
        {
            PropertyInfo propInfo = type.GetProperty(propName);

            return propInfo.PropertyType;
        }

        public static Type GetItemType(this Type type)
        {
            if (!type.IsCollection())
                return type;

            Type argType =
                type.GenericTypeArguments.First();

            return argType;
        }

        public static Type GetTaskTypeIfTask(this Type type)
        {
            if (!type.IsTask())
                return type;

            Type argType =
                type.GenericTypeArguments.FirstOrDefault();

            return argType ?? type;
        }

        public static async Task<object> GetTaskResultIfTask(this object result)
        {
            if (result is Task t)
            {
                await t;

                result = t.GetPropValue("Result");
            }

            return result;
        }
    }
}
