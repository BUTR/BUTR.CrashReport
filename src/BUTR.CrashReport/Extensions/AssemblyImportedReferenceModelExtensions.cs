﻿using BUTR.CrashReport.Decompilers.Utils;
using BUTR.CrashReport.Models;

using System;
using System.Globalization;
using System.Reflection;

namespace BUTR.CrashReport.Extensions;

/// <summary>
/// Extensions for <inheritdoc cref="BUTR.CrashReport.Models.AssemblyImportedReferenceModel"/>
/// </summary>
public static class AssemblyImportedReferenceModelExtensions
{
    /// <summary>
    /// <inheritdoc cref="System.Reflection.AssemblyName.FullName"/>
    /// </summary>
    /// <returns><inheritdoc cref="System.Reflection.AssemblyName.FullName"/></returns>
    public static string GetFullName(this AssemblyImportedReferenceModel model) =>
        AssemblyNameFormatter.ComputeDisplayName(model.Name, model.Version, model.Culture, model.PublicKeyToken);

    /// <summary>
    /// Creates the model for an assembly name.
    /// </summary>
    public static AssemblyImportedReferenceModel Create(AssemblyName assemblyName) => new()
    {
        Name = assemblyName.Name,
        Version = AssemblyNameFormatter.GetVersion(assemblyName.Version),
        Culture = assemblyName.CultureName,
        PublicKeyToken = string.Join(string.Empty, Array.ConvertAll(assemblyName.GetPublicKeyToken(), x => x.ToString("x2", CultureInfo.InvariantCulture))),
    };
}