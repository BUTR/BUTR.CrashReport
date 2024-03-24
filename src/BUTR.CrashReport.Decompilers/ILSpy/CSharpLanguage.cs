using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;

namespace BUTR.CrashReport.Decompilers.ILSpy;

internal class CSharpLanguage : Language
{
    private const int _transformCount = int.MaxValue;

    public static CSharpDecompiler CreateDecompiler(PEFile module, DecompilerSettings settings, CancellationToken ct)
    {
        var resolver = new UniversalAssemblyResolver(null, false, module.DetectTargetFrameworkId(), module.DetectRuntimePack());
        var decompiler = new CSharpDecompiler(module, resolver, settings) { CancellationToken = ct };
        while (decompiler.AstTransforms.Count >= _transformCount)
            decompiler.AstTransforms.RemoveAt(decompiler.AstTransforms.Count - 1);
        decompiler.AstTransforms.Add(new EscapeInvalidIdentifiers());
        return decompiler;
    }

    private static void WriteCode(ITextOutput output, DecompilerSettings settings, SyntaxTree syntaxTree, IDecompilerTypeSystem typeSystem)
    {
        syntaxTree.AcceptVisitor(new InsertParenthesesVisitor { InsertParenthesesForReadability = true });
        output.IndentationString = settings.CSharpFormattingOptions.IndentationString;
        TokenWriter tokenWriter = new TextTokenWriter(output, settings, typeSystem);
        syntaxTree.AcceptVisitor(new CSharpOutputVisitor(tokenWriter, settings.CSharpFormattingOptions));
    }

    public override void DecompileMethod(IMethod method, ITextOutput output, DecompilerSettings settings)
    {
        if (method.ParentModule?.PEFile is null)
            return;

        var assembly = method.ParentModule.PEFile;
        var decompiler = CreateDecompiler(assembly, settings, CancellationToken.None);
        WriteCommentLine(output, assembly.FullName);
        WriteCommentLine(output, TypeToString(method.DeclaringType, includeNamespace: true));

        if (decompiler.TypeSystem.MainModule.ResolveEntity(method.MetadataToken) is not IMethod methodDefinition)
            return;

        if (methodDefinition.DeclaringTypeDefinition is not null && methodDefinition.IsConstructor && methodDefinition.DeclaringType.IsReferenceType != false)
        {
            var members = CollectFieldsAndCtors(methodDefinition.DeclaringTypeDefinition, methodDefinition.IsStatic);
            decompiler.AstTransforms.Add(new SelectCtorTransform(methodDefinition));
            WriteCode(output, settings, decompiler.Decompile(members), decompiler.TypeSystem);
        }
        else
        {
            WriteCode(output, settings, decompiler.Decompile(method.MetadataToken), decompiler.TypeSystem);
        }
    }

    private static List<EntityHandle> CollectFieldsAndCtors(ITypeDefinition type, bool isStatic)
    {
        var members = new List<EntityHandle>();
        members.AddRange(type.Fields.Where(field => !field.MetadataToken.IsNil && field.IsStatic == isStatic).Select(field => field.MetadataToken));
        members.AddRange(type.Methods.Where(ctor => !ctor.MetadataToken.IsNil && ctor.IsConstructor && ctor.IsStatic == isStatic).Select(ctor => ctor.MetadataToken));
        return members;
    }

    private static CSharpAmbience CreateAmbience() => new() { ConversionFlags = ConversionFlags.ShowTypeParameterList | ConversionFlags.PlaceReturnTypeAfterParameterList };

    public override string TypeToString(IType type, bool includeNamespace)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        var ambience = CreateAmbience();
        if (includeNamespace)
        {
            ambience.ConversionFlags |= ConversionFlags.UseFullyQualifiedTypeNames;
            ambience.ConversionFlags |= ConversionFlags.UseFullyQualifiedEntityNames;
        }

        if (type is ITypeDefinition definition)
            return ambience.ConvertSymbol(definition);
        // HACK : UnknownType is not supported by CSharpAmbience.

        if (type.Kind == TypeKind.Unknown)
            return (includeNamespace ? type.FullName : type.Name) + (type.TypeParameterCount > 0 ? "<" + string.Join(", ", type.TypeArguments.Select(t => t.Name)) + ">" : "");

        return ambience.ConvertType(type);
    }

    private class SelectCtorTransform : IAstTransform
    {
        private readonly IMethod _ctor;
        private readonly HashSet<ISymbol> _removedSymbols = new();

        public SelectCtorTransform(IMethod ctor) => _ctor = ctor;

        public void Run(AstNode rootNode, TransformContext context)
        {
            ConstructorDeclaration? ctorDecl = null;
            foreach (var node in rootNode.Children)
            {
                switch (node)
                {
                    case ConstructorDeclaration ctor:
                        if (Equals(ctor.GetSymbol(), _ctor))
                        {
                            ctorDecl = ctor;
                        }
                        else
                        {
                            // remove other ctors
                            ctor.Remove();
                            _removedSymbols.Add(ctor.GetSymbol());
                        }
                        break;
                    case FieldDeclaration fd:
                        // Remove any fields without initializers
                        if (fd.Variables.All(v => v.Initializer.IsNull))
                        {
                            fd.Remove();
                            _removedSymbols.Add(fd.GetSymbol());
                        }
                        break;
                }
            }
            if (ctorDecl?.Initializer.ConstructorInitializerType == ConstructorInitializerType.This)
            {
                // remove all fields
                foreach (var node in rootNode.Children)
                {
                    switch (node)
                    {
                        case FieldDeclaration fd:
                            fd.Remove();
                            _removedSymbols.Add(fd.GetSymbol());
                            break;
                    }
                }
            }
            foreach (var node in rootNode.Children)
            {
                if (node is Comment && _removedSymbols.Contains(node.GetSymbol()))
                    node.Remove();
            }
        }
    }
}