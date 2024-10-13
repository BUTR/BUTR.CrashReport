using System;
using System.Diagnostics;
using System.Reflection;

namespace BUTR.CrashReport.Interfaces;

public interface ICommonProvider
{
    /// <summary>
    /// Returns the JIT compiled (native) start address of a method.
    /// </summary>
    IntPtr GetNativeMethodBody(MethodBase method);
}