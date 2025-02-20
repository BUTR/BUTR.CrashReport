using BUTR.CrashReport.Models;

using System.Collections.Generic;
using System.Linq;

namespace BUTR.CrashReport.ContextualAnalysis.Utils;

public static class ContextualAnalysisUtils
{
    public static IEnumerable<CrashDiagnosis> AnalyzeCrashReport(CrashReportModel crashReport, IEnumerable<CrashDiagnosis> availableDiagnoses)
    {
        foreach (var crashDiagnosis in availableDiagnoses)
        {
            var criteria = crashDiagnosis.MatchCriteria;

            if (!string.IsNullOrEmpty(criteria.ExceptionType) && crashReport.Exception.Type != criteria.ExceptionType)
                continue;

            if (!string.IsNullOrEmpty(criteria.InvariantMessageContains) && !crashReport.Exception.Message.Contains(criteria.InvariantMessageContains))
                continue;

            if (!string.IsNullOrEmpty(criteria.Source) && crashReport.Exception.Source != criteria.Source)
                continue;

            if (criteria.HResult.HasValue && crashReport.Exception.HResult != criteria.HResult)
                continue;

            if (!string.IsNullOrEmpty(criteria.SourceModuleId) && crashReport.Exception.SourceModuleId != criteria.SourceModuleId)
                continue;

            if (!string.IsNullOrEmpty(criteria.SourceLoaderPluginId) && crashReport.Exception.SourceLoaderPluginId != criteria.SourceLoaderPluginId)
                continue;

            if (criteria.StacktracePatterns is { Length: > 0 })
            {
                if (!MatchesStacktracePatterns(crashReport.EnhancedStacktrace, criteria.StacktracePatterns))
                    continue;
                /*
                var stacktraceEntries = crashReport.Exception.CallStack.Split('\n')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x)) // Remove empty lines
                    .ToArray();

                if (!MatchesStacktracePatterns(stacktraceEntries, criteria.StacktracePatterns))
                    continue;
                */
            }

            if (criteria.AvailableModules is { Length: > 0 })
            {
                if (criteria.AvailableModules.Any(x => crashReport.Modules.All(y => y.Id != x.Id)))
                    continue;
            }

            if (criteria.AvailableLoaderPlugins is { Length: > 0 })
            {
                if (criteria.AvailableLoaderPlugins.Any(x => crashReport.LoaderPlugins.All(y => y.Id != x.Id)))
                    continue;
            }

            yield return crashDiagnosis;
        }
    }

    private static bool MatchesStacktracePatterns(IList<EnhancedStacktraceFrameModel> stacktrace, CrashStacktracePattern[] patterns) => patterns.Select(pattern => pattern.Position switch
    {
        StacktraceMatchPosition.Any => stacktrace.Any(frame => MatchesStacktraceFrame(frame, pattern)),
        StacktraceMatchPosition.AtIndex => pattern.Index is >= 0 && pattern.Index.Value < stacktrace.Count && MatchesStacktraceFrame(stacktrace[pattern.Index.Value], pattern),
        StacktraceMatchPosition.BeforeIndex => pattern.Index is > 0 && stacktrace.Take(pattern.Index.Value).Any(frame => MatchesStacktraceFrame(frame, pattern)),
        StacktraceMatchPosition.AfterIndex => pattern.Index.HasValue && pattern.Index.Value < stacktrace.Count - 1 && stacktrace.Skip(pattern.Index.Value + 1).Any(frame => MatchesStacktraceFrame(frame, pattern)),
        StacktraceMatchPosition.AtStart => stacktrace.Count > 0 && MatchesStacktraceFrame(stacktrace[0], pattern),
        StacktraceMatchPosition.AtEnd => stacktrace.Count > 0 && MatchesStacktraceFrame(stacktrace[stacktrace.Count - 1], pattern),
        _ => false,
    }).All(matched => matched);

    private static bool MatchesStacktraceFrame(EnhancedStacktraceFrameModel frame, CrashStacktracePattern pattern)
    {
        if (!string.IsNullOrEmpty(pattern.Type) && frame.ExecutingMethod.MethodDeclaredTypeName != pattern.Type)
            return false;

        if (!string.IsNullOrEmpty(pattern.Method) && frame.ExecutingMethod.MethodName != pattern.Method)
            return false;

        if (pattern.ArgumentTypes is { Length: > 0 } && (!frame.ExecutingMethod.MethodParameters.Any() || !pattern.ArgumentTypes.SequenceEqual(frame.ExecutingMethod.MethodParameters)))
            return false;

        if (pattern.TypeParameters is { Length: > 0 } && (!frame.ExecutingMethod.MethodTypeArguments.Any() || !pattern.TypeParameters.SequenceEqual(frame.ExecutingMethod.MethodTypeArguments)))
            return false;

        return true;
    }


    private static bool MatchesStacktracePatterns(string[] stacktrace, CrashStacktracePattern[] patterns) => patterns.Select(pattern => pattern.Position switch
    {
        StacktraceMatchPosition.Any => stacktrace.Any(entry => MatchesStacktraceEntry(entry, pattern)),
        StacktraceMatchPosition.AtIndex => pattern.Index is >= 0 && pattern.Index.Value < stacktrace.Length && MatchesStacktraceEntry(stacktrace[pattern.Index.Value], pattern),
        StacktraceMatchPosition.BeforeIndex => pattern.Index is > 0 && stacktrace.Take(pattern.Index.Value).Any(entry => MatchesStacktraceEntry(entry, pattern)),
        StacktraceMatchPosition.AfterIndex => pattern.Index.HasValue && pattern.Index.Value < stacktrace.Length - 1 && stacktrace.Skip(pattern.Index.Value + 1).Any(entry => MatchesStacktraceEntry(entry, pattern)),
        StacktraceMatchPosition.AtStart => stacktrace.Length > 0 && MatchesStacktraceEntry(stacktrace[0], pattern),
        StacktraceMatchPosition.AtEnd => stacktrace.Length > 0 && MatchesStacktraceEntry(stacktrace[stacktrace.Length - 1], pattern),
        _ => false,
    }).All(matched => matched);

    private static bool MatchesStacktraceEntry(string entry, CrashStacktracePattern pattern)
    {
        if (!string.IsNullOrEmpty(pattern.Type) && !entry.Contains(pattern.Type!))
            return false;

        if (!string.IsNullOrEmpty(pattern.Method) && !entry.Contains(pattern.Method!))
            return false;

        if (pattern.ArgumentTypes is { Length: > 0 } && !pattern.ArgumentTypes.All(entry.Contains))
            return false;

        if (pattern.TypeParameters is { Length: > 0 } && !pattern.TypeParameters.All(entry.Contains))
            return false;

        return true;
    }
}