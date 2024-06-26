﻿using System;

namespace BUTR.CrashReport.Bannerlord.Parser.Extensions;

internal static class StringExtensions
{
    public static string[] Split(this string str, string separator) => str.Split([separator], StringSplitOptions.None);

    public static string[] Split(this string str, string separator, StringSplitOptions stringSplitOptions) => str.Split([separator], stringSplitOptions);
}