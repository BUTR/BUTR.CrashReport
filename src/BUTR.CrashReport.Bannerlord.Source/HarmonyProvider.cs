// <auto-generated>
//   This code file has automatically been added by the "BUTR.CrashReport.Bannerlord.Source" NuGet package (https://www.nuget.org/packages/BUTR.CrashReport.Bannerlord.Source).
//   Please see https://github.com/BUTR/BUTR.CrashReport for more information.
//
//   IMPORTANT:
//   DO NOT DELETE THIS FILE if you are using a "packages.config" file to manage your NuGet references.
//   Consider migrating to PackageReferences instead:
//   https://docs.microsoft.com/en-us/nuget/consume-packages/migrate-packages-config-to-package-reference
//   Migrating brings the following benefits:
//   * The "BUTR.CrashReport.Bannerlord.Source" folder and the "CrashReportCreatorHelper.cs" file don't appear in your project.
//   * The added file is immutable and can therefore not be modified by coincidence.
//   * Updating/Uninstalling the package will work flawlessly.
// </auto-generated>

#region License
// MIT License
//
// Copyright (c) Bannerlord's Unofficial Tools & Resources
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

#if !BUTRCRASHREPORT_DISABLE
#nullable enable
#if !BUTRCRASHREPORT_ENABLEWARNINGS
#pragma warning disable
#endif

namespace BUTR.CrashReport.Bannerlord
{
    using global::BUTR.CrashReport.Interfaces;

    using global::HarmonyLib;

    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Linq;
    using global::System.Reflection;
    
    using static global::HarmonyLib.BUTR.Extensions.AccessTools2;

    public class HarmonyProvider : IHarmonyProvider
    {
        private static class MonoModUtils
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
        
        public virtual IEnumerable<MethodBase> GetAllPatchedMethods() => Harmony.GetAllPatchedMethods();

        public virtual global::BUTR.CrashReport.Models.HarmonyPatches? GetPatchInfo(MethodBase originalMethod)
        {
            static global::BUTR.CrashReport.Models.HarmonyPatch Convert(Patch patch, global::BUTR.CrashReport.Models.HarmonyPatchType type) => new()
            {
                Owner = patch.owner,
                Index = patch.index,
                Priority = patch.priority,
                Before = patch.before,
                After = patch.after,
                PatchMethod = patch.PatchMethod,
                Type = type,
            };
        
            var patches = Harmony.GetPatchInfo(originalMethod);
            if (patches is null) return null;
            return new()
            {
                Prefixes = patches.Prefixes.Select(x => Convert(x, Models.HarmonyPatchType.Prefix)).ToArray(),
                Postfixes = patches.Postfixes.Select(x => Convert(x, Models.HarmonyPatchType.Postfix)).ToArray(),
                Finalizers = patches.Finalizers.Select(x => Convert(x, Models.HarmonyPatchType.Finalizer)).ToArray(),
                Transpilers = patches.Transpilers.Select(x => Convert(x, Models.HarmonyPatchType.Transpiler)).ToArray(),
            };
        }

        public virtual MethodBase? GetOriginalMethod(MethodInfo replacement)
        {
            try
            {
                return Harmony.GetOriginalMethod(replacement);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }

        public virtual MethodBase? GetMethodFromStackframe(StackFrame frame)
        {
            try
            {
                return Harmony.GetMethodFromStackframe(frame);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }
        
        public MethodBase? GetIdentifiable(MethodBase method) => MonoModUtils.GetIdentifiable(method);

        public IntPtr GetNativeMethodBody(MethodBase method) => MonoModUtils.GetNativeMethodBody(method);
    }
}

#pragma warning restore
#nullable restore
#endif // BUTRCRASHREPORT_DISABLE