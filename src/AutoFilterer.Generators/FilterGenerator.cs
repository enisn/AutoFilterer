using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Generators
{
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

            var classSymbols = GetClassSymbols(context, receiver);
            foreach (var classSymbol in classSymbols)
            {
                var properties = classSymbol.GetMembers().OfType<IPropertySymbol>()
                    .Where(x => !x.IsStatic && !x.ContainingType.IsGenericType && x.Kind == SymbolKind.Property);

                context.AddSource($"{classSymbol.Name}FilterDto.g.cs",
                    SourceText.From(GetFilterDtoCode(classSymbol.Name, properties), Encoding.UTF8));
            }
        }

        private static List<INamedTypeSymbol> GetClassSymbols(GeneratorExecutionContext context, SyntaxReceiver receiver)
        {
            var compilation = context.Compilation;
            var classSymbols = new List<INamedTypeSymbol>();
            foreach (var clazz in receiver.CandidateClasses)
            {
                var model = compilation.GetSemanticModel(clazz.SyntaxTree);
                var classSymbol = model.GetDeclaredSymbol(clazz);
                if (classSymbol.GetAttributes().Any(ad => ad.AttributeClass.Name == nameof(AutoFilterDtoAttribute)))
                {
                    classSymbols.Add((INamedTypeSymbol)classSymbol);
                }
            }

            return classSymbols;
        }

        internal string GetFilterDtoCode(string className, IEnumerable<IPropertySymbol> properties)
        {
            var start = $@"
using System;
using AutoFilterer;
using AutoFilterer.Attributes;
using AutoFilterer.Types;

namespace AutoFilterer.Filters
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

                body.AppendLine($"\t\tpublic {propertyType} {property.Name} {{ get; set; }}");
            }
            var end = "\t}\n}";


            return start + body.ToString() + end;
        }
    }
}
