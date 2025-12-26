using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace AutoFilterer.Generators;

[Generator]
public class ApplyFilterGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Inject the attribute into the user's compilation so it can be referenced in source code.
        context.RegisterPostInitializationOutput(static ctx =>
        {
            var source = @"using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class GenerateApplyFilterAttribute : Attribute
{
    public GenerateApplyFilterAttribute(Type entityType)
    {
        EntityType = entityType;
    }

    public GenerateApplyFilterAttribute(Type entityType, string @namespace)
    {
        EntityType = entityType;
        Namespace = @namespace;
    }

    public Type EntityType { get; }
    public string Namespace { get; }
}";
            ctx.AddSource("GenerateApplyFilterAttribute.g.cs", SourceText.From(source, Encoding.UTF8));
        });

        var candidates = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null);

        var compilationAndClasses = context.CompilationProvider.Combine(candidates.Collect());

        context.RegisterSourceOutput(compilationAndClasses, (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        if (node is not ClassDeclarationSyntax cds || cds.AttributeLists.Count == 0) return false;
        return cds.AttributeLists.SelectMany(al => al.Attributes)
            .Any(a => a.Name.ToString().EndsWith("GenerateApplyFilterAttribute") ||
                      a.Name.ToString().EndsWith("GenerateApplyFilter"));
    }

    private static ClassDeclarationSyntax GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        foreach (var attributeList in classDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                var attributeSymbol = context.SemanticModel.GetSymbolInfo(attribute).Symbol as IMethodSymbol;
                var fullName = attributeSymbol?.ContainingType?.ToDisplayString() ?? attribute.Name.ToString();
                if (fullName.EndsWith("GenerateApplyFilterAttribute"))
                {
                    return classDeclaration;
                }
            }
        }
        return null;
    }

    private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
    {
        if (classes.IsDefaultOrEmpty) return;

        foreach (var classSyntax in classes.Distinct())
        {
            var model = compilation.GetSemanticModel(classSyntax.SyntaxTree);
            if (model.GetDeclaredSymbol(classSyntax) is not INamedTypeSymbol filterSymbol) continue;

            var genAttr = filterSymbol.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.Name == "GenerateApplyFilterAttribute");
            if (genAttr == null) continue;

            // Resolve entity type and target namespace
            var entityType = genAttr.ConstructorArguments.Length > 0 ? genAttr.ConstructorArguments[0].Value as INamedTypeSymbol : null;
            var explicitNamespace = genAttr.ConstructorArguments.Length > 1 ? genAttr.ConstructorArguments[1].Value?.ToString() : null;
            if (entityType == null) continue;

            var targetNamespace = !string.IsNullOrWhiteSpace(explicitNamespace) ? explicitNamespace : GetNamespaceRecursively(filterSymbol.ContainingNamespace);

            // Build extension source
            var source = BuildApplyExtensionSource(targetNamespace, filterSymbol, entityType);
            var hintName = $"{filterSymbol.Name}.ApplyFilterExtensions.g.cs";
            context.AddSource(hintName, SourceText.From(source, Encoding.UTF8));
        }
    }

    private static string BuildApplyExtensionSource(string @namespace, INamedTypeSymbol filterSymbol, INamedTypeSymbol entitySymbol)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Linq;");
        sb.AppendLine("using AutoFilterer.Abstractions;");
        sb.AppendLine("using AutoFilterer;");
        sb.AppendLine("using AutoFilterer.Attributes;");
        sb.AppendLine("using AutoFilterer.Types;");
        sb.AppendLine();
        sb.AppendLine($"namespace {@namespace}");
        sb.AppendLine("{");
        sb.AppendLine($"    public static class {filterSymbol.Name}ApplyExtensions");
        sb.AppendLine("    {");
        sb.AppendLine($"        public static IQueryable<{entitySymbol.ToDisplayString()}> ApplyFilter(this IQueryable<{entitySymbol.ToDisplayString()}> source, {filterSymbol.ToDisplayString()} filter)");
        sb.AppendLine("        {");
        sb.AppendLine("            if (filter == null) return source;");

        // WHERE generation for each property
        foreach (var prop in filterSymbol.GetMembers().OfType<IPropertySymbol>().Where(p => !p.IsStatic && !p.IsIndexer))
        {
            var ignore = prop.GetAttributes().Any(a => a.AttributeClass?.Name == "IgnoreFilterAttribute");
            if (ignore) continue;

            // Check for CollectionFilterAttribute first
            var collectionFilterAttr = prop.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.Name == "CollectionFilterAttribute");
            
            if (collectionFilterAttr != null)
            {
                var compareToAttrs = prop.GetAttributes()
                    .Where(a => a.AttributeClass?.Name == "CompareToAttribute")
                    .ToArray();

                if (compareToAttrs.Length == 0)
                {
                    // Default to property name mapping
                    var entityProp = entitySymbol.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(ep => ep.Name == prop.Name);
                    if (entityProp != null)
                    {
                        EmitCollectionFilter(sb, entitySymbol, prop, entityProp, collectionFilterAttr, "x");
                    }
                }
                else
                {
                    foreach (var attr in compareToAttrs)
                    {
                        var propNamesConst = attr.ConstructorArguments.Length > 0 ? attr.ConstructorArguments[0] : default;
                        if (propNamesConst.Kind == TypedConstantKind.Array)
                        {
                            foreach (var item in propNamesConst.Values)
                            {
                                var targetName = item.Value?.ToString();
                                if (string.IsNullOrWhiteSpace(targetName)) continue;
                                var entityProp = ResolveMemberByPath(entitySymbol, targetName);
                                if (entityProp != null)
                                {
                                    EmitCollectionFilter(sb, entitySymbol, prop, entityProp, collectionFilterAttr, "x");
                                }
                            }
                        }
                        else
                        {
                            var targetName = propNamesConst.Value?.ToString();
                            if (!string.IsNullOrWhiteSpace(targetName))
                            {
                                var entityProp = ResolveMemberByPath(entitySymbol, targetName);
                                if (entityProp != null)
                                {
                                    EmitCollectionFilter(sb, entitySymbol, prop, entityProp, collectionFilterAttr, "x");
                                }
                            }
                        }
                    }
                }
                continue;
            }

            var compareToAttrs2 = prop.GetAttributes()
                .Where(a => a.AttributeClass?.Name == "CompareToAttribute")
                .ToArray();

            if (compareToAttrs2.Length == 0)
            {
                // Default to property name mapping if present on entity
                var entityProp = entitySymbol.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(ep => ep.Name == prop.Name);
                if (entityProp == null) continue;

                EmitScalarComparison(sb, entitySymbol, prop, entityProp, combineType: "And");
            }
            else
            {
                // For each CompareTo target property, emit comparisons
                foreach (var attr in compareToAttrs2)
                {
                    var combineArg = attr.NamedArguments.FirstOrDefault(kv => kv.Key == "CombineWith").Value;
                    var combineWith = combineArg.Value?.ToString() ?? "Or";

                    var propNamesConst = attr.ConstructorArguments.Length > 0 ? attr.ConstructorArguments[0] : default;
                    if (propNamesConst.Kind == TypedConstantKind.Array)
                    {
                        foreach (var item in propNamesConst.Values)
                        {
                            var targetName = item.Value?.ToString();
                            if (string.IsNullOrWhiteSpace(targetName)) continue;
                            var entityProp = ResolveMemberByPath(entitySymbol, targetName);
                            if (entityProp == null) continue;
                            EmitScalarComparison(sb, entitySymbol, prop, entityProp, combineWith);
                        }
                    }
                    else
                    {
                        var targetName = propNamesConst.Value?.ToString();
                        if (string.IsNullOrWhiteSpace(targetName)) continue;
                        var entityProp = ResolveMemberByPath(entitySymbol, targetName);
                        if (entityProp == null) continue;
                        EmitScalarComparison(sb, entitySymbol, prop, entityProp, combineWith);
                    }
                }
            }
        }

        // ORDER BY support (if filter exposes Sort)
        var sortProp = filterSymbol.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(p => p.Name == "Sort");
        if (sortProp != null && sortProp.Type.SpecialType == SpecialType.System_String)
        {
            sb.AppendLine("            if (!string.IsNullOrEmpty(filter.Sort))");
            sb.AppendLine("            {");
            sb.AppendLine("                switch (filter.Sort)");
            sb.AppendLine("                {");
            foreach (var ep in entitySymbol.GetMembers().OfType<IPropertySymbol>().Where(p => !p.IsStatic))
            {
                sb.AppendLine($"                    case nameof({entitySymbol.ToDisplayString()}.{ep.Name}):");
                sb.AppendLine("                    {");
                var sortByProp = filterSymbol.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(p => p.Name == "SortBy");
                if (sortByProp != null)
                {
                    sb.AppendLine("                        if (filter.SortBy == AutoFilterer.Sorting.Descending)");
                    sb.AppendLine($"                            source = source.OrderByDescending(x => x.{ep.Name});");
                    sb.AppendLine("                        else");
                    sb.AppendLine($"                            source = source.OrderBy(x => x.{ep.Name});");
                }
                else
                {
                    sb.AppendLine($"                        source = source.OrderBy(x => x.{ep.Name});");
                }
                sb.AppendLine("                        break;");
                sb.AppendLine("                    }");
            }
            sb.AppendLine("                    default: throw new ArgumentException(\"Invalid Sort field\", nameof(filter.Sort));");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
        }

        // Pagination support (Page & PerPage)
        var pageProp = filterSymbol.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(p => p.Name == "Page");
        var perPageProp = filterSymbol.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(p => p.Name == "PerPage");
        if (pageProp != null && perPageProp != null)
        {
            sb.AppendLine("            if (filter.Page > 0 && filter.PerPage > 0)");
            sb.AppendLine("            {");
            sb.AppendLine("                source = AutoFilterer.Extensions.QueryExtensions.ToPaged(source, filter.Page, filter.PerPage);");
            sb.AppendLine("            }");
        }

        sb.AppendLine("            return source;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static IPropertySymbol ResolveMemberByPath(INamedTypeSymbol entitySymbol, string path)
    {
        // Supports simple names and one-level navigation: Prop or Nav.Prop
        var parts = path.Split('.');
        ITypeSymbol currentType = entitySymbol;
        IPropertySymbol lastProp = null;
        foreach (var part in parts)
        {
            lastProp = currentType.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(p => p.Name == part);
            if (lastProp == null) return null;
            currentType = lastProp.Type;
        }
        return lastProp;
    }

    private static void EmitCollectionFilter(StringBuilder sb, INamedTypeSymbol entitySymbol, IPropertySymbol filterProp, IPropertySymbol entityProp, AttributeData collectionFilterAttr, string parentParam)
    {
        var fpName = filterProp.Name;
        var epName = entityProp.Name;

        // Get FilterOption (Any or All)
        var filterOption = "Any"; // default
        var filterOptionArg = collectionFilterAttr.ConstructorArguments.Length > 0 ? collectionFilterAttr.ConstructorArguments[0] : default;
        if (filterOptionArg.Value != null)
        {
            // The value is an integer enum value
            var enumValue = filterOptionArg.Value.ToString();
            filterOption = enumValue == "0" ? "Any" : "All";
        }
        else
        {
            var namedArg = collectionFilterAttr.NamedArguments.FirstOrDefault(kv => kv.Key == "FilterOption").Value;
            if (namedArg.Value != null)
            {
                var enumValue = namedArg.Value.ToString();
                filterOption = enumValue == "0" ? "Any" : "All";
            }
        }

        // Check if entity property is a collection
        if (entityProp.Type is not INamedTypeSymbol collectionType) return;
        
        // Try to get the element type
        ITypeSymbol elementType = null;
        if (collectionType.IsGenericType)
        {
            elementType = collectionType.TypeArguments.FirstOrDefault();
        }
        
        if (elementType == null) return;

        // Check if filter property type is an IFilter
        var isFilterType = IsFilterType(filterProp.Type);
        if (!isFilterType) return;

        var nestedFilterType = filterProp.Type as INamedTypeSymbol;
        if (nestedFilterType == null) return;

        sb.AppendLine($"            if (filter.{fpName} != null)");
        sb.AppendLine("            {");
        
        // Generate nested lambda parameter
        var nestedParam = GetNestedParameterName(parentParam);
        
        // Build nested filter conditions with proper filter path
        var nestedConditions = BuildNestedFilterConditions(nestedFilterType, elementType as INamedTypeSymbol, nestedParam, $"filter.{fpName}");
        
        if (!string.IsNullOrEmpty(nestedConditions))
        {
            sb.AppendLine($"                source = source.Where({parentParam} => {parentParam}.{epName}.{filterOption}({nestedParam} =>");
            sb.AppendLine($"                    {nestedConditions}");
            sb.AppendLine("                ));");
        }
        
        sb.AppendLine("            }");
    }

    private static bool IsFilterType(ITypeSymbol type)
    {
        if (type == null) return false;
        
        // Check if implements IFilter interface
        foreach (var iface in type.AllInterfaces)
        {
            if (iface.Name == "IFilter" && iface.ContainingNamespace?.ToDisplayString() == "AutoFilterer.Abstractions")
            {
                return true;
            }
        }
        
        // Check if inherits from FilterBase
        var baseType = type.BaseType;
        while (baseType != null)
        {
            if (baseType.Name == "FilterBase" && baseType.ContainingNamespace?.ToDisplayString() == "AutoFilterer.Types")
            {
                return true;
            }
            baseType = baseType.BaseType;
        }
        
        return false;
    }

    private static string GetNestedParameterName(string parentParam)
    {
        // x -> a -> b -> c -> d
        var paramNames = new[] { "x", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
        var index = Array.IndexOf(paramNames, parentParam);
        if (index >= 0 && index < paramNames.Length - 1)
        {
            return paramNames[index + 1];
        }
        return "item";
    }

    private static string BuildNestedFilterConditions(INamedTypeSymbol nestedFilterType, INamedTypeSymbol nestedEntityType, string paramName, string filterPath = "filter")
    {
        if (nestedFilterType == null || nestedEntityType == null) return string.Empty;

        var conditions = new List<string>();

        foreach (var filterProp in nestedFilterType.GetMembers().OfType<IPropertySymbol>().Where(p => !p.IsStatic && !p.IsIndexer))
        {
            var ignore = filterProp.GetAttributes().Any(a => a.AttributeClass?.Name == "IgnoreFilterAttribute");
            if (ignore) continue;

            // Check for nested CollectionFilter (recursive)
            var nestedCollectionAttr = filterProp.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.Name == "CollectionFilterAttribute");
            
            if (nestedCollectionAttr != null)
            {
                // Handle nested collection filters recursively
                var compareToAttrs = filterProp.GetAttributes()
                    .Where(a => a.AttributeClass?.Name == "CompareToAttribute")
                    .ToArray();

                if (compareToAttrs.Length == 0)
                {
                    var entityProp = nestedEntityType.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(ep => ep.Name == filterProp.Name);
                    if (entityProp != null)
                    {
                        var nestedCondition = BuildNestedCollectionCondition(filterProp, entityProp, nestedCollectionAttr, paramName, filterPath);
                        if (!string.IsNullOrEmpty(nestedCondition))
                        {
                            conditions.Add(nestedCondition);
                        }
                    }
                }
                else
                {
                    foreach (var attr in compareToAttrs)
                    {
                        var propNamesConst = attr.ConstructorArguments.Length > 0 ? attr.ConstructorArguments[0] : default;
                        var targetNames = new List<string>();
                        
                        if (propNamesConst.Kind == TypedConstantKind.Array)
                        {
                            targetNames.AddRange(propNamesConst.Values.Select(v => v.Value?.ToString()).Where(n => !string.IsNullOrWhiteSpace(n)));
                        }
                        else
                        {
                            var targetName = propNamesConst.Value?.ToString();
                            if (!string.IsNullOrWhiteSpace(targetName))
                            {
                                targetNames.Add(targetName);
                            }
                        }

                        foreach (var targetName in targetNames)
                        {
                            var entityProp = ResolveNestedMemberByPath(nestedEntityType, targetName);
                            if (entityProp != null)
                            {
                                var nestedCondition = BuildNestedCollectionCondition(filterProp, entityProp, nestedCollectionAttr, paramName, filterPath);
                                if (!string.IsNullOrEmpty(nestedCondition))
                                {
                                    conditions.Add(nestedCondition);
                                }
                            }
                        }
                    }
                }
                continue;
            }

            // Handle regular scalar comparisons
            var compareToAttrs2 = filterProp.GetAttributes()
                .Where(a => a.AttributeClass?.Name == "CompareToAttribute")
                .ToArray();

            if (compareToAttrs2.Length == 0)
            {
                var entityProp = nestedEntityType.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(ep => ep.Name == filterProp.Name);
                if (entityProp != null)
                {
                    var condition = BuildNestedScalarCondition(filterProp, entityProp, paramName, filterPath);
                    if (!string.IsNullOrEmpty(condition))
                    {
                        conditions.Add(condition);
                    }
                }
            }
            else
            {
                foreach (var attr in compareToAttrs2)
                {
                    var propNamesConst = attr.ConstructorArguments.Length > 0 ? attr.ConstructorArguments[0] : default;
                    var targetNames = new List<string>();
                    
                    if (propNamesConst.Kind == TypedConstantKind.Array)
                    {
                        targetNames.AddRange(propNamesConst.Values.Select(v => v.Value?.ToString()).Where(n => !string.IsNullOrWhiteSpace(n)));
                    }
                    else
                    {
                        var targetName = propNamesConst.Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(targetName))
                        {
                            targetNames.Add(targetName);
                        }
                    }

                    foreach (var targetName in targetNames)
                    {
                        var entityProp = ResolveNestedMemberByPath(nestedEntityType, targetName);
                        if (entityProp != null)
                        {
                            var condition = BuildNestedScalarCondition(filterProp, entityProp, paramName, filterPath);
                            if (!string.IsNullOrEmpty(condition))
                            {
                                conditions.Add(condition);
                            }
                        }
                    }
                }
            }
        }

        // Combine conditions with AND
        if (conditions.Count == 0) return "true";
        if (conditions.Count == 1) return conditions[0];
        return string.Join(" && ", conditions.Select(c => $"({c})"));
    }

    private static string BuildNestedCollectionCondition(IPropertySymbol filterProp, IPropertySymbol entityProp, AttributeData collectionFilterAttr, string parentParam, string filterPath)
    {
        var fpName = filterProp.Name;
        var epName = entityProp.Name;

        // Get FilterOption
        var filterOption = "Any";
        var filterOptionArg = collectionFilterAttr.ConstructorArguments.Length > 0 ? collectionFilterAttr.ConstructorArguments[0] : default;
        if (filterOptionArg.Value != null)
        {
            var enumValue = filterOptionArg.Value.ToString();
            filterOption = enumValue == "0" ? "Any" : "All";
        }
        else
        {
            var namedArg = collectionFilterAttr.NamedArguments.FirstOrDefault(kv => kv.Key == "FilterOption").Value;
            if (namedArg.Value != null)
            {
                var enumValue = namedArg.Value.ToString();
                filterOption = enumValue == "0" ? "Any" : "All";
            }
        }

        if (entityProp.Type is not INamedTypeSymbol collectionType) return string.Empty;
        
        ITypeSymbol elementType = null;
        if (collectionType.IsGenericType)
        {
            elementType = collectionType.TypeArguments.FirstOrDefault();
        }
        
        if (elementType == null) return string.Empty;

        var isFilterType = IsFilterType(filterProp.Type);
        if (!isFilterType) return string.Empty;

        var nestedFilterType = filterProp.Type as INamedTypeSymbol;
        if (nestedFilterType == null) return string.Empty;

        var deeperParam = GetNestedParameterName(parentParam);
        var deeperFilterPath = $"{filterPath}.{fpName}";
        var innerConditions = BuildNestedFilterConditions(nestedFilterType, elementType as INamedTypeSymbol, deeperParam, deeperFilterPath);

        if (string.IsNullOrEmpty(innerConditions)) return string.Empty;

        return $"{filterPath}.{fpName} != null && {parentParam}.{epName}.{filterOption}({deeperParam} => {innerConditions})";
    }

    private static string BuildNestedScalarCondition(IPropertySymbol filterProp, IPropertySymbol entityProp, string paramName, string filterPath)
    {
        var fpName = filterProp.Name;
        var epName = entityProp.Name;
        var filterRef = $"{filterPath}.{fpName}";

        // Handle StringFilter
        if (filterProp.Type is INamedTypeSymbol named && named.ToDisplayString().StartsWith("AutoFilterer.Types.StringFilter"))
        {
            var conditions = new List<string>();
            conditions.Add($"{filterRef}.Eq != null && {paramName}.{epName} == {filterRef}.Eq");
            conditions.Add($"{filterRef}.Contains != null && {paramName}.{epName}.Contains({filterRef}.Contains)");
            conditions.Add($"{filterRef}.StartsWith != null && {paramName}.{epName}.StartsWith({filterRef}.StartsWith)");
            conditions.Add($"{filterRef}.EndsWith != null && {paramName}.{epName}.EndsWith({filterRef}.EndsWith)");
            return $"{filterRef} == null || ({string.Join(" || ", conditions)})";
        }

        // Handle Range<T>
        if (filterProp.Type is INamedTypeSymbol namedRange && namedRange.Name == "Range")
        {
            var conditions = new List<string>();
            conditions.Add($"{filterRef}.Min == null || {paramName}.{epName} >= {filterRef}.Min");
            conditions.Add($"{filterRef}.Max == null || {paramName}.{epName} <= {filterRef}.Max");
            return $"{filterRef} == null || ({string.Join(" && ", conditions)})";
        }

        // Handle OperatorFilter<T>
        if (filterProp.Type is INamedTypeSymbol namedOp && namedOp.Name == "OperatorFilter")
        {
            var conditions = new List<string>();
            conditions.Add($"{filterRef}.Eq == null || {paramName}.{epName} == {filterRef}.Eq");
            conditions.Add($"{filterRef}.Gt == null || {paramName}.{epName} > {filterRef}.Gt");
            conditions.Add($"{filterRef}.Lt == null || {paramName}.{epName} < {filterRef}.Lt");
            conditions.Add($"{filterRef}.Gte == null || {paramName}.{epName} >= {filterRef}.Gte");
            conditions.Add($"{filterRef}.Lte == null || {paramName}.{epName} <= {filterRef}.Lte");
            return $"{filterRef} == null || ({string.Join(" && ", conditions)})";
        }

        // Default scalar comparison
        if (filterProp.Type.SpecialType == SpecialType.System_String)
        {
            return $"string.IsNullOrEmpty({filterRef}) || {paramName}.{epName} == {filterRef}";
        }
        else
        {
            var isNullable = filterProp.NullableAnnotation == Microsoft.CodeAnalysis.NullableAnnotation.Annotated;
            if (isNullable)
            {
                return $"{filterRef} == null || {paramName}.{epName}.Equals({filterRef})";
            }
        }
        
        return string.Empty;
    }

    private static IPropertySymbol ResolveNestedMemberByPath(INamedTypeSymbol entitySymbol, string path)
    {
        if (entitySymbol == null || string.IsNullOrEmpty(path)) return null;
        
        var parts = path.Split('.');
        ITypeSymbol currentType = entitySymbol;
        IPropertySymbol lastProp = null;
        
        foreach (var part in parts)
        {
            lastProp = currentType.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(p => p.Name == part);
            if (lastProp == null) return null;
            currentType = lastProp.Type;
        }
        
        return lastProp;
    }

    private static void EmitScalarComparison(StringBuilder sb, INamedTypeSymbol entitySymbol, IPropertySymbol filterProp, IPropertySymbol entityProp, string combineType)
    {
        var fpName = filterProp.Name;
        var epName = entityProp.Name;

        // Handle StringFilter
        if (filterProp.Type is INamedTypeSymbol named && named.ToDisplayString().StartsWith("AutoFilterer.Types.StringFilter"))
        {
            sb.AppendLine($"            if (filter.{fpName} != null)");
            sb.AppendLine("            {");
            if (combineType == "Or")
            {
                sb.AppendLine("                source = source.Where(x => ");
                sb.AppendLine($"                    (filter.{fpName}.Eq != null && x.{epName} == filter.{fpName}.Eq) ||");
                sb.AppendLine($"                    (filter.{fpName}.Contains != null && x.{epName}.Contains(filter.{fpName}.Contains)) ||");
                sb.AppendLine($"                    (filter.{fpName}.StartsWith != null && x.{epName}.StartsWith(filter.{fpName}.StartsWith)) ||");
                sb.AppendLine($"                    (filter.{fpName}.EndsWith != null && x.{epName}.EndsWith(filter.{fpName}.EndsWith))");
                sb.AppendLine("                );");
            }
            else
            {
                sb.AppendLine($"                if (filter.{fpName}.Eq != null) source = source.Where(x => x.{epName} == filter.{fpName}.Eq);");
                sb.AppendLine($"                if (filter.{fpName}.Contains != null) source = source.Where(x => x.{epName}.Contains(filter.{fpName}.Contains));");
                sb.AppendLine($"                if (filter.{fpName}.StartsWith != null) source = source.Where(x => x.{epName}.StartsWith(filter.{fpName}.StartsWith));");
                sb.AppendLine($"                if (filter.{fpName}.EndsWith != null) source = source.Where(x => x.{epName}.EndsWith(filter.{fpName}.EndsWith));");
            }
            sb.AppendLine("            }");
            return;
        }

        // Handle Range<T>
        if (filterProp.Type is INamedTypeSymbol namedRange && namedRange.Name == "Range" && namedRange.ContainingNamespace.ToDisplayString() == "AutoFilterer.Types")
        {
            sb.AppendLine($"            if (filter.{fpName} != null)");
            sb.AppendLine("            {");
            sb.AppendLine($"                if (filter.{fpName}.Min != null) source = source.Where(x => x.{epName} >= filter.{fpName}.Min);");
            sb.AppendLine($"                if (filter.{fpName}.Max != null) source = source.Where(x => x.{epName} <= filter.{fpName}.Max);");
            sb.AppendLine("            }");
            return;
        }

        // Handle OperatorFilter<T>
        if (filterProp.Type is INamedTypeSymbol namedOp && namedOp.Name == "OperatorFilter")
        {
            sb.AppendLine($"            if (filter.{fpName} != null)");
            sb.AppendLine("            {");
            sb.AppendLine($"                if (filter.{fpName}.Eq != null) source = source.Where(x => x.{epName} == filter.{fpName}.Eq);");
            sb.AppendLine($"                if (filter.{fpName}.Gt != null) source = source.Where(x => x.{epName} > filter.{fpName}.Gt);");
            sb.AppendLine($"                if (filter.{fpName}.Lt != null) source = source.Where(x => x.{epName} < filter.{fpName}.Lt);");
            sb.AppendLine($"                if (filter.{fpName}.Gte != null) source = source.Where(x => x.{epName} >= filter.{fpName}.Gte);");
            sb.AppendLine($"                if (filter.{fpName}.Lte != null) source = source.Where(x => x.{epName} <= filter.{fpName}.Lte);");
            sb.AppendLine("            }");
            return;
        }

        // Default scalar/string equality comparison
        if (filterProp.Type.SpecialType == SpecialType.System_String)
        {
            sb.AppendLine($"            if (!string.IsNullOrEmpty(filter.{fpName})) source = source.Where(x => x.{epName} == filter.{fpName});");
        }
        else
        {
            // Generate only for nullable value types to avoid unintended defaults
            var isNullable = filterProp.NullableAnnotation == Microsoft.CodeAnalysis.NullableAnnotation.Annotated;
            if (isNullable)
            {
                sb.AppendLine($"            if (filter.{fpName} != null) source = source.Where(x => x.{epName}.Equals(filter.{fpName}));");
            }
        }
    }

    private static string GetNamespaceRecursively(INamespaceSymbol symbol)
    {
        if (symbol == null || symbol.IsGlobalNamespace) return string.Empty;
        var parent = GetNamespaceRecursively(symbol.ContainingNamespace);
        return string.IsNullOrEmpty(parent) ? symbol.Name : parent + "." + symbol.Name;
    }
}
