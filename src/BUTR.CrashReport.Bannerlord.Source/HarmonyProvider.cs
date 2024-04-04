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
    using global::Bannerlord.BUTR.Shared.Extensions;
    using global::Bannerlord.BUTR.Shared.Helpers;
    using global::Bannerlord.ModuleManager;

    using global::BUTR.CrashReport.Extensions;
    using global::BUTR.CrashReport.Interfaces;
    using global::BUTR.CrashReport.Models;
    using global::BUTR.CrashReport.Utils;

    using global::HarmonyLib;
    using global::HarmonyLib.BUTR.Extensions;

    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Globalization;
    using global::System.IO;
    using global::System.Linq;
    using global::System.Reflection;
    using global::System.Security.Cryptography;

    public class HarmonyProvider : IHarmonyProvider
    {
        public virtual IEnumerable<MethodBase> GetAllPatchedMethods() => Harmony.GetAllPatchedMethods();

        public virtual global::BUTR.CrashReport.Models.HarmonyPatches GetPatchInfo(MethodBase originalMethod)
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
    }
}

#pragma warning restore
#nullable restore
#endif // BUTRCRASHREPORT_DISABLE