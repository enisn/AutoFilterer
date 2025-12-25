using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace AutoFilterer.Generators;

using Extensions;

[Generator]
public class FilterGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null);

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        if (node is not ClassDeclarationSyntax classDeclaration || classDeclaration.AttributeLists.Count == 0) {
            return false;
        }

        return classDeclaration.AttributeLists
            .SelectMany(al => al.Attributes)
            .Any(a => a.Name.ToString().EnsureEndsWith("Attribute").EndsWith(nameof(GenerateAutoFilterAttribute)));
    }

    private static ClassDeclarationSyntax GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        foreach (var attributeList in classDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                var attributeSymbol = context.SemanticModel.GetSymbolInfo(attribute).Symbol as IMethodSymbol;
                var fullName = attributeSymbol?.ContainingType.ToDisplayString() ?? attribute.Name.ToString();
                if (fullName.EnsureEndsWith("Attribute").EndsWith(nameof(GenerateAutoFilterAttribute)))
                {
                    return classDeclaration;
                }
            }
        }

        return null;
    }

    private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
    {
        if (classes.IsDefaultOrEmpty) {
            return;
        }

        foreach (var classSyntax in classes.Distinct())
        {
            var model = compilation.GetSemanticModel(classSyntax.SyntaxTree);
            if (model.GetDeclaredSymbol(classSyntax) is not { } symbol)
            {
                continue;
            }

            var attribute = symbol.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.Name == nameof(GenerateAutoFilterAttribute));
            var targetNamespace = attribute?.ConstructorArguments.FirstOrDefault().Value?.ToString().Trim('\"'); // Temporary... Attribute has only one argument for now.
            if (string.IsNullOrEmpty(targetNamespace)) {
                targetNamespace = GetNamespaceRecursively(symbol.ContainingNamespace);
            }

            var properties = symbol.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(x => !x.IsStatic && !x.IsIndexer && x.Kind == SymbolKind.Property);

            var sourceCode = GetFilterDtoCode(symbol.Name, properties, targetNamespace);

            context.AddSource($"{symbol.Name}FilterDto.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
        }
    }

    private static string GetFilterDtoCode(string className, IEnumerable<IPropertySymbol> properties,
        string @namespace = null)
    {
        var generatedCode = new StringBuilder();
        generatedCode.AppendLine("using System;");
        generatedCode.AppendLine("using AutoFilterer.Attributes;");
        generatedCode.AppendLine("using AutoFilterer.Types;");
        generatedCode.AppendLine();
        generatedCode.AppendLine($"namespace {@namespace}");
        generatedCode.AppendLine("{");
        generatedCode.AppendLine($"\tpublic partial class {className}Filter : PaginationFilterBase");
        generatedCode.AppendLine("\t{");

        foreach (var property in properties)
        {
            var propertyType = property.Type.ToDisplayString(NullableFlowState.None);

            if (TypeMapping.Mappings.TryGetValue(propertyType, out var mapped)) {
                propertyType = mapped;
            }

            if (property.Type.SpecialType == SpecialType.System_String)
            {
                generatedCode.AppendLine("\t\t[ToLowerContainsComparison]");
            }

            generatedCode.AppendLine($"\t\tpublic virtual {propertyType} {property.Name} {{ get; set; }}");
        }

        generatedCode.AppendLine("\t}");
        generatedCode.AppendLine("}");

        return generatedCode.ToString();
    }

    private static string GetNamespaceRecursively(INamespaceSymbol symbol)
    {
        if (symbol.ContainingNamespace == null)
        {
            return symbol.Name;
        }

        return (GetNamespaceRecursively(symbol.ContainingNamespace) + "." + symbol.Name).Trim('.');
    }
}