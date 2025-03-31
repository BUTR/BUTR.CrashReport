using Microsoft.CodeAnalysis;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BUTR.CrashReport.Native.SourceGenerator;

[Generator]
public class DelegateLoaderGenerator : IIncrementalGenerator
{
    private const string AttributeName = "BUTR.CrashReport.Native.DelegateLoaderAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var assemblyAttributes = context.CompilationProvider.SelectMany(static (compilation, _) =>
        {
            var attributeSymbol = compilation.GetTypeByMetadataName(AttributeName);
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

        var classAttributes = context.SyntaxProvider.ForAttributeWithMetadataName(
                AttributeName,
                static (node, _) => node is Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax,
                static (context, _) =>
                {
                    var attribute = context.Attributes.FirstOrDefault();
                    if (attribute == null) return default;

                    var typeToWrap = attribute.ConstructorArguments[0].Value as INamedTypeSymbol;
                    var useDelegateTypeName = (bool) (attribute.ConstructorArguments[1].Value ?? false);

                    return (typeToWrap, useDelegateTypeName);
                })
            .Where(tuple => tuple.typeToWrap != null);

        void Action(SourceProductionContext spc, (INamedTypeSymbol?, bool) attr)
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

            sourceBuilder.AppendLine($"    internal unsafe static class {className}");
            sourceBuilder.AppendLine("    {");

            // Add Load method
            sourceBuilder.AppendLine($"        public static void LoadFrom(this {typeToWrap.Name} origial, Func<string, IntPtr> loadFunctionPointer)");
            sourceBuilder.AppendLine("        {");

            foreach (var fieldSymbol in GetFieldsFromClass(typeToWrap))
            {
                var fieldName = fieldSymbol.Name;
                var delegateType = fieldSymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                if (fieldSymbol.Type is not INamedTypeSymbol delegateSymbol) continue;

                var entryPoint = useDelegateTypeName ? delegateSymbol.Name : fieldName;

                sourceBuilder.AppendLine($"            origial.{fieldName} = Marshal.GetDelegateForFunctionPointer<{delegateType}>(loadFunctionPointer(\"{entryPoint}\"));");
            }

            sourceBuilder.AppendLine("        }");

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
        return typeSymbol is { TypeKind: TypeKind.Delegate, DelegateInvokeMethod: not null } &&
               typeSymbol.GetAttributes().Any(attr => attr.AttributeClass?.Name == "UnmanagedFunctionPointerAttribute");
    }
}