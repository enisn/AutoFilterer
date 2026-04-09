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
            var targetNamespace = attribute?.ConstructorArguments.FirstOrDefault().Value?.ToString().Trim('\"');
            if (string.IsNullOrEmpty(targetNamespace)) {
                targetNamespace = GetNamespaceRecursively(symbol.ContainingNamespace);
            }

            // Parse generation options from attribute
            var baseClass = GetNamedArgumentValue<string>(attribute, nameof(GenerateAutoFilterAttribute.BaseClass)) ?? "PaginationFilterBase";
            var useStringFilter = GetNamedArgumentValue<bool>(attribute, nameof(GenerateAutoFilterAttribute.UseStringFilter));

            // Handle nullable bool options - use default true when not specified
            var useRangeForNumbersTemp = GetNamedArgumentValue<bool?>(attribute, nameof(GenerateAutoFilterAttribute.UseRangeForNumbers));
            var useRangeForNumbers = useRangeForNumbersTemp ?? true;

            var useRangeForDatesTemp = GetNamedArgumentValue<bool?>(attribute, nameof(GenerateAutoFilterAttribute.UseRangeForDates));
            var useRangeForDates = useRangeForDatesTemp ?? true;

            var generateForEnumPropertiesTemp = GetNamedArgumentValue<bool?>(attribute, nameof(GenerateAutoFilterAttribute.GenerateForEnumProperties));
            var generateForEnumProperties = generateForEnumPropertiesTemp ?? true;

            var properties = symbol.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(x => !x.IsStatic && !x.IsIndexer && x.Kind == SymbolKind.Property)
                .Where(x => !ShouldSkipProperty(x));

            var sourceCode = GetFilterDtoCode(symbol.Name, properties, targetNamespace, baseClass, useStringFilter, useRangeForNumbers, useRangeForDates, generateForEnumProperties);

            context.AddSource($"{symbol.Name}FilterDto.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
        }
    }

    private static T GetNamedArgumentValue<T>(AttributeData attribute, string argumentName)
    {
        if (attribute == null)
        {
            return default;
        }

        foreach (var namedArgument in attribute.NamedArguments)
        {
            if (namedArgument.Key == argumentName)
            {
                try
                {
                    return (T)namedArgument.Value.Value;
                }
                catch
                {
                    return default;
                }
            }
        }

        return default;
    }

    private static bool ShouldSkipProperty(IPropertySymbol property)
    {
        // Skip collection types (arrays, IEnumerable<T>, etc.)
        if (property.Type is IArrayTypeSymbol)
        {
            return true;
        }

        if (property.Type is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            var typeDefinition = namedType.ConstructedFrom?.ToDisplayString() ?? namedType.ToDisplayString();

            // Skip IEnumerable<T>, ICollection<T>, IList<T>, List<T>, etc.
            if (typeDefinition.StartsWith("System.Collections.Generic.IEnumerable<") ||
                typeDefinition.StartsWith("System.Collections.Generic.ICollection<") ||
                typeDefinition.StartsWith("System.Collections.Generic.IList<") ||
                typeDefinition.StartsWith("System.Collections.Generic.List<") ||
                typeDefinition.StartsWith("System.Collections.Generic.HashSet<") ||
                typeDefinition.StartsWith("System.Collections.Generic.IReadOnlyCollection<") ||
                typeDefinition.StartsWith("System.Collections.Generic.IReadOnlyList<"))
            {
                return true;
            }
        }

        // Skip navigation properties to other complex types (non-primitive, non-enum)
        // Only generate for basic types, strings, enums, and nullable versions of these
        var type = property.Type;
        var underlyingType = type;

        if (type is INamedTypeSymbol namedValueType && namedValueType.IsValueType && namedValueType.IsGenericType)
        {
            var genericDefinition = namedValueType.ConstructedFrom?.ToDisplayString();
            if (genericDefinition == "System.Nullable<T>")
            {
                underlyingType = namedValueType.TypeArguments[0];
            }
        }

        // Check if it's a type we should generate for
        if (underlyingType.SpecialType == SpecialType.System_String)
        {
            return false;
        }

        // Check for enums
        if (underlyingType.TypeKind == TypeKind.Enum)
        {
            return false;
        }

        // Check for basic numeric types
        if (underlyingType.SpecialType >= SpecialType.System_Boolean && underlyingType.SpecialType <= SpecialType.System_UInt64)
        {
            return false;
        }

        // Check for decimal
        if (underlyingType.SpecialType == SpecialType.System_Decimal)
        {
            return false;
        }

        // Check for DateTime, DateTimeOffset, TimeSpan, Guid
        var typeName = underlyingType.ToDisplayString();
        if (typeName == "System.DateTime" || typeName == "System.DateTimeOffset" ||
            typeName == "System.TimeSpan" || typeName == "System.Guid")
        {
            return false;
        }

        // Skip any other complex types (navigation properties)
        return true;
    }

    private static string GetFilterDtoCode(string className, IEnumerable<IPropertySymbol> properties,
        string @namespace = null,
        string baseClass = "PaginationFilterBase",
        bool useStringFilter = false,
        bool useRangeForNumbers = true,
        bool useRangeForDates = true,
        bool generateForEnumProperties = true)
    {
        var generatedCode = new StringBuilder();
        generatedCode.AppendLine("using System;");
        generatedCode.AppendLine("using AutoFilterer.Attributes;");
        generatedCode.AppendLine("using AutoFilterer.Types;");
        generatedCode.AppendLine();
        generatedCode.AppendLine($"namespace {@namespace}");
        generatedCode.AppendLine("{");
        generatedCode.AppendLine($"\tpublic partial class {className}Filter : {baseClass}");
        generatedCode.AppendLine("\t{");

        foreach (var property in properties)
        {
            var propertyType = property.Type.ToDisplayString(NullableFlowState.None);
            var originalType = property.Type;
            var underlyingType = originalType;

            // Check for nullable types to get the underlying type
            if (originalType is INamedTypeSymbol namedType && namedType.IsValueType && namedType.IsGenericType)
            {
                var genericDefinition = namedType.ConstructedFrom?.ToDisplayString();
                if (genericDefinition == "System.Nullable<T>")
                {
                    underlyingType = namedType.TypeArguments[0];
                }
            }

            // Skip enum properties if not configured to generate them
            if (underlyingType.TypeKind == TypeKind.Enum && !generateForEnumProperties)
            {
                continue;
            }

            // Handle string type mapping
            if (originalType.SpecialType == SpecialType.System_String && useStringFilter)
            {
                propertyType = "StringFilter";
            }
            // Handle numeric types
            else if (TypeMapping.Mappings.TryGetValue(propertyType, out var mapped))
            {
                // Check if this is a numeric or date type that should use Range
                var typeName = underlyingType.ToDisplayString();

                // For numbers, respect useRangeForNumbers flag
                if ((underlyingType.SpecialType >= SpecialType.System_SByte && underlyingType.SpecialType <= SpecialType.System_UInt64) ||
                    underlyingType.SpecialType == SpecialType.System_Decimal)
                {
                    if (!useRangeForNumbers)
                    {
                        // Use the original type instead of Range
                        propertyType = originalType.ToDisplayString(NullableFlowState.None);
                    }
                    else
                    {
                        propertyType = mapped;
                    }
                }
                // For DateTime/DateTimeOffset/TimeSpan, respect useRangeForDates flag
                else if (typeName == "System.DateTime" || typeName == "System.DateTimeOffset" || typeName == "System.TimeSpan")
                {
                    if (!useRangeForDates)
                    {
                        propertyType = originalType.ToDisplayString(NullableFlowState.None);
                    }
                    else
                    {
                        propertyType = mapped;
                    }
                }
                // For bool and Guid, always keep as-is (they don't use Range)
                else
                {
                    propertyType = mapped;
                }
            }

            // Add attributes based on type
            if (originalType.SpecialType == SpecialType.System_String)
            {
                if (!useStringFilter)
                {
                    generatedCode.AppendLine("\t\t[ToLowerContainsComparison]");
                }
                // When useStringFilter is true, StringFilter handles the filtering internally, no attribute needed
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