using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace BUTR.CrashReport.ImGui.Utils;

internal static class FieldAccessor
{
    public static Func<T, TField> CompileFieldGetter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicFields)] T, TField>(string fieldName)
    {
        var fieldInfo = typeof(T).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (fieldInfo == null)
            throw new ArgumentException($"Field '{fieldName}' does not exist on type '{typeof(T)}'.");

        var parameter = Expression.Parameter(typeof(T), "instance");
        var fieldAccess = Expression.Field(parameter, fieldInfo);
        var lambda = Expression.Lambda<Func<T, TField>>(fieldAccess, parameter);
        return lambda.Compile();
    }
}