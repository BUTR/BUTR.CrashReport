using System;
using System.Reflection;
using System.Text;

namespace BUTR.CrashReport.Utils;

internal static class TypeUtils
{
    public static Type? GetReturnedType(MethodBase? methodOrConstructor) => methodOrConstructor switch
    {
        ConstructorInfo => typeof(void),
        MethodInfo method => method.ReturnType,
        _ => null,
    };

    public static StringBuilder FullDescription(this Type? type)
    {
        var sb = new StringBuilder();
        if (type is null)
        {
            sb.Append("null");
            return sb;
        }

        if (!string.IsNullOrEmpty(type.Namespace))
            sb.Append(type.Namespace).Append('.');
        sb.Append(type.Name);
        if (type.IsGenericType)
        {
            sb.Append('<');
            var subTypes = type.GetGenericArguments();
            for (var i = 0; i < subTypes.Length; i++)
            {
                if (sb[sb.Length - 1] != '<')
                    sb.Append(", ");
                sb.Append(FullDescription(subTypes[i]));
            }
            sb.Append('>');
        }
        return sb;
    }

    public static StringBuilder FullDescription(this MethodBase? member)
    {
        var sb = new StringBuilder();
        if (member is null)
        {
            sb.Append("null");
            return sb;
        }

        var returnType = GetReturnedType(member);

        var result = new StringBuilder();
        if (member.IsStatic) _ = result.Append("static ");
        if (member.IsAbstract) _ = result.Append("abstract ");
        if (member.IsVirtual) _ = result.Append("virtual ");
        result.Append(FullDescription(returnType)).Append(' ');
        if (member.DeclaringType is not null)
            result.Append(FullDescription(member.DeclaringType)).Append("::");
        result.Append(member.Name).Append('(');
        var parameters = member.GetParameters();
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            result.Append(FullDescription(parameter.ParameterType)).Append(' ').Append(parameter.Name);
            if (i < parameters.Length - 1) _ = result.Append(", ");
        }
        result.Append(')');
        return result;
    }
}