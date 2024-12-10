// ReSharper disable RedundantUsingDirective.Global
global using GLboolean = byte;
global using GLbyte = sbyte;
global using GLubyte = byte;
global using GLshort = short;
global using GLushort = ushort;
global using GLint = int;
global using GLuint = uint;
// GLfixed
global using GLint64 = long;
global using GLuint64 = ulong;
global using GLsizei = uint;
global using GLenum = int;
global using GLintptr = nint;
global using GLsizeiptr = nuint;
global using GLsync = nint;
global using GLbitfield = uint;
// GLhalf
global using GLfloat = float;
global using GLclampf = float;
global using GLdouble = double;
global using GLclampd = double;
global using GLchar = byte;

global using UnusedSB = sbyte;
global using UnusedS = short;
global using UnusedUS = ushort;
global using UnusedU = nuint;
global using UnusedL = long;
global using UnusedUL = ulong;
// ReSharper restore RedundantUsingDirective.Global

#if NET6_0_OR_GREATER
global using IntPtrByte = BUTR.CrashReport.Native.Pointer<byte>;
global using IntPtrVoid = BUTR.CrashReport.Native.Pointer;
#else
global using IntPtrByte = BUTR.CrashReport.Native.Pointer;
global using IntPtrVoid = BUTR.CrashReport.Native.Pointer;
#endif