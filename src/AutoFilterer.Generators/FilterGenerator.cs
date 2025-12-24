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
                predicate: static (s, _) => s is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null);

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private static ClassDeclarationSyntax GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        foreach (var attributeList in classDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                var symbolInfo = context.SemanticModel.GetSymbolInfo(attribute);
                var attributeSymbol = symbolInfo.Symbol as IMethodSymbol;

                var fullName = attributeSymbol?.ContainingType.ToDisplayString()
                                  ?? attribute.Name.ToString();

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

        foreach (var classSyntax in classes)
        {
            var model = compilation.GetSemanticModel(classSyntax.SyntaxTree);
            if (model.GetDeclaredSymbol(classSyntax) is not { } symbol)
            {
                continue;
            }

            var attribute = symbol.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.Name == nameof(GenerateAutoFilterAttribute));
            var namespaceParam = attribute?.ConstructorArguments.FirstOrDefault().Value?.ToString().Trim('\"'); // Temprorary... Attribute has only one argument for now.
            var realNamespace = GetNamespaceRecursively(symbol.ContainingNamespace);

            var properties = symbol.GetMembers().OfType<IPropertySymbol>()
                .Where(x => !x.IsStatic && !x.ContainingType.IsGenericType && x.Kind == SymbolKind.Property);

            var sourceCode = GetFilterDtoCode(symbol.Name, properties, namespaceParam ?? realNamespace);

            context.AddSource($"{symbol.Name}FilterDto.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
        }
    }

    private static string GetFilterDtoCode(string className, IEnumerable<IPropertySymbol> properties,
        string @namespace = null)
    {
        var start = $@"
using System;
using AutoFilterer;
using AutoFilterer.Attributes;
using AutoFilterer.Types;

namespace {@namespace ?? "AutoFilterer.Filters"}
{{
    public partial class {className}Filter : PaginationFilterBase
    {{
";

        var body = new StringBuilder();

        foreach (var property in properties)
        {
            var propertyType = property.Type.ToDisplayString(NullableFlowState.None);

            if (TypeMapping.Mappings.TryGetValue(propertyType, out var mapped)) {
                propertyType = mapped;
            }

            if (propertyType.Equals(nameof(String), StringComparison.InvariantCultureIgnoreCase))
            {
                body.AppendLine("\t\t[ToLowerContainsComparison]");
            }
            body.AppendLine($"\t\tpublic virtual {propertyType} {property.Name} {{ get; set; }}");
        }

        return start + body + "\t}\n}";
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