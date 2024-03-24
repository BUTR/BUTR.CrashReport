using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BUTR.CrashReport.Utils;

internal static class HarmonyUtils
{
    public static Type? GetReturnedType(MethodBase? methodOrConstructor) => methodOrConstructor switch
    {
        ConstructorInfo => typeof(void),
        MethodInfo method => method.ReturnType,
        _ => null
    };

    public static string Join<T>(this IEnumerable<T> enumeration, Func<T, string>? converter = null, string delimiter = ", ")
    {
        converter ??= t => t!.ToString();
        return enumeration.Aggregate("", (prev, curr) => prev + (prev.Length > 0 ? delimiter : "") + converter(curr));
    }

    public static string FullDescription(this Type? type)
    {
        if (type is null)
            return "null";

        var ns = type.Namespace;
        if (string.IsNullOrEmpty(ns) is false) ns += ".";
        var result = ns + type.Name;

        if (type.IsGenericType)
        {
            result += "<";
            var subTypes = type.GetGenericArguments();
            for (var i = 0; i < subTypes.Length; i++)
            {
                if (!result.EndsWith("<", StringComparison.Ordinal))
                    result += ", ";
                result += subTypes[i].FullDescription();
            }
            result += ">";
        }
        return result;
    }

    public static string FullDescription(this MethodBase? member)
    {
        if (member is null) return "null";
        var returnType = GetReturnedType(member);

        var result = new StringBuilder();
        if (member.IsStatic) _ = result.Append("static ");
        if (member.IsAbstract) _ = result.Append("abstract ");
        if (member.IsVirtual) _ = result.Append("virtual ");
        _ = result.Append($"{returnType.FullDescription()} ");
        if (member.DeclaringType is not null)
            _ = result.Append($"{member.DeclaringType.FullDescription()}::");
        var parameterString = member.GetParameters().Join(p => $"{p.ParameterType.FullDescription()} {p.Name}");
        _ = result.Append($"{member.Name}({parameterString})");
        return result.ToString();
    }
}