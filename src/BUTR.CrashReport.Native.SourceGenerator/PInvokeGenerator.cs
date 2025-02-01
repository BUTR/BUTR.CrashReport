using Microsoft.CodeAnalysis;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BUTR.CrashReport.Native.SourceGenerator;

[Generator]
public class PInvokeGenerator : IIncrementalGenerator
{
    private const string AttributeName = "BUTR.CrashReport.Native.PInvokeDelegateLoaderAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var assemblyAttributes = context.CompilationProvider.SelectMany(static (compilation, _) =>
        {
            var attributeSymbol = compilation.GetTypeByMetadataName(AttributeName);
            if (attributeSymbol == null) return Enumerable.Empty<(INamedTypeSymbol?, string?, bool)>();

            return compilation.Assembly.GetAttributes()
                .Where(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, attributeSymbol))
                .Select(attr =>
                {
                    var typeToWrap = attr.ConstructorArguments[0].Value as INamedTypeSymbol;
                    var nativeLibName = attr.ConstructorArguments[1].Value as string;
                    var useDelegateTypeName = (bool) (attr.ConstructorArguments[2].Value ?? false);

                    return (typeToWrap, nativeLibName, useDelegateTypeName);
                })
                .Where(tuple => tuple.typeToWrap != null && !string.IsNullOrEmpty(tuple.nativeLibName));
        });

        var classAttributes = context.SyntaxProvider.ForAttributeWithMetadataName(
                AttributeName,
                static (node, _) => node is Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax,
                static (context, _) =>
                {
                    var attribute = context.Attributes.FirstOrDefault();
                    if (attribute == null) return default;

                    var classSymbol = (INamedTypeSymbol) context.TargetSymbol;
                    var typeToWrap = classSymbol;
                    var nativeLibName = attribute.ConstructorArguments[0].Value as string;
                    var useDelegateTypeName = (bool) (attribute.ConstructorArguments[1].Value ?? false);

                    return (typeToWrap, nativeLibName, useDelegateTypeName);
                })
            .Where(tuple => tuple.typeToWrap != null && !string.IsNullOrEmpty(tuple.nativeLibName));

        void Action(SourceProductionContext spc, (INamedTypeSymbol?, string?, bool) attr)
        {
            var (typeToWrap, nativeLibName, useDelegateTypeName) = attr;

            if (typeToWrap == null || string.IsNullOrEmpty(nativeLibName)) return;

            var className = $"{typeToWrap.Name}PInvoke";
            var namespaceName = typeToWrap.ContainingNamespace.ToDisplayString();

            var sourceBuilder = new StringBuilder();

            sourceBuilder.AppendLine("using System;");
            sourceBuilder.AppendLine("using System.Runtime.InteropServices;");

            sourceBuilder.AppendLine($"namespace {namespaceName}");
            sourceBuilder.AppendLine("{");

            sourceBuilder.AppendLine($"    internal unsafe static class {className}");
            sourceBuilder.AppendLine("    {");

            sourceBuilder.AppendLine($"        private const string NativeLibName = \"{nativeLibName}\";");

            foreach (var fieldSymbol in GetFieldsFromClass(typeToWrap))
            {
                var fieldName = fieldSymbol.Name;
                if (fieldSymbol.Type is not INamedTypeSymbol delegateSymbol) continue;

                var methodReturn = GetMethodReturn(delegateSymbol);
                var methodParameters = GetMethodParameters(delegateSymbol);

                var entryPoint = useDelegateTypeName ? delegateSymbol.Name : fieldName;

                sourceBuilder.AppendLine($"#if NET7_0_OR_GREATER_");
                sourceBuilder.AppendLine($"        [LibraryImport(\"{nativeLibName}\", EntryPoint = \"{entryPoint}\", StringMarshalling = StringMarshalling.Utf8)]");
                sourceBuilder.AppendLine($"        private static partial {methodReturn} {fieldName}{methodParameters};");
                sourceBuilder.AppendLine($"#else");
                sourceBuilder.AppendLine($"        [DllImport(\"{nativeLibName}\", EntryPoint = \"{entryPoint}\", CallingConvention = CallingConvention.Cdecl)]");
                sourceBuilder.AppendLine($"        private static extern {methodReturn} {fieldName}{methodParameters};");
                sourceBuilder.AppendLine($"#endif");
                sourceBuilder.AppendLine();
            }

            sourceBuilder.AppendLine($"        public static void LoadFromPInvoke(this {typeToWrap.Name} origial)");
            sourceBuilder.AppendLine($"        {{");
            foreach (var fieldSymbol in GetFieldsFromClass(typeToWrap))
            {
                var fieldName = fieldSymbol.Name;

                sourceBuilder.AppendLine($"            origial.{fieldName} = {fieldName};");
            }

            sourceBuilder.AppendLine($"        }}");

            sourceBuilder.AppendLine("    }");
            sourceBuilder.AppendLine("}");

            spc.AddSource($"{className}_Generated.g.cs", sourceBuilder.ToString());
        }

        context.RegisterSourceOutput(assemblyAttributes, Action);
        context.RegisterSourceOutput(classAttributes, Action);
    }

    private static IEnumerable<IFieldSymbol> GetFieldsFromClass(INamedTypeSymbol classSymbol)
    {
        foreach (var fieldSymbol in classSymbol.GetMembers().OfType<IFieldSymbol>())
        {
            if (fieldSymbol.Type is not INamedTypeSymbol typeSymbol) continue;

            if (IsDelegateWithUnmanagedFunctionPointer(typeSymbol))
                yield return fieldSymbol;
        }
    }

    private static bool IsDelegateWithUnmanagedFunctionPointer(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.TypeKind == TypeKind.Delegate &&
               typeSymbol.DelegateInvokeMethod is not null &&
               typeSymbol.GetAttributes().Any(attr => attr.AttributeClass?.Name == "UnmanagedFunctionPointerAttribute");
    }

    private static string GetMethodReturn(INamedTypeSymbol delegateSymbol)
    {
        if (delegateSymbol.DelegateInvokeMethod == null)
            return "void";

        return delegateSymbol.DelegateInvokeMethod.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }

    private static string GetMethodParameters(INamedTypeSymbol delegateSymbol)
    {
        if (delegateSymbol.DelegateInvokeMethod == null)
            return "()";

        var parameters = delegateSymbol.DelegateInvokeMethod.Parameters
            .Select(p => $"{p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} {p.Name}")
            .ToArray();

        return $"({string.Join(", ", parameters)})";
    }
}