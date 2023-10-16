// <auto-generated>
//   This code file has automatically been added by the "BUTR.CrashReport.Bannerlord.Source" NuGet package (https://www.nuget.org/packages/BUTR.CrashReport.Bannerlord.Source).
//   Please see https://github.com/BUTR/BUTR.CrashReport for more information.
//
//   IMPORTANT:
//   DO NOT DELETE THIS FILE if you are using a "packages.config" file to manage your NuGet references.
//   Consider migrating to PackageReferences instead:
//   https://docs.microsoft.com/en-us/nuget/consume-packages/migrate-packages-config-to-package-reference
//   Migrating brings the following benefits:
//   * The "BUTR.CrashReport.Bannerlord.Source" folder and the "CrashReportShared.cs" file don't appear in your project.
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

#if !BUTRCRASHREPORT_DISABLE || BUTRCRASHREPORT_ENABLE_HTML_RENDERER
#nullable enable
#if !BUTRCRASHREPORT_ENABLEWARNINGS
#pragma warning disable
#endif

namespace BUTR.CrashReport.Bannerlord
{
    using global::BUTR.CrashReport.Models;

    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Reflection;

    internal static class CrashReportShared
    {
        // Inspired by SMAPI's detection
        // Still Work In Progress for more complex capabilities
        public static readonly string[] OSFileSystemTypeReferences = new[]
        {
            typeof(System.IO.File).FullName!,
            typeof(System.IO.FileStream).FullName!,
            typeof(System.IO.FileInfo).FullName!,
            typeof(System.IO.Directory).FullName!,
            typeof(System.IO.DirectoryInfo).FullName!,
            typeof(System.IO.DriveInfo).FullName!,
            typeof(System.IO.FileSystemWatcher).FullName!,
        };
        public static readonly string[] GameFileSystemTypeReferences = new[]
        {
            "TaleWorlds.Library.*File*",
            "TaleWorlds.Library.*Directory*",
            "TaleWorlds.SaveSystem.*File*",
        };
        public static readonly string[] ShellTypeReferences = new[]
        {
            typeof(System.Diagnostics.Process).FullName!,
        };
        public static readonly string[] SaveSystemTypeReferences = new[]
        {
            "TaleWorlds.Library.*Save*",
            "TaleWorlds.Core.MBSaveLoad",
        };
        public static readonly string[] SaveSystemAssemblyReferences = new[]
        {
            "TaleWorlds.SaveSystem",
        };
        public static readonly string[] GameEntitiesTypeReferences = new[]
        {
            "TaleWorlds.Core.EntitySystem*",
        };
        public static readonly string[] GameEntitiesAssemblyReferences = new[]
        {
            "TaleWorlds.ObjectSystem",
        };
        public static readonly string[] InputSystemAssemblyReferences = new[]
        {
            "TaleWorlds.InputSystem",
        };
        public static readonly string[] LocalizationSystemAssemblyReferences = new[]
        {
            "TaleWorlds.Localization",
        };
        public static readonly string[] UITypeReferences = new[]
        {
            "TaleWorlds.Library.IViewModel",
            "TaleWorlds.Library.ViewModel",
        };
        public static readonly string[] UIAssemblyReferences = new[]
        {
            "*GauntletUI*",
        };
        public static readonly string[] HttpTypeReferences = new[]
        {
            "TaleWorlds.Library.*Http*",
            "System.Net*Http.*",
        };
        public static readonly string[] AchievementSystemTypeReferences = new[]
        {
            "TaleWorlds.*Achievement*",
        };
        public static readonly string[] CampaignSystemTypeReferences = new[]
        {
            "TaleWorlds.*CampaignSystem*",
        };
        public static readonly string[] SkillSystemTypeReferences = new[]
        {
            "TaleWorlds.Core.CharacterSkills",
            "TaleWorlds.Core.DefaultSkills",
            "TaleWorlds.Core.SkillObject",
        };
        public static readonly string[] ItemSystemTypeReferences = new[]
        {
            "TaleWorlds.Core.ItemObject",
        };
        public static readonly string[] CultureSystemTypeReferences = new[]
        {
            "TaleWorlds.*Culture*",
        };

        public static IEnumerable<ModuleCapabilities> GetModuleCapabilities(CrashReportModel crashReport, ModuleModel module)
        {
            if (module.ContainsTypeReferences(crashReport, CrashReportShared.OSFileSystemTypeReferences))
                yield return ModuleCapabilities.OSFileSystem;

            if (module.ContainsTypeReferences(crashReport, CrashReportShared.GameFileSystemTypeReferences))
                yield return ModuleCapabilities.GameFileSystem;

            if (module.ContainsTypeReferences(crashReport, CrashReportShared.ShellTypeReferences))
                yield return ModuleCapabilities.Shell;

            if (module.ContainsTypeReferences(crashReport, CrashReportShared.SaveSystemTypeReferences))
                yield return ModuleCapabilities.SaveSystem;
            if (module.ContainsAssemblyReferences(crashReport, CrashReportShared.SaveSystemAssemblyReferences))
                yield return ModuleCapabilities.SaveSystem;

            if (module.ContainsTypeReferences(crashReport, CrashReportShared.GameEntitiesTypeReferences))
                yield return ModuleCapabilities.GameEntities;
            if (module.ContainsAssemblyReferences(crashReport, CrashReportShared.GameEntitiesAssemblyReferences))
                yield return ModuleCapabilities.GameEntities;

            if (module.ContainsAssemblyReferences(crashReport, CrashReportShared.InputSystemAssemblyReferences))
                yield return ModuleCapabilities.InputSystem;

            if (module.ContainsAssemblyReferences(crashReport, CrashReportShared.LocalizationSystemAssemblyReferences))
                yield return ModuleCapabilities.Localization;

            if (module.ContainsTypeReferences(crashReport, CrashReportShared.UITypeReferences))
                yield return ModuleCapabilities.UserInterface;
            if (module.ContainsAssemblyReferences(crashReport, CrashReportShared.UIAssemblyReferences))
                yield return ModuleCapabilities.UserInterface;

            if (module.ContainsTypeReferences(crashReport, CrashReportShared.HttpTypeReferences))
                yield return ModuleCapabilities.Http;

            if (module.ContainsTypeReferences(crashReport, CrashReportShared.AchievementSystemTypeReferences))
                yield return ModuleCapabilities.Achievements;

            if (module.ContainsTypeReferences(crashReport, CrashReportShared.CampaignSystemTypeReferences))
                yield return ModuleCapabilities.Campaign;

            if (module.ContainsTypeReferences(crashReport, CrashReportShared.SkillSystemTypeReferences))
                yield return ModuleCapabilities.Skills;

            if (module.ContainsTypeReferences(crashReport, CrashReportShared.ItemSystemTypeReferences))
                yield return ModuleCapabilities.Items;

            if (module.ContainsTypeReferences(crashReport, CrashReportShared.CultureSystemTypeReferences))
                yield return ModuleCapabilities.Cultures;
        }

        public static string GetBUTRLoaderVersion(CrashReportModel crashReport)
        {
            if (crashReport.Assemblies.FirstOrDefault(x => x.Name == "Bannerlord.BUTRLoader") is { } bAssembly)
                return bAssembly.Version;
            return string.Empty;
        }

        public static string GetLauncherType(CrashReportModel crashReport)
        {
            if (crashReport.AdditionalMetadata.FirstOrDefault(x => x.Key == "METADATA:Parent_Process_Name").Value is { } parentProcessName)
            {
                return parentProcessName switch
                {
                    "Vortex" => "vortex",
                    "BannerLordLauncher" => "bannerlordlauncher",
                    "steam" => "steam",
                    "GalaxyClient" => "gog",
                    "EpicGamesLauncher" => "epicgames",
                    "devenv" => "debuggervisualstudio",
                    "JetBrains.Debugger.Worker64c" => "debuggerjetbrains",
                    "explorer" => "explorer",
                    "NovusLauncher" => "novus",
                    "ModOrganizer" => "modorganizer",
                    _ => $"unknown launcher - {parentProcessName}"
                };
            }

            if (!string.IsNullOrEmpty(GetBUTRLoaderVersion(crashReport)))
                return "butrloader";

            return "vanilla";
        }

        public static string GetLauncherVersion(CrashReportModel crashReport)
        {
            if (crashReport.AdditionalMetadata.FirstOrDefault(x => x.Key == "METADATA:Parent_Process_File_Version").Value is { } parentProcessFileVersion)
                return parentProcessFileVersion;

            if (GetBUTRLoaderVersion(crashReport) is { } bVersion && !string.IsNullOrEmpty(bVersion))
                return bVersion;

            return "0";
        }
    }
}

#pragma warning restore
#nullable restore
#endif // BUTRCRASHREPORT_DISABLE