using HarmonyLib.BUTR.Extensions;

using System;
using System.Reflection;

namespace BUTR.CrashReport.Utils;

public static class MonoModUtils
{
    public delegate object GetCurrentPlatformTripleDelegate();
    public static readonly GetCurrentPlatformTripleDelegate? CurrentPlatformTriple =
        AccessTools2.GetPropertyGetterDelegate<GetCurrentPlatformTripleDelegate>("MonoMod.Core.Platforms.PlatformTriple:Current");

    public delegate MethodBase GetIdentifiableDelegate(object instance, MethodBase method);
    public static readonly GetIdentifiableDelegate? GetIdentifiable =
        AccessTools2.GetDelegate<GetIdentifiableDelegate>("MonoMod.Core.Platforms.PlatformTriple:GetIdentifiable");

    public delegate IntPtr GetNativeMethodBodyDelegate(object instance, MethodBase method);
    public static readonly GetNativeMethodBodyDelegate? GetNativeMethodBody =
        AccessTools2.GetDelegate<GetNativeMethodBodyDelegate>("MonoMod.Core.Platforms.PlatformTriple:GetNativeMethodBody");
}