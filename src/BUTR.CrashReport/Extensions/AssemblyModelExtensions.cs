using BUTR.CrashReport.Models;
using BUTR.CrashReport.Utils;

namespace BUTR.CrashReport.Extensions;

/// <summary>
/// Extensions for <inheritdoc cref="BUTR.CrashReport.Models.AssemblyModel"/>
/// </summary>
public static class AssemblyModelExtensions
{
    /// <summary>
    /// <inheritdoc cref="System.Reflection.AssemblyName.FullName"/>
    /// </summary>
    /// <returns><inheritdoc cref="System.Reflection.AssemblyName.FullName"/></returns>
    public static string GetFullName(this AssemblyModel model) =>
        AssemblyNameFormatter.ComputeDisplayName(model.Name, model.Version, model.Culture, model.PublicKeyToken);
}