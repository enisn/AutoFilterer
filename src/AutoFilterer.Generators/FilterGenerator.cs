using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DotNurse.CodeAnalysis;
using AutoFilterer.Generators.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;

namespace AutoFilterer.Generators;

[Generator]
public class FilterGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new AttributeSyntaxReceiver<GenerateAutoFilterAttribute>());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (!(context.SyntaxReceiver is AttributeSyntaxReceiver<GenerateAutoFilterAttribute> receiver))
            return;

        //if (!Debugger.IsAttached)
        //{
        //    Debugger.Launch();
        //}

        foreach (var classSyntax in receiver.Classes)
        {
            var attribute = classSyntax.AttributeLists.SelectMany(sm => sm.Attributes).FirstOrDefault(x => x.Name.ToString().EnsureEndsWith("Attribute").Equals(typeof(GenerateAutoFilterAttribute).Name));
            var namespaceParam = attribute.ArgumentList?.Arguments.FirstOrDefault(); // Temprorary... Attribute has only one argument for now.

            var model = context.Compilation.GetSemanticModel(classSyntax.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(classSyntax);
            var attrs = symbol.GetAttributes();

            var realNamespace = GetNamespaceRecursively(symbol.ContainingNamespace);

            var properties = symbol.GetMembers().OfType<IPropertySymbol>()
            .Where(x => !x.IsStatic && !x.ContainingType.IsGenericType && x.Kind == SymbolKind.Property);

            context.AddSource($"{symbol.Name}FilterDto.g.cs",
                   SourceText.From(GetFilterDtoCode(symbol.Name, properties, namespaceParam?.ToString().Trim('\"') ?? realNamespace), Encoding.UTF8));
        }
    }

    private static Dictionary<INamedTypeSymbol, IList<AttributeData>> GetClassAttributePairs(GeneratorExecutionContext context,
        SyntaxReceiver receiver)
    {
        var compilation = context.Compilation;
        var classSymbols = new Dictionary<INamedTypeSymbol, IList<AttributeData>>();
        foreach (var clazz in receiver.CandidateClasses)
        {
            var model = compilation.GetSemanticModel(clazz.SyntaxTree);
            var classSymbol = model.GetDeclaredSymbol(clazz);
            var attributes = classSymbol.GetAttributes();
            if (attributes.Any(ad => ad.AttributeClass.Name == nameof(GenerateAutoFilterAttribute)))
            {
                classSymbols.Add((INamedTypeSymbol)classSymbol, attributes.ToList());
            }
        }

        return classSymbols;
    }

    internal string GetFilterDtoCode(string className, IEnumerable<IPropertySymbol> properties,
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
            string propertyType = property.Type.ToDisplayString(NullableFlowState.None);

            if (TypeMapping.Mappings.TryGetValue(propertyType, out var mapped))
                propertyType = mapped;

            if (propertyType.Equals(nameof(String), StringComparison.InvariantCultureIgnoreCase))
            {
                body.AppendLine("\t\t[ToLowerContainsComparison]");
            }
            body.AppendLine($"\t\tpublic virtual {propertyType} {property.Name} {{ get; set; }}");
        }

        var end = "\t}\n}";


        return start + body.ToString() + end;
    }

    private string GetNamespaceRecursively(INamespaceSymbol symbol)
    {
        if (symbol.ContainingNamespace == null)
        {
            return symbol.Name;
        }

        return (GetNamespaceRecursively(symbol.ContainingNamespace) + "." + symbol.Name).Trim('.');
    }
}
