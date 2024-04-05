using BUTR.CrashReport.Models;
using BUTR.CrashReport.Renderer.Html.Utils;

namespace BUTR.CrashReport.Renderer.Html.Extensions;

/// <summary>
/// Extensions for <inheritdoc cref="BUTR.CrashReport.Models.AssemblyModel"/>
/// </summary>
internal static class AssemblyModelExtensions
{
    /// <summary>
    /// <inheritdoc cref="System.Reflection.AssemblyName.FullName"/>
    /// </summary>
    /// <returns><inheritdoc cref="System.Reflection.AssemblyName.FullName"/></returns>
    public static string GetFullName(this AssemblyModel model) =>
        AssemblyNameFormatter.ComputeDisplayName(model.Id.Name, model.Id.Version, model.CultureName, model.Id.PublicKeyToken);
}