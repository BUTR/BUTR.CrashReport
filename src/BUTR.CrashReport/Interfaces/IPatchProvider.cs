using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using BUTR.CrashReport.Models;

namespace BUTR.CrashReport.Interfaces;

public interface IPatchProvider
{
    IList<RuntimePatch> GetAllPatches();
    IList<RuntimePatch> GetPatches(MethodBase originalMethod);
    StackFrameRuntimePatch? GetPatches(StackFrame frame);
    MethodBase GetOriginalMethod(StackFrame frame);
    IntPtr GetNativeMethodBody(MethodBase method);
}