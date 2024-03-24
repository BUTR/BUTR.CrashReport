using System;
using System.Diagnostics;
using System.Reflection;

using static HarmonyLib.BUTR.Extensions.AccessTools2;

namespace BUTR.CrashReport.Decompilers.Utils;

/// <summary>
/// Wrapper for MonoMod methods, since MonoMod.Core is not a dependency for us
/// </summary>
public static class MonoModUtils
{
    private delegate object GetCurrentRuntimeDelegate();
    private static readonly GetCurrentRuntimeDelegate? CurrentRuntimeMethod = GetPropertyGetterDelegate<GetCurrentRuntimeDelegate>(
        "MonoMod.RuntimeDetour.DetourHelper:Runtime", logErrorInTrace: false);

    private delegate MethodBase GetIdentifiableOldDelegate(object instance, MethodBase method);
    private static readonly GetIdentifiableOldDelegate? GetIdentifiableOldMethod = GetDelegate<GetIdentifiableOldDelegate>(
        "MonoMod.RuntimeDetour.IDetourRuntimePlatform:GetIdentifiable", logErrorInTrace: false);

    private delegate IntPtr GetNativeStartDelegate(object instance, MethodBase method);
    private static readonly GetNativeStartDelegate? GetNativeStartMethod = GetDelegate<GetNativeStartDelegate>(
        "MonoMod.RuntimeDetour.IDetourRuntimePlatform:GetNativeStart", logErrorInTrace: false);


    private delegate object GetCurrentPlatformTripleDelegate();
    private static readonly GetCurrentPlatformTripleDelegate? CurrentPlatformTripleMethod = GetPropertyGetterDelegate<GetCurrentPlatformTripleDelegate>(
        "MonoMod.Core.Platforms.PlatformTriple:Current", logErrorInTrace: false);

    private delegate MethodBase GetIdentifiableDelegate(object instance, MethodBase method);
    private static readonly GetIdentifiableDelegate? GetIdentifiableMethod = GetDelegate<GetIdentifiableDelegate>(
        "MonoMod.Core.Platforms.PlatformTriple:GetIdentifiable", logErrorInTrace: false);

    private delegate IntPtr GetNativeMethodBodyDelegate(object instance, MethodBase method);
    private static readonly GetNativeMethodBodyDelegate? GetNativeMethodBodyMethod = GetDelegate<GetNativeMethodBodyDelegate>(
        "MonoMod.Core.Platforms.PlatformTriple:GetNativeMethodBody", logErrorInTrace: false);

    /// <summary>
    /// <inheritdoc cref="MonoMod.Core.Platforms.PlatformTriple.GetIdentifiable"/>
    /// </summary>
    /// <returns><inheritdoc cref="MonoMod.Core.Platforms.PlatformTriple.GetIdentifiable"/></returns>
    public static MethodBase? GetIdentifiable(MethodBase method)
    {
        try
        {
            if (CurrentRuntimeMethod?.Invoke() is { } runtime)
                return GetIdentifiableOldMethod?.Invoke(runtime, method);

            if (CurrentPlatformTripleMethod?.Invoke() is { } platformTriple)
                return GetIdentifiableMethod?.Invoke(platformTriple, method);
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
        }

        return null;
    }

    /// <summary>
    /// <inheritdoc cref="MonoMod.Core.Platforms.PlatformTriple.GetNativeMethodBody"/>
    /// </summary>
    /// <returns><inheritdoc cref="MonoMod.Core.Platforms.PlatformTriple.GetNativeMethodBody"/></returns>
    public static IntPtr GetNativeMethodBody(MethodBase method)
    {
        try
        {
            if (CurrentRuntimeMethod?.Invoke() is { } runtine)
                return GetNativeStartMethod?.Invoke(runtine, method) ?? IntPtr.Zero;

            if (CurrentPlatformTripleMethod?.Invoke() is { } platformTriple)
                return GetNativeMethodBodyMethod?.Invoke(platformTriple, method) ?? IntPtr.Zero;
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
        }

        return IntPtr.Zero;
    }
}