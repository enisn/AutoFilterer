using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoFilterer.Generators;

[Generator]
public class FilterGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
            return;

        var classAttributePairs = GetClassAttributePairs(context, receiver);
        foreach (var classAttributePair in classAttributePairs)
        {
            var classSymbol = classAttributePair.Key;

            var properties = classSymbol.GetMembers().OfType<IPropertySymbol>()
                .Where(x => !x.IsStatic && !x.ContainingType.IsGenericType && x.Kind == SymbolKind.Property);

            var realNamespace = GetNamespaceRecursively(classSymbol.ContainingNamespace);

            foreach (var attr in classAttributePair.Value)
            {
                var param = attr.ConstructorArguments.FirstOrDefault(); // Temprorary... Attribute has only one argument for now.
                context.AddSource($"{classSymbol.Name}FilterDto.g.cs",
                    SourceText.From(GetFilterDtoCode(classSymbol.Name, properties, param.Value?.ToString() ?? realNamespace), Encoding.UTF8));
            }
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
