using Microsoft.CodeAnalysis;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BUTR.CrashReport.Native.SourceGenerator;

[Generator]
public class DelegateLoaderGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var assemblyAttributes = context.CompilationProvider.SelectMany(static (compilation, _) =>
        {
            var attributeSymbol = compilation.GetTypeByMetadataName("BUTR.CrashReport.Native.DelegateLoaderAttribute");
            if (attributeSymbol == null) return Enumerable.Empty<(INamedTypeSymbol?, bool)>();

            return compilation.Assembly.GetAttributes()
                .Where(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, attributeSymbol))
                .Select(attr =>
                {
                    var typeToWrap = attr.ConstructorArguments[0].Value as INamedTypeSymbol;
                    var useDelegateTypeName = (bool) (attr.ConstructorArguments[1].Value ?? false);

                    return (typeToWrap, useDelegateTypeName);
                })
                .Where(tuple => tuple.typeToWrap != null);
        });

        context.RegisterSourceOutput(assemblyAttributes, (spc, attr) =>
        {
            var (typeToWrap, useDelegateTypeName) = attr;

            if (typeToWrap == null) return;

            var className = $"{typeToWrap.Name}DelegateLoader";
            var namespaceName = typeToWrap.ContainingNamespace.ToDisplayString();

            var sourceBuilder = new StringBuilder();

            sourceBuilder.AppendLine("using System;");
            sourceBuilder.AppendLine("using System.Runtime.InteropServices;");

            sourceBuilder.AppendLine($"namespace {namespaceName}");
            sourceBuilder.AppendLine("{");

            sourceBuilder.AppendLine($"    public unsafe static class {className}");
            sourceBuilder.AppendLine("    {");

            // Add Load method
            sourceBuilder.AppendLine($"        public static void LoadFrom(this {typeToWrap.Name} origial, Func<string, IntPtr> loadFunctionPointer)");
            sourceBuilder.AppendLine("        {");

            foreach (var fieldSymbol in GetFieldsFromClass(typeToWrap))
            {
                var fieldName = fieldSymbol.Name;
                var delegateType = fieldSymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                var delegateSymbol = fieldSymbol.Type as INamedTypeSymbol;
                if (delegateSymbol == null) continue;

                var entryPoint = useDelegateTypeName ? delegateSymbol.Name : fieldName;

                sourceBuilder.AppendLine($"            origial.{fieldName} = Marshal.GetDelegateForFunctionPointer<{delegateType}>(loadFunctionPointer(\"{entryPoint}\"));");
            }

            sourceBuilder.AppendLine("        }");

            sourceBuilder.AppendLine("    }");
            sourceBuilder.AppendLine("}");

            spc.AddSource($"{className}_Generated.g.cs", sourceBuilder.ToString());
        });
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
               typeSymbol.DelegateInvokeMethod != null &&
               typeSymbol.GetAttributes().Any(attr => attr.AttributeClass?.Name == "UnmanagedFunctionPointerAttribute");
    }
}