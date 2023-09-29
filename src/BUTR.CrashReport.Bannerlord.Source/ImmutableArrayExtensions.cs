﻿// <auto-generated>
//   This code file has automatically been added by the "BUTR.CrashReport.Bannerlord.Source" NuGet package (https://www.nuget.org/packages/BUTR.CrashReport.Bannerlord.Source).
//   Please see https://github.com/BUTR/BUTR.CrashReport for more information.
//
//   IMPORTANT:
//   DO NOT DELETE THIS FILE if you are using a "packages.config" file to manage your NuGet references.
//   Consider migrating to PackageReferences instead:
//   https://docs.microsoft.com/en-us/nuget/consume-packages/migrate-packages-config-to-package-reference
//   Migrating brings the following benefits:
//   * The "BUTR.CrashReport.Bannerlord.Source" folder and the "ImmutableArrayExtensions.cs" file don't appear in your project.
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
    using global::System.Collections.Immutable;
    using global::System.Runtime.CompilerServices;

    public static class ImmutableArrayExtensions
    {
        public static T[] AsArray<T>(this ImmutableArray<T> immutableArray) => Unsafe.As<ImmutableArray<T>, T[]>(ref immutableArray);
        public static ImmutableArray<T> AsImmutableArray<T>(this T[] array) => Unsafe.As<T[], ImmutableArray<T>>(ref array);
    }
}

#pragma warning restore
#nullable restore
#endif // BUTRCRASHREPORT_DISABLE