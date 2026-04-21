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
        sb.AppendLine("#if LEGACY_NAMESPACE");
        sb.AppendLine("using AutoFilterer.Enums;");
        sb.AppendLine("#endif");
        sb.AppendLine();
        sb.AppendLine($"namespace {@namespace}");
        sb.AppendLine("{");
        sb.AppendLine($"    public static class {filterSymbol.Name}ApplyExtensions");
        sb.AppendLine("    {");
        sb.AppendLine($"        public static IQueryable<{entitySymbol.ToDisplayString()}> ApplyFilter(this IQueryable<{entitySymbol.ToDisplayString()}> source, {filterSymbol.ToDisplayString()} filter)");
        sb.AppendLine("        {");
        sb.AppendLine("            if (filter == null) return source;");

        EmitParityPrefilters(sb, filterSymbol, entitySymbol);

        var filterState = BuildFilterState(filterSymbol, entitySymbol, "x", "filter");
        if (!string.IsNullOrWhiteSpace(filterState.Condition) && filterState.Condition != "true")
        {
            sb.AppendLine($"            source = source.Where(x => {filterState.Condition});");
        }

        EmitOrderingAndPagination(sb, filterSymbol, entitySymbol);

        sb.AppendLine("            return source;");
        sb.AppendLine("        }");
        EmitNestedFilterHelpers(sb, filterSymbol, entitySymbol, new HashSet<string>(StringComparer.Ordinal));
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static bool IsBatch1SafeFilter(INamedTypeSymbol filterSymbol, INamedTypeSymbol entitySymbol)
    {
        // Batch 1 strict support: direct/single-segment mapping only
        // No collection filters, no nested IFilter recursion, no typed CompareTo, no multi-target CompareTo
        // Limited operator/range features

        var filterProps = filterSymbol.GetMembers().OfType<IPropertySymbol>()
            .Where(p => !p.IsStatic && !p.IsIndexer)
            .ToArray();

        foreach (var prop in filterProps)
        {
            var ignore = prop.GetAttributes().Any(a => a.AttributeClass?.Name == "IgnoreFilterAttribute");
            if (ignore) continue;

            // No collection filters in Batch 1
            var collectionFilterAttr = prop.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.Name == "CollectionFilterAttribute");
            if (collectionFilterAttr != null)
            {
                return false;
            }

            // No array properties in Batch 1 (complex runtime behavior)
            if (prop.Type is IArrayTypeSymbol)
            {
                return false;
            }

            // No nested IFilter types in Batch 1
            if (IsFilterType(prop.Type))
            {
                return false;
            }

            // No StringFilterOptionsAttribute in Batch 1
            var stringFilterOptions = prop.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.Name == "StringFilterOptionsAttribute");
            if (stringFilterOptions != null)
            {
                return false;
            }

            // No OperatorComparisonAttribute in Batch 1
            var operatorComparison = prop.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.Name == "OperatorComparisonAttribute");
            if (operatorComparison != null)
            {
                return false;
            }

            // Check CompareToAttribute constraints
            var compareToAttrs = prop.GetAttributes()
                .Where(a => a.AttributeClass?.Name == "CompareToAttribute")
                .ToArray();

            foreach (var attr in compareToAttrs)
            {
                // No multi-target CompareTo (array of property names)
                var propNamesConst = attr.ConstructorArguments.Length > 0 ? attr.ConstructorArguments[0] : default;
                var targetNames = ExtractTargetPropertyNames(propNamesConst).ToArray();
                if (targetNames.Length > 1)
                {
                    return false;
                }

                // Only support single-segment property names (no nested navigation like "Nav.Prop")
                var targetName = targetNames.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(targetName) && targetName.Contains('.'))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static void EmitBatch1WhereConditions(StringBuilder sb, INamedTypeSymbol filterSymbol, INamedTypeSymbol entitySymbol)
    {
        var filterProps = filterSymbol.GetMembers().OfType<IPropertySymbol>()
            .Where(p => !p.IsStatic && !p.IsIndexer)
            .ToArray();

        foreach (var prop in filterProps)
        {
            var ignore = prop.GetAttributes().Any(a => a.AttributeClass?.Name == "IgnoreFilterAttribute");
            if (ignore) continue;

            // Resolve single target property (direct or single-segment CompareTo)
            var targetPath = prop.Name; // default to property name
            var entityProp = entitySymbol.GetMembers().OfType<IPropertySymbol>()
                .FirstOrDefault(ep => ep.Name == prop.Name);

            var compareToAttrs = prop.GetAttributes()
                .Where(a => a.AttributeClass?.Name == "CompareToAttribute")
                .ToArray();

            if (compareToAttrs.Length > 0)
            {
                // Use the single CompareTo target (we already validated no multi-target in IsBatch1SafeFilter)
                var targetName = (string)null;
                if (compareToAttrs[0].ConstructorArguments.Length > 0)
                {
                    var targetArg = compareToAttrs[0].ConstructorArguments[0];
                    targetName = ExtractTargetPropertyNames(targetArg).FirstOrDefault();
                }

                if (!string.IsNullOrWhiteSpace(targetName))
                {
                    entityProp = entitySymbol.GetMembers().OfType<IPropertySymbol>()
                        .FirstOrDefault(ep => ep.Name == targetName);
                    targetPath = targetName;
                }
            }

            if (entityProp == null) continue;

            // Handle Range<T>
            if (prop.Type is INamedTypeSymbol namedRange && namedRange.Name == "Range")
            {
                sb.AppendLine($"            if (filter.{prop.Name} != null)");
                sb.AppendLine("            {");
                sb.AppendLine($"                if (filter.{prop.Name}.Min != null) source = source.Where(x => x.{targetPath} >= filter.{prop.Name}.Min);");
                sb.AppendLine($"                if (filter.{prop.Name}.Max != null) source = source.Where(x => x.{targetPath} <= filter.{prop.Name}.Max);");
                sb.AppendLine("            }");
                continue;
            }

            // Handle OperatorFilter<T>
            if (prop.Type is INamedTypeSymbol namedOp && namedOp.Name == "OperatorFilter")
            {
                sb.AppendLine($"            if (filter.{prop.Name} != null)");
                sb.AppendLine("            {");
                sb.AppendLine($"                source = source.Where(x => {BuildOperatorFilterPredicate($"filter.{prop.Name}", $"x.{targetPath}", IsNullableValueType(entityProp.Type))});");
                sb.AppendLine("            }");
                continue;
            }

            // Handle StringFilter
            if (prop.Type is INamedTypeSymbol named && named.ToDisplayString().StartsWith("AutoFilterer.Types.StringFilter"))
            {
                sb.AppendLine($"            if (filter.{prop.Name} != null)");
                sb.AppendLine("            {");
                sb.AppendLine($"                source = source.Where(x => {BuildStringFilterPredicate($"filter.{prop.Name}", $"x.{targetPath}")});");
                sb.AppendLine("            }");
                continue;
            }

            // Default scalar/string equality comparison
            if (prop.Type.SpecialType == SpecialType.System_String)
            {
                sb.AppendLine($"            if (filter.{prop.Name} != null) source = source.Where(x => x.{targetPath} == filter.{prop.Name});");
            }
            else if (IsNullableValueType(prop.Type))
            {
                sb.AppendLine($"            if (filter.{prop.Name} != null) source = source.Where(x => x.{targetPath}.Equals(filter.{prop.Name}));");
            }
        }

        // ORDER BY support (if filter exposes Sort)
        var sortProp = FindPropertyIncludingBase(filterSymbol, "Sort");
        if (sortProp != null && sortProp.Type.SpecialType == SpecialType.System_String)
        {
            sb.AppendLine("            if (!string.IsNullOrEmpty(filter.Sort))");
            sb.AppendLine("            {");
            sb.AppendLine("                switch (filter.Sort)");
            sb.AppendLine("                {");
            foreach (var sortPath in EnumerateSortablePaths(entitySymbol).Distinct(StringComparer.Ordinal))
            {
                sb.AppendLine($"                    case \"{sortPath}\":");
                sb.AppendLine("                    {");
                var sortByProp = FindPropertyIncludingBase(filterSymbol, "SortBy");
                if (sortByProp != null)
                {
                    sb.AppendLine("                        if (filter.SortBy == Sorting.Descending)");
                    sb.AppendLine($"                            source = source.OrderByDescending(x => x.{sortPath});");
                    sb.AppendLine("                        else");
                    sb.AppendLine($"                            source = source.OrderBy(x => x.{sortPath});");
                }
                else
                {
                    sb.AppendLine($"                        source = source.OrderBy(x => x.{sortPath});");
                }
                sb.AppendLine("                        break;");
                sb.AppendLine("                    }");
            }
            sb.AppendLine("                    default: throw new ArgumentException(\"Invalid Sort field\", nameof(filter.Sort));");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
        }

        // Pagination support (Page & PerPage)
        var pageProp = FindPropertyIncludingBase(filterSymbol, "Page");
        var perPageProp = FindPropertyIncludingBase(filterSymbol, "PerPage");
        if (pageProp != null && perPageProp != null)
        {
            sb.AppendLine("            if (filter.Page > 0 && filter.PerPage > 0)");
            sb.AppendLine("            {");
            sb.AppendLine("                source = AutoFilterer.Extensions.QueryExtensions.ToPaged(source, filter.Page, filter.PerPage);");
            sb.AppendLine("            }");
        }
    }

    private static (string Active, string Condition) BuildFilterState(INamedTypeSymbol filterSymbol, INamedTypeSymbol entitySymbol, string paramName, string filterPath)
    {
        var propertyStates = new List<(string Active, string Expression)>();

        foreach (var filterProp in filterSymbol.GetMembers().OfType<IPropertySymbol>().Where(p => !p.IsStatic && !p.IsIndexer))
        {
            var state = BuildPropertyState(filterProp, entitySymbol, paramName, filterPath);
            if (!string.IsNullOrWhiteSpace(state.Active) && !string.IsNullOrWhiteSpace(state.Condition))
            {
                propertyStates.Add((state.Active, state.Condition));
            }
        }

        if (propertyStates.Count == 0)
        {
            return ("false", "true");
        }

        var anyActive = BuildAnyActiveExpression(propertyStates);
        var andExpression = BuildAndExpression(propertyStates);
        var orExpression = BuildOrExpression(propertyStates);

        return (anyActive, $"!({anyActive}) || ({filterPath}.CombineWith == CombineType.Or ? ({orExpression}) : ({andExpression}))");
    }

    private static (string Active, string Condition) BuildPropertyState(IPropertySymbol filterProp, INamedTypeSymbol entitySymbol, string paramName, string filterPath)
    {
        if (filterProp.GetAttributes().Any(a => a.AttributeClass?.Name == "IgnoreFilterAttribute"))
        {
            return default;
        }

        var compareToAttrs = filterProp.GetAttributes().Where(a => a.AttributeClass?.Name == "CompareToAttribute").ToArray();
        var collectionFilterAttr = filterProp.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "CollectionFilterAttribute");
        var filterRef = $"{filterPath}.{filterProp.Name}";

        if (compareToAttrs.Length == 0)
        {
            var targetProp = ResolveMemberByPath(entitySymbol, filterProp.Name);
            if (targetProp == null)
            {
                return default;
            }

            return BuildTargetState(filterProp, targetProp, filterProp.Name, paramName, filterRef, null, collectionFilterAttr);
        }

        string combinedCondition = null;
        var activeParts = new List<string>();

        foreach (var compareToAttr in compareToAttrs)
        {
            var targetStates = new List<(string Active, string Condition)>();

            foreach (var targetPath in ExtractStringTargets(compareToAttr))
            {
                var targetProp = ResolveMemberByPath(entitySymbol, targetPath);
                if (targetProp == null)
                {
                    continue;
                }

                var state = BuildTargetState(filterProp, targetProp, targetPath, paramName, filterRef, compareToAttr, collectionFilterAttr);
                if (!string.IsNullOrWhiteSpace(state.Active) && !string.IsNullOrWhiteSpace(state.Condition))
                {
                    targetStates.Add(state);
                }
            }

            if (targetStates.Count == 0)
            {
                continue;
            }

            activeParts.AddRange(targetStates.Select(x => $"({x.Active})"));

            var attributeCondition = CombineStates(targetStates.Select(x => x.Condition), GetCompareCombineOperator(compareToAttr));
            combinedCondition = CombineStates(new[] { combinedCondition, attributeCondition }.Where(x => !string.IsNullOrWhiteSpace(x)), GetCompareCombineOperator(compareToAttr));
        }

        if (string.IsNullOrWhiteSpace(combinedCondition) || activeParts.Count == 0)
        {
            return default;
        }

        return (string.Join(" || ", activeParts), combinedCondition);
    }

    private static (string Active, string Condition) BuildTargetState(IPropertySymbol filterProp, IPropertySymbol targetProp, string targetPath, string paramName, string filterRef, AttributeData compareToAttr, AttributeData collectionFilterAttr)
    {
        if (collectionFilterAttr != null)
        {
            return BuildCollectionState(filterProp, targetProp, targetPath, paramName, filterRef, collectionFilterAttr);
        }

        if (IsFilterType(filterProp.Type) && IsCollectionType(targetProp.Type))
        {
            return BuildCollectionState(filterProp, targetProp, targetPath, paramName, filterRef, null);
        }

        if (IsFilterType(filterProp.Type))
        {
            return BuildNestedFilterState(filterProp, targetProp, targetPath, paramName, filterRef);
        }

        if (filterProp.Type is IArrayTypeSymbol arrayType && !IsCollectionType(targetProp.Type))
        {
            return BuildConditionState($"{filterRef} != null", WrapWithGuard($"{filterRef}.Contains({BuildMemberAccess(paramName, targetPath)})", BuildPathGuard(paramName, targetPath, includeLeaf: false)));
        }

        var attributeState = BuildAttributeState(filterProp, targetProp, targetPath, paramName, filterRef, compareToAttr);
        if (!string.IsNullOrWhiteSpace(attributeState.Active) && !string.IsNullOrWhiteSpace(attributeState.Condition))
        {
            return attributeState;
        }

        if (compareToAttr != null && ExtractFilterableType(compareToAttr) != null)
        {
            return default;
        }

        var targetRef = BuildMemberAccess(paramName, targetPath);
        var pathGuard = BuildPathGuard(paramName, targetPath, includeLeaf: false);

        if (filterProp.Type is INamedTypeSymbol namedStringFilter && namedStringFilter.ToDisplayString().StartsWith("AutoFilterer.Types.StringFilter"))
        {
            return BuildConditionState($"{filterRef} != null && ({BuildStringFilterActiveExpression(filterRef)})", BuildStringFilterConditionExpression(filterRef, targetRef, pathGuard));
        }

        if (filterProp.Type is INamedTypeSymbol namedRange && namedRange.Name == "Range")
        {
            var active = $"{filterRef} != null && ({filterRef}.Min != null || {filterRef}.Max != null)";
            var condition = BuildRangeConditionExpression(filterRef, targetRef, pathGuard);
            return BuildConditionState(active, condition);
        }

        if (filterProp.Type is INamedTypeSymbol namedOperator && namedOperator.Name == "OperatorFilter")
        {
            return BuildConditionState($"{filterRef} != null && ({BuildOperatorFilterActiveExpression(filterRef, SupportsNullChecks(targetProp.Type))})", BuildOperatorFilterConditionExpression(filterRef, targetRef, pathGuard, SupportsNullChecks(targetProp.Type)));
        }

        if (filterProp.Type.SpecialType == SpecialType.System_String)
        {
            return BuildConditionState($"{filterRef} != null", WrapWithGuard($"{targetRef} == {filterRef}", pathGuard));
        }

        if (filterProp.Type is INamedTypeSymbol namedFilterProp && (namedFilterProp.TypeKind == TypeKind.Enum || namedFilterProp.IsValueType))
        {
            if (IsNullableValueType(filterProp.Type))
            {
                return BuildConditionState($"{filterRef} != null", WrapWithGuard($"{targetRef} == {filterRef}", pathGuard));
            }

            return BuildConditionState("true", WrapWithGuard($"{targetRef}.Equals({filterRef})", pathGuard));
        }

        return default;
    }

    private static (string Active, string Condition) BuildCollectionState(IPropertySymbol filterProp, IPropertySymbol targetProp, string targetPath, string paramName, string filterRef, AttributeData collectionFilterAttr)
    {
        if (filterProp.Type is not INamedTypeSymbol nestedFilterType || targetProp.Type is not INamedTypeSymbol collectionType || !collectionType.IsGenericType)
        {
            return default;
        }

        var elementType = collectionType.TypeArguments.FirstOrDefault() as INamedTypeSymbol;
        if (elementType == null)
        {
            return default;
        }

        var collectionAccess = BuildMemberAccess(paramName, targetPath);
        var filterOption = GetCollectionFilterOption(collectionFilterAttr);
        var collectionQuery = $"{collectionAccess}.AsQueryable()";
        var filteredQuery = $"{GetApplyHelperName(nestedFilterType)}({collectionQuery}, {filterRef})";
        var condition = filterOption == "All"
            ? $"(!{collectionQuery}.Any() || {filteredQuery}.Count() == {collectionQuery}.Count())"
            : $"{filteredQuery}.Any()";

        return BuildConditionState($"{filterRef} != null && {GetHasFiltersHelperName(nestedFilterType)}({filterRef})", WrapWithGuard(condition, BuildPathGuard(paramName, targetPath, includeLeaf: false)));
    }

    private static (string Active, string Condition) BuildNestedFilterState(IPropertySymbol filterProp, IPropertySymbol targetProp, string targetPath, string paramName, string filterRef)
    {
        if (filterProp.Type is not INamedTypeSymbol nestedFilterType || targetProp.Type is not INamedTypeSymbol nestedEntityType)
        {
            return default;
        }

        var nestedState = BuildFilterState(nestedFilterType, nestedEntityType, BuildMemberAccess(paramName, targetPath), filterRef);
        if (nestedState.Active == "false")
        {
            return default;
        }

        return BuildConditionState($"{filterRef} != null && ({nestedState.Active})", nestedState.Condition);
    }

    private static (string Active, string Condition) BuildAttributeState(IPropertySymbol filterProp, IPropertySymbol targetProp, string targetPath, string paramName, string filterRef, AttributeData compareToAttr)
    {
        var targetRef = BuildMemberAccess(paramName, targetPath);
        var pathGuard = BuildPathGuard(paramName, targetPath, includeLeaf: false);

        if (compareToAttr != null)
        {
            var filterableType = ExtractFilterableType(compareToAttr);
            if (filterableType != null)
            {
                return BuildCustomFilterableState(filterableType, filterRef, targetRef, pathGuard);
            }
        }

        var inlineAttribute = filterProp.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name is not "CompareToAttribute" and not "CollectionFilterAttribute" and not "IgnoreFilterAttribute");

        if (inlineAttribute?.AttributeClass?.Name == "StringFilterOptionsAttribute")
        {
            var method = GetStringFilterMethod(inlineAttribute);
            if (!string.IsNullOrWhiteSpace(method))
            {
                return BuildConditionState($"{filterRef} != null", BuildStringMethodCondition(targetRef, filterRef, pathGuard, method, GetStringComparisonExpression(inlineAttribute)));
            }
        }

        if (inlineAttribute?.AttributeClass?.Name == "OperatorComparisonAttribute")
        {
            return BuildOperatorComparisonState(inlineAttribute, filterRef, targetRef, pathGuard, SupportsNullChecks(targetProp.Type));
        }

        return default;
    }

    private static (string Active, string Condition) BuildCustomFilterableState(INamedTypeSymbol filterableType, string filterRef, string targetRef, string pathGuard)
    {
        if (filterableType.Name == "ToLowerContainsComparisonAttribute")
        {
            return BuildConditionState($"{filterRef} != null", WrapWithGuard($"{targetRef}.ToLower().Contains({filterRef}.ToLower())", pathGuard));
        }

        if (filterableType.Name == "ToLowerEqualsComparisonAttribute")
        {
            return BuildConditionState($"{filterRef} != null", WrapWithGuard($"{targetRef}.ToLower().Equals({filterRef}.ToLower())", pathGuard));
        }

        if (InheritsFrom(filterableType, "StringFilterOptionsAttribute", "AutoFilterer.Attributes"))
        {
            var method = InferMethodFromName(filterableType.Name);
            if (!string.IsNullOrWhiteSpace(method))
            {
                return BuildConditionState($"{filterRef} != null", BuildStringMethodCondition(targetRef, filterRef, pathGuard, method, "StringComparison.InvariantCultureIgnoreCase"));
            }
        }

        return default;
    }

    private static (string Active, string Condition) BuildOperatorComparisonState(AttributeData attribute, string filterRef, string targetRef, string pathGuard, bool supportsNullChecks)
    {
        var operatorName = GetOperatorName(attribute);
        if (string.IsNullOrWhiteSpace(operatorName))
        {
            return default;
        }

        return operatorName switch
        {
            "Equal" => BuildConditionState($"{filterRef} != null", WrapWithGuard($"{targetRef} == {filterRef}", pathGuard)),
            "NotEqual" => BuildConditionState($"{filterRef} != null", WrapWithGuard($"{targetRef} != {filterRef}", pathGuard)),
            "GreaterThan" => BuildConditionState($"{filterRef} != null", WrapWithGuard($"{targetRef} > {filterRef}", pathGuard)),
            "GreaterThanOrEqual" => BuildConditionState($"{filterRef} != null", WrapWithGuard($"{targetRef} >= {filterRef}", pathGuard)),
            "LessThan" => BuildConditionState($"{filterRef} != null", WrapWithGuard($"{targetRef} < {filterRef}", pathGuard)),
            "LessThanOrEqual" => BuildConditionState($"{filterRef} != null", WrapWithGuard($"{targetRef} <= {filterRef}", pathGuard)),
            "IsNull" when supportsNullChecks => BuildConditionState("true", WrapWithGuard($"{targetRef} == null", pathGuard)),
            "IsNotNull" when supportsNullChecks => BuildConditionState("true", WrapWithGuard($"{targetRef} != null", pathGuard)),
            _ => default
        };
    }

    private static string BuildRangeConditionExpression(string filterRef, string targetRef, string pathGuard)
    {
        var conditions = new List<string>
        {
            $"({filterRef}.Min == null || {WrapWithGuard($"{targetRef} >= {filterRef}.Min", pathGuard)})",
            $"({filterRef}.Max == null || {WrapWithGuard($"{targetRef} <= {filterRef}.Max", pathGuard)})"
        };

        return string.Join(" && ", conditions);
    }

    private static string BuildStringFilterActiveExpression(string filterRef)
    {
        return BuildAnyActiveExpression(GetStringFilterConditions(filterRef).Select(x => (x.Active, x.Expression)));
    }

    private static string BuildStringFilterConditionExpression(string filterRef, string targetRef, string pathGuard)
    {
        var conditions = GetStringFilterConditions(filterRef)
            .Select(x => (x.Active, Expression: ApplyStringCondition(x.Expression, targetRef, filterRef, pathGuard)))
            .ToList();

        var andExpression = BuildAndExpression(conditions);
        var orExpression = BuildOrExpression(conditions);
        return $"({filterRef}.CombineWith == CombineType.Or ? ({orExpression}) : ({andExpression}))";
    }

    private static string BuildOperatorFilterActiveExpression(string filterRef, bool supportsNullChecks)
    {
        return BuildAnyActiveExpression(GetOperatorFilterConditions(filterRef, supportsNullChecks).Select(x => (x.Active, x.Expression)));
    }

    private static string BuildOperatorFilterConditionExpression(string filterRef, string targetRef, string pathGuard, bool supportsNullChecks)
    {
        var conditions = GetOperatorFilterConditions(filterRef, supportsNullChecks)
            .Select(x => (x.Active, Expression: WrapWithGuard(x.Expression.Replace("{target}", targetRef), pathGuard)))
            .ToList();

        var andExpression = BuildAndExpression(conditions);
        var orExpression = BuildOrExpression(conditions);
        return $"({filterRef}.CombineWith == CombineType.Or ? ({orExpression}) : ({andExpression}))";
    }

    private static void EmitOrderingAndPagination(StringBuilder sb, INamedTypeSymbol filterSymbol, INamedTypeSymbol entitySymbol)
    {
        var sortProp = FindPropertyIncludingBase(filterSymbol, "Sort");
        if (sortProp != null && sortProp.Type.SpecialType == SpecialType.System_String)
        {
            sb.AppendLine("            if (!string.IsNullOrEmpty(filter.Sort))");
            sb.AppendLine("            {");
            sb.AppendLine("                switch (filter.Sort)");
            sb.AppendLine("                {");
            foreach (var sortPath in EnumerateSortablePaths(entitySymbol).Distinct(StringComparer.Ordinal))
            {
                sb.AppendLine($"                    case \"{sortPath}\":");
                sb.AppendLine("                    {");
                var sortByProp = FindPropertyIncludingBase(filterSymbol, "SortBy");
                if (sortByProp != null)
                {
                    sb.AppendLine("                        if (filter.SortBy == Sorting.Descending)");
                    sb.AppendLine($"                            source = source.OrderByDescending(x => x.{sortPath});");
                    sb.AppendLine("                        else");
                    sb.AppendLine($"                            source = source.OrderBy(x => x.{sortPath});");
                }
                else
                {
                    sb.AppendLine($"                        source = source.OrderBy(x => x.{sortPath});");
                }
                sb.AppendLine("                        break;");
                sb.AppendLine("                    }");
            }
            sb.AppendLine("                    default: throw new ArgumentException(\"Invalid Sort field\", nameof(filter.Sort));");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
        }

        var pageProp = FindPropertyIncludingBase(filterSymbol, "Page");
        var perPageProp = FindPropertyIncludingBase(filterSymbol, "PerPage");
        if (pageProp != null && perPageProp != null)
        {
            sb.AppendLine("            if (filter.Page > 0 && filter.PerPage > 0)");
            sb.AppendLine("            {");
            sb.AppendLine("                source = AutoFilterer.Extensions.QueryExtensions.ToPaged(source, filter.Page, filter.PerPage);");
            sb.AppendLine("            }");
        }
    }

    private static void EmitNestedFilterHelpers(StringBuilder sb, INamedTypeSymbol filterSymbol, INamedTypeSymbol entitySymbol, HashSet<string> emitted)
    {
        foreach (var filterProp in filterSymbol.GetMembers().OfType<IPropertySymbol>().Where(p => !p.IsStatic && !p.IsIndexer))
        {
            if (!IsFilterType(filterProp.Type))
            {
                continue;
            }

            var compareToAttrs = filterProp.GetAttributes().Where(a => a.AttributeClass?.Name == "CompareToAttribute").ToArray();
            if (compareToAttrs.Length == 0)
            {
                var defaultTarget = ResolveMemberByPath(entitySymbol, filterProp.Name);
                EmitNestedFilterHelperIfNeeded(sb, filterProp, defaultTarget, emitted);
                continue;
            }

            foreach (var compareToAttr in compareToAttrs)
            {
                foreach (var targetPath in ExtractStringTargets(compareToAttr))
                {
                    var targetProp = ResolveMemberByPath(entitySymbol, targetPath);
                    EmitNestedFilterHelperIfNeeded(sb, filterProp, targetProp, emitted);
                }
            }
        }
    }

    private static void EmitNestedFilterHelperIfNeeded(StringBuilder sb, IPropertySymbol filterProp, IPropertySymbol targetProp, HashSet<string> emitted)
    {
        if (targetProp == null || filterProp.Type is not INamedTypeSymbol nestedFilterType)
        {
            return;
        }

        if (targetProp.Type is not INamedTypeSymbol namedTargetType)
        {
            return;
        }

        INamedTypeSymbol nestedEntityType;
        if (IsCollectionType(targetProp.Type))
        {
            if (!namedTargetType.IsGenericType || namedTargetType.TypeArguments.FirstOrDefault() is not INamedTypeSymbol elementType)
            {
                return;
            }

            nestedEntityType = elementType;
        }
        else
        {
            nestedEntityType = namedTargetType;
        }

        var key = nestedFilterType.ToDisplayString() + "->" + nestedEntityType.ToDisplayString();
        if (!emitted.Add(key))
        {
            return;
        }

        EmitNestedFilterHelpers(sb, nestedFilterType, nestedEntityType, emitted);
        EmitHasFiltersHelper(sb, nestedFilterType);
        EmitApplyNestedHelper(sb, nestedFilterType, nestedEntityType);
    }

    private static void EmitHasFiltersHelper(StringBuilder sb, INamedTypeSymbol filterSymbol)
    {
        sb.AppendLine();
        sb.AppendLine($"        private static bool {GetHasFiltersHelperName(filterSymbol)}({filterSymbol.ToDisplayString()} filter)");
        sb.AppendLine("        {");
        sb.AppendLine("            if (filter == null)");
        sb.AppendLine("            {");
        sb.AppendLine("                return false;");
        sb.AppendLine("            }");

        foreach (var prop in filterSymbol.GetMembers().OfType<IPropertySymbol>().Where(p => !p.IsStatic && !p.IsIndexer))
        {
            if (prop.GetAttributes().Any(a => a.AttributeClass?.Name == "IgnoreFilterAttribute"))
            {
                continue;
            }

            var filterRef = $"filter.{prop.Name}";

            if (prop.Type is INamedTypeSymbol namedStringFilter && namedStringFilter.ToDisplayString().StartsWith("AutoFilterer.Types.StringFilter"))
            {
                sb.AppendLine($"            if ({filterRef} != null && ({BuildStringFilterActiveExpression(filterRef)})) return true;");
                continue;
            }

            if (prop.Type is INamedTypeSymbol namedOperator && namedOperator.Name == "OperatorFilter")
            {
                sb.AppendLine($"            if ({filterRef} != null && ({BuildOperatorFilterActiveExpression(filterRef, true)})) return true;");
                continue;
            }

            if (prop.Type is INamedTypeSymbol namedRange && namedRange.Name == "Range")
            {
                sb.AppendLine($"            if ({filterRef} != null && ({filterRef}.Min != null || {filterRef}.Max != null)) return true;");
                continue;
            }

            if (prop.Type is IArrayTypeSymbol)
            {
                sb.AppendLine($"            if ({filterRef} != null) return true;");
                continue;
            }

            if (IsFilterType(prop.Type) && prop.Type is INamedTypeSymbol nestedFilterType)
            {
                sb.AppendLine($"            if ({filterRef} != null && {GetHasFiltersHelperName(nestedFilterType)}({filterRef})) return true;");
                continue;
            }

            if (prop.Type.SpecialType == SpecialType.System_String || IsNullableValueType(prop.Type))
            {
                sb.AppendLine($"            if ({filterRef} != null) return true;");
                continue;
            }

            if (prop.Type is INamedTypeSymbol namedProp && (namedProp.TypeKind == TypeKind.Enum || namedProp.IsValueType))
            {
                sb.AppendLine("            return true;");
                sb.AppendLine("        }");
                return;
            }
        }

        sb.AppendLine("            return false;");
        sb.AppendLine("        }");
    }

    private static void EmitApplyNestedHelper(StringBuilder sb, INamedTypeSymbol filterSymbol, INamedTypeSymbol entitySymbol)
    {
        sb.AppendLine();
        sb.AppendLine($"        private static IQueryable<{entitySymbol.ToDisplayString()}> {GetApplyHelperName(filterSymbol)}(IQueryable<{entitySymbol.ToDisplayString()}> source, {filterSymbol.ToDisplayString()} filter)");
        sb.AppendLine("        {");
        sb.AppendLine("            if (filter == null) return source;");
        EmitParityPrefilters(sb, filterSymbol, entitySymbol);
        var filterState = BuildFilterState(filterSymbol, entitySymbol, "x", "filter");
        if (!string.IsNullOrWhiteSpace(filterState.Condition) && filterState.Condition != "true")
        {
            sb.AppendLine($"            source = source.Where(x => {filterState.Condition});");
        }
        EmitOrderingAndPagination(sb, filterSymbol, entitySymbol);
        sb.AppendLine("            return source;");
        sb.AppendLine("        }");
    }

    private static string GetApplyHelperName(INamedTypeSymbol filterSymbol)
    {
        return "ApplyGenerated_" + filterSymbol.Name;
    }

    private static string GetHasFiltersHelperName(INamedTypeSymbol filterSymbol)
    {
        return "HasFilters_" + filterSymbol.Name;
    }

    private static IEnumerable<(string Active, string Expression)> GetStringFilterConditions(string filterRef)
    {
        yield return ($"{filterRef}.Eq != null", "{target} == " + filterRef + ".Eq");
        yield return ($"{filterRef}.Not != null", "{target} != " + filterRef + ".Not");
        yield return ($"{filterRef}.Equals != null", "STRING_EQUALS");
        yield return ($"{filterRef}.Contains != null", "STRING_CONTAINS");
        yield return ($"{filterRef}.NotContains != null", "NOT_STRING_CONTAINS");
        yield return ($"{filterRef}.StartsWith != null", "STRING_STARTSWITH");
        yield return ($"{filterRef}.NotStartsWith != null", "NOT_STRING_STARTSWITH");
        yield return ($"{filterRef}.EndsWith != null", "STRING_ENDSWITH");
        yield return ($"{filterRef}.NotEndsWith != null", "NOT_STRING_ENDSWITH");
        yield return ($"{filterRef}.IsNull != null", $"({filterRef}.IsNull.Value ? {{target}} == null : {{target}} != null)");
        yield return ($"{filterRef}.IsNotNull != null", $"({filterRef}.IsNotNull.Value ? {{target}} != null : {{target}} == null)");
        yield return ($"{filterRef}.IsEmpty != null", "IS_EMPTY");
        yield return ($"{filterRef}.IsNotEmpty != null", "IS_NOT_EMPTY");
    }

    private static IEnumerable<(string Active, string Expression)> GetOperatorFilterConditions(string filterRef, bool supportsNullChecks)
    {
        yield return ($"{filterRef}.Eq != null", "{target} == " + filterRef + ".Eq");
        yield return ($"{filterRef}.Not != null", "{target} != " + filterRef + ".Not");
        yield return ($"{filterRef}.Gt != null", "{target} > " + filterRef + ".Gt");
        yield return ($"{filterRef}.Lt != null", "{target} < " + filterRef + ".Lt");
        yield return ($"{filterRef}.Gte != null", "{target} >= " + filterRef + ".Gte");
        yield return ($"{filterRef}.Lte != null", "{target} <= " + filterRef + ".Lte");

        if (supportsNullChecks)
        {
            yield return ($"{filterRef}.IsNull != null", $"({filterRef}.IsNull.Value ? {{target}} == null : {{target}} != null)");
            yield return ($"{filterRef}.IsNotNull != null", $"({filterRef}.IsNotNull.Value ? {{target}} != null : {{target}} == null)");
        }
    }

    private static string ApplyStringCondition(string template, string targetRef, string filterRef, string pathGuard)
    {
        return template switch
        {
            "STRING_EQUALS" => BuildDynamicStringMethodCondition(targetRef, $"{filterRef}.Equals", pathGuard, "Equals", $"{filterRef}.Compare"),
            "STRING_CONTAINS" => BuildDynamicStringMethodCondition(targetRef, $"{filterRef}.Contains", pathGuard, "Contains", $"{filterRef}.Compare"),
            "NOT_STRING_CONTAINS" => $"!({BuildDynamicStringMethodCondition(targetRef, $"{filterRef}.NotContains", pathGuard, "Contains", $"{filterRef}.Compare")})",
            "STRING_STARTSWITH" => BuildDynamicStringMethodCondition(targetRef, $"{filterRef}.StartsWith", pathGuard, "StartsWith", $"{filterRef}.Compare"),
            "NOT_STRING_STARTSWITH" => $"!({BuildDynamicStringMethodCondition(targetRef, $"{filterRef}.NotStartsWith", pathGuard, "StartsWith", $"{filterRef}.Compare")})",
            "STRING_ENDSWITH" => BuildDynamicStringMethodCondition(targetRef, $"{filterRef}.EndsWith", pathGuard, "EndsWith", $"{filterRef}.Compare"),
            "NOT_STRING_ENDSWITH" => $"!({BuildDynamicStringMethodCondition(targetRef, $"{filterRef}.NotEndsWith", pathGuard, "EndsWith", $"{filterRef}.Compare")})",
            "IS_EMPTY" => $"({filterRef}.IsEmpty.Value ? {BuildDynamicStringMethodCondition(targetRef, "string.Empty", pathGuard, "Equals", $"{filterRef}.Compare", valueIsLiteral: true)} : !({BuildDynamicStringMethodCondition(targetRef, "string.Empty", pathGuard, "Equals", $"{filterRef}.Compare", valueIsLiteral: true)}))",
            "IS_NOT_EMPTY" => $"({filterRef}.IsNotEmpty.Value ? !({BuildDynamicStringMethodCondition(targetRef, "string.Empty", pathGuard, "Equals", $"{filterRef}.Compare", valueIsLiteral: true)}) : {BuildDynamicStringMethodCondition(targetRef, "string.Empty", pathGuard, "Equals", $"{filterRef}.Compare", valueIsLiteral: true)})",
            _ => WrapWithGuard(template.Replace("{target}", targetRef), pathGuard)
        };
    }

    private static string BuildDynamicStringMethodCondition(string targetRef, string valueRef, string pathGuard, string methodName, string comparisonRef, bool valueIsLiteral = false)
    {
        var valueExpression = valueIsLiteral ? valueRef : valueRef;
        var withComparison = BuildStringMethodCondition(targetRef, valueExpression, pathGuard, methodName, $"{comparisonRef}.Value");
        var withoutComparison = BuildStringMethodCondition(targetRef, valueExpression, pathGuard, methodName, null);
        return $"({comparisonRef} != null ? {withComparison} : {withoutComparison})";
    }

    private static string BuildStringMethodCondition(string targetRef, string valueRef, string pathGuard, string methodName, string comparisonExpression)
    {
        var methodCall = string.IsNullOrWhiteSpace(comparisonExpression)
            ? BuildStringMethodInvocation(targetRef, methodName, valueRef)
            : BuildStringMethodInvocation(targetRef, methodName, valueRef, comparisonExpression);

        return WrapWithGuard($"{targetRef} != null && {methodCall}", pathGuard);
    }

    private static string BuildStringMethodInvocation(string targetRef, string methodName, string valueRef, string comparisonExpression = null)
    {
        return string.IsNullOrWhiteSpace(comparisonExpression)
            ? $"{targetRef}.{methodName}({valueRef})"
            : $"{targetRef}.{methodName}({valueRef}, {comparisonExpression})";
    }

    private static string BuildPathGuard(string paramName, string targetPath, bool includeLeaf)
    {
        var parts = targetPath.Split('.');
        var count = includeLeaf ? parts.Length : parts.Length - 1;
        if (count <= 0)
        {
            return null;
        }

        return string.Join(" && ", Enumerable.Range(1, count).Select(i => BuildMemberAccess(paramName, string.Join(".", parts.Take(i))) + " != null"));
    }

    private static string BuildMemberAccess(string paramName, string targetPath)
    {
        return string.IsNullOrWhiteSpace(targetPath) ? paramName : $"{paramName}.{targetPath}";
    }

    private static string WrapWithGuard(string condition, string pathGuard)
    {
        if (string.IsNullOrWhiteSpace(pathGuard))
        {
            return condition;
        }

        return $"({pathGuard} && ({condition}))";
    }

    private static string CombineStates(IEnumerable<string> expressions, string combineOperator)
    {
        var parts = expressions.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => $"({x})").ToArray();
        if (parts.Length == 0)
        {
            return null;
        }

        if (parts.Length == 1)
        {
            return parts[0];
        }

        var op = string.Equals(combineOperator, "And", StringComparison.Ordinal) ? " && " : " || ";
        return string.Join(op, parts);
    }

    private static (string Active, string Condition) BuildConditionState(string active, string condition)
    {
        return string.IsNullOrWhiteSpace(active) || string.IsNullOrWhiteSpace(condition) ? default : (active, condition);
    }

    private static string GetCompareCombineOperator(AttributeData compareToAttr)
    {
        var combine = compareToAttr.NamedArguments.FirstOrDefault(x => x.Key == "CombineWith").Value;
        return combine.Value?.ToString() == "0" ? "And" : "Or";
    }

    private static INamedTypeSymbol ExtractFilterableType(AttributeData compareToAttr)
    {
        foreach (var argument in compareToAttr.ConstructorArguments)
        {
            if (argument.Kind != TypedConstantKind.Array && argument.Value is INamedTypeSymbol namedType)
            {
                return namedType;
            }
        }

        return null;
    }

    private static string GetCollectionFilterOption(AttributeData collectionFilterAttr)
    {
        if (collectionFilterAttr == null)
        {
            return "Any";
        }

        var filterOptionArg = collectionFilterAttr.ConstructorArguments.Length > 0 ? collectionFilterAttr.ConstructorArguments[0] : default;
        if (filterOptionArg.Value != null)
        {
            return filterOptionArg.Value.ToString() == "0" ? "Any" : "All";
        }

        var namedArg = collectionFilterAttr.NamedArguments.FirstOrDefault(kv => kv.Key == "FilterOption").Value;
        return namedArg.Value?.ToString() == "1" ? "All" : "Any";
    }

    private static bool SupportsNullChecks(ITypeSymbol type)
    {
        return IsNullableValueType(type) || type.SpecialType == SpecialType.System_String;
    }

    private static bool IsCollectionType(ITypeSymbol type)
    {
        return type is INamedTypeSymbol named && IsCollectionLike(named);
    }

    private static bool InheritsFrom(INamedTypeSymbol type, string baseTypeName, string baseNamespace)
    {
        for (var current = type; current != null; current = current.BaseType)
        {
            if (current.Name == baseTypeName && current.ContainingNamespace?.ToDisplayString() == baseNamespace)
            {
                return true;
            }
        }

        return false;
    }

    private static string InferMethodFromName(string typeName)
    {
        if (typeName.Contains("StartsWith", StringComparison.Ordinal)) return "StartsWith";
        if (typeName.Contains("EndsWith", StringComparison.Ordinal)) return "EndsWith";
        if (typeName.Contains("Contains", StringComparison.Ordinal)) return "Contains";
        if (typeName.Contains("Equals", StringComparison.Ordinal)) return "Equals";
        return null;
    }

    private static string GetStringFilterMethod(AttributeData attribute)
    {
        var optionArg = attribute.ConstructorArguments.Length > 0 ? attribute.ConstructorArguments[0].Value?.ToString() : null;
        return optionArg switch
        {
            "0" => "Equals",
            "1" => "StartsWith",
            "2" => "EndsWith",
            "4" => "Contains",
            _ => null
        };
    }

    private static string GetStringComparisonExpression(AttributeData attribute)
    {
        var comparisonArg = attribute.ConstructorArguments.Length > 1 ? attribute.ConstructorArguments[1].Value?.ToString() : null;
        if (!string.IsNullOrWhiteSpace(comparisonArg))
        {
            return $"(StringComparison){comparisonArg}";
        }

        var namedComparison = attribute.NamedArguments.FirstOrDefault(x => x.Key == "Comparison").Value;
        return namedComparison.Value == null ? null : $"(StringComparison){namedComparison.Value}";
    }

    private static string GetOperatorName(AttributeData attribute)
    {
        var operatorArg = attribute.ConstructorArguments.Length > 0 ? attribute.ConstructorArguments[0].Value?.ToString() : null;
        return operatorArg switch
        {
            "0" => "Equal",
            "1" => "NotEqual",
            "2" => "GreaterThan",
            "3" => "GreaterThanOrEqual",
            "4" => "LessThan",
            "5" => "LessThanOrEqual",
            "6" => "IsNull",
            "7" => "IsNotNull",
            _ => null
        };
    }

    private static string BuildStringFilterPredicate(string filterRef, string targetRef)
    {
        var conditions = new List<(string Active, string Expression)>
        {
            ($"{filterRef}.Eq != null", $"{targetRef} == {filterRef}.Eq"),
            ($"{filterRef}.Not != null", $"{targetRef} != {filterRef}.Not"),
            ($"{filterRef}.Equals != null", BuildGuardedStringComparison(targetRef, "Equals", $"{filterRef}.Equals", filterRef)),
            ($"{filterRef}.Contains != null", BuildGuardedStringComparison(targetRef, "Contains", $"{filterRef}.Contains", filterRef)),
            ($"{filterRef}.NotContains != null", $"!({BuildGuardedStringComparison(targetRef, "Contains", $"{filterRef}.NotContains", filterRef)})"),
            ($"{filterRef}.StartsWith != null", BuildGuardedStringComparison(targetRef, "StartsWith", $"{filterRef}.StartsWith", filterRef)),
            ($"{filterRef}.NotStartsWith != null", $"!({BuildGuardedStringComparison(targetRef, "StartsWith", $"{filterRef}.NotStartsWith", filterRef)})"),
            ($"{filterRef}.EndsWith != null", BuildGuardedStringComparison(targetRef, "EndsWith", $"{filterRef}.EndsWith", filterRef)),
            ($"{filterRef}.NotEndsWith != null", $"!({BuildGuardedStringComparison(targetRef, "EndsWith", $"{filterRef}.NotEndsWith", filterRef)})"),
            ($"{filterRef}.IsNull != null", $"({filterRef}.IsNull.Value ? {targetRef} == null : {targetRef} != null)"),
            ($"{filterRef}.IsNotNull != null", $"({filterRef}.IsNotNull.Value ? {targetRef} != null : {targetRef} == null)"),
            ($"{filterRef}.IsEmpty != null", $"({filterRef}.IsEmpty.Value ? {BuildGuardedStringComparison(targetRef, "Equals", "string.Empty", filterRef)} : !({BuildGuardedStringComparison(targetRef, "Equals", "string.Empty", filterRef)}))"),
            ($"{filterRef}.IsNotEmpty != null", $"({filterRef}.IsNotEmpty.Value ? !({BuildGuardedStringComparison(targetRef, "Equals", "string.Empty", filterRef)}) : {BuildGuardedStringComparison(targetRef, "Equals", "string.Empty", filterRef)})")
        };

        var anyActive = BuildAnyActiveExpression(conditions);
        var andExpression = BuildAndExpression(conditions);
        var orExpression = BuildOrExpression(conditions);

        return $"{filterRef} == null || (!({anyActive}) || ({filterRef}.CombineWith == CombineType.Or ? ({orExpression}) : ({andExpression})))";
    }

    private static string BuildOperatorFilterPredicate(string filterRef, string targetRef, bool supportsNullChecks)
    {
        var conditions = new List<(string Active, string Expression)>
        {
            ($"{filterRef}.Eq != null", $"{targetRef} == {filterRef}.Eq"),
            ($"{filterRef}.Not != null", $"{targetRef} != {filterRef}.Not"),
            ($"{filterRef}.Gt != null", $"{targetRef} > {filterRef}.Gt"),
            ($"{filterRef}.Lt != null", $"{targetRef} < {filterRef}.Lt"),
            ($"{filterRef}.Gte != null", $"{targetRef} >= {filterRef}.Gte"),
            ($"{filterRef}.Lte != null", $"{targetRef} <= {filterRef}.Lte")
        };

        if (supportsNullChecks)
        {
            conditions.Add(($"{filterRef}.IsNull != null", $"({filterRef}.IsNull.Value ? {targetRef} == null : {targetRef} != null)"));
            conditions.Add(($"{filterRef}.IsNotNull != null", $"({filterRef}.IsNotNull.Value ? {targetRef} != null : {targetRef} == null)"));
        }

        var anyActive = BuildAnyActiveExpression(conditions);
        var andExpression = BuildAndExpression(conditions);
        var orExpression = BuildOrExpression(conditions);

        return $"{filterRef} == null || (!({anyActive}) || ({filterRef}.CombineWith == CombineType.Or ? ({orExpression}) : ({andExpression})))";
    }

    private static string BuildGuardedStringComparison(string targetRef, string methodName, string valueRef, string filterRef)
    {
        return $"{targetRef} != null && ({BuildStringComparison(targetRef, methodName, valueRef, filterRef)})";
    }

    private static string BuildStringComparison(string targetRef, string methodName, string valueRef, string filterRef)
    {
        return $"({filterRef}.Compare != null ? {targetRef}.{methodName}({valueRef}, {filterRef}.Compare.Value) : {targetRef}.{methodName}({valueRef}))";
    }

    private static string BuildAnyActiveExpression(IEnumerable<(string Active, string Expression)> conditions)
    {
        return string.Join(" || ", conditions.Select(c => $"({c.Active})"));
    }

    private static string BuildAndExpression(IEnumerable<(string Active, string Expression)> conditions)
    {
        return string.Join(" && ", conditions.Select(c => $"(!({c.Active}) || ({c.Expression}))"));
    }

    private static string BuildOrExpression(IEnumerable<(string Active, string Expression)> conditions)
    {
        return string.Join(" || ", conditions.Select(c => $"(({c.Active}) && ({c.Expression}))"));
    }

    private static IEnumerable<string> EnumerateSortablePaths(INamedTypeSymbol entitySymbol, int depth = 2, string prefix = null)
    {
        foreach (var property in entitySymbol.GetMembers().OfType<IPropertySymbol>().Where(p => !p.IsStatic && !p.IsIndexer))
        {
            var currentPath = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";

            if (IsSortableLeaf(property.Type))
            {
                yield return currentPath;
                continue;
            }

            if (depth <= 1 || property.Type is not INamedTypeSymbol named || IsCollectionLike(named))
            {
                continue;
            }

            foreach (var nestedPath in EnumerateSortablePaths(named, depth - 1, currentPath))
            {
                yield return nestedPath;
            }
        }
    }

    private static bool IsSortableLeaf(ITypeSymbol type)
    {
        if (type.SpecialType == SpecialType.System_String)
        {
            return true;
        }

        if (type.TypeKind == TypeKind.Enum || type.IsValueType)
        {
            return true;
        }

        return type is INamedTypeSymbol named && named.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
    }

    private static bool IsCollectionLike(INamedTypeSymbol type)
    {
        if (type.AllInterfaces.Any(i => i.Name == nameof(System.Collections.IEnumerable)))
        {
            return type.SpecialType != SpecialType.System_String;
        }

        return false;
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

    private static IPropertySymbol FindPropertyIncludingBase(INamedTypeSymbol typeSymbol, string propertyName)
    {
        for (var current = typeSymbol; current != null; current = current.BaseType)
        {
            var property = current.GetMembers().OfType<IPropertySymbol>()
                .FirstOrDefault(p => !p.IsStatic && !p.IsIndexer && p.Name == propertyName);

            if (property != null)
            {
                return property;
            }
        }

        return null;
    }

    private static void EmitParityPrefilters(StringBuilder sb, INamedTypeSymbol filterSymbol, INamedTypeSymbol entitySymbol)
    {
        var filterProps = filterSymbol.GetMembers().OfType<IPropertySymbol>()
            .Where(p => !p.IsStatic && !p.IsIndexer)
            .ToArray();

        foreach (var prop in filterProps.Where(p => p.Type is IArrayTypeSymbol))
        {
            sb.AppendLine($"            if (filter.{prop.Name} != null && filter.{prop.Name}.Length == 0)");
            sb.AppendLine("            {");
            sb.AppendLine("                return source.Where(_ => false);");
            sb.AppendLine("            }");
        }

        var candidateForNullEquality = filterSymbol.BaseType?.Name == "FilterBase" && filterProps.Length == 1
            ? filterProps[0]
            : null;

        if (candidateForNullEquality != null &&
            candidateForNullEquality.Type.SpecialType == SpecialType.System_String &&
            !candidateForNullEquality.GetAttributes().Any(a => a.AttributeClass?.Name == "CompareToAttribute") &&
            !candidateForNullEquality.GetAttributes().Any(a => a.AttributeClass?.Name == "StringFilterOptionsAttribute"))
        {
            var entityProp = entitySymbol.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x => x.Name == candidateForNullEquality.Name);
            if (entityProp != null && entityProp.Type.SpecialType == SpecialType.System_String)
            {
                sb.AppendLine($"            if (filter.{candidateForNullEquality.Name} == string.Empty)");
                sb.AppendLine("            {");
                sb.AppendLine($"                source = source.Where(x => x.{entityProp.Name} == string.Empty);");
                sb.AppendLine("            }");
                sb.AppendLine($"            else if (filter.{candidateForNullEquality.Name} == null)");
                sb.AppendLine("            {");
                sb.AppendLine($"                source = source.Where(x => x.{entityProp.Name} == null);");
                sb.AppendLine("            }");
            }
        }

        foreach (var prop in filterProps)
        {
            if (!TryGetSingleCompareTarget(prop, entitySymbol, out var targetPath, out var targetProp))
            {
                continue;
            }

            var nullGuards = BuildNullGuardsForPath(targetPath);

            if (!IsNullableValueType(targetProp.Type))
            {
                continue;
            }

            if (prop.Type is INamedTypeSymbol namedRange && namedRange.Name == "Range")
            {
                sb.AppendLine($"            if (filter.{prop.Name} != null && (filter.{prop.Name}.Min != null || filter.{prop.Name}.Max != null))");
                sb.AppendLine("            {");
                sb.AppendLine($"                source = source.Where(x => {nullGuards});");
                sb.AppendLine("            }");
                continue;
            }

            if (prop.Type is INamedTypeSymbol namedOperator && namedOperator.Name == "OperatorFilter")
            {
                sb.AppendLine($"            if (filter.{prop.Name} != null && (filter.{prop.Name}.Eq != null || filter.{prop.Name}.Not != null || filter.{prop.Name}.Gt != null || filter.{prop.Name}.Lt != null || filter.{prop.Name}.Gte != null || filter.{prop.Name}.Lte != null))");
                sb.AppendLine("            {");
                sb.AppendLine($"                source = source.Where(x => {nullGuards});");
                sb.AppendLine("            }");
            }
        }

        foreach (var prop in filterProps.Where(p => p.Type.SpecialType == SpecialType.System_String))
        {
            var stringFilterOptions = prop.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "StringFilterOptionsAttribute");
            var compareToAttr = prop.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "CompareToAttribute");
            if (stringFilterOptions == null || compareToAttr == null)
            {
                continue;
            }

            var optionArg = stringFilterOptions.ConstructorArguments.Length > 0 ? stringFilterOptions.ConstructorArguments[0].Value?.ToString() : null;
            if (optionArg != "4" && !string.Equals(optionArg, "Contains", StringComparison.Ordinal)) // StringFilterOption.Contains
            {
                continue;
            }

            string target = null;
            if (compareToAttr.ConstructorArguments.Length > 0)
            {
                var targetArg = compareToAttr.ConstructorArguments[0];
                target = targetArg.Kind == TypedConstantKind.Array
                    ? targetArg.Values.FirstOrDefault().Value?.ToString()
                    : targetArg.Value?.ToString();
            }
            if (string.IsNullOrWhiteSpace(target) || !target.Contains('.'))
            {
                continue;
            }

            var pathParts = target.Split('.');
            var accessPath = string.Join(".", pathParts);
            var nullGuards = string.Join(" && ", Enumerable.Range(1, pathParts.Length)
                .Select(i => "x." + string.Join(".", pathParts.Take(i)) + " != null"));

            var comparisonArg = stringFilterOptions.ConstructorArguments.Length > 1
                ? stringFilterOptions.ConstructorArguments[1].Value?.ToString()
                : null;

            var comparison = comparisonArg == null
                ? "StringComparison.InvariantCultureIgnoreCase"
                : $"(StringComparison){comparisonArg}";

            sb.AppendLine($"            if (filter.{prop.Name} != null)");
            sb.AppendLine("            {");
            sb.AppendLine($"                source = source.Where(x => {nullGuards} && x.{accessPath}.Contains(filter.{prop.Name}, {comparison}));");
            sb.AppendLine("            }");
        }
    }

    private static bool TryGetSingleCompareTarget(IPropertySymbol filterProp, INamedTypeSymbol entitySymbol, out string targetPath, out IPropertySymbol targetProp)
    {
        targetPath = null;
        targetProp = null;

        var compareToAttrs = filterProp.GetAttributes().Where(a => a.AttributeClass?.Name == "CompareToAttribute").ToArray();
        if (compareToAttrs.Length == 0)
        {
            targetProp = entitySymbol.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(ep => ep.Name == filterProp.Name);
            if (targetProp == null)
            {
                return false;
            }

            targetPath = filterProp.Name;
            return true;
        }

        if (compareToAttrs.Length != 1)
        {
            return false;
        }

        var targetNames = ExtractStringTargets(compareToAttrs[0]).ToArray();
        if (targetNames.Length != 1)
        {
            return false;
        }

        targetPath = targetNames[0];
        targetProp = ResolveMemberByPath(entitySymbol, targetPath);
        return targetProp != null;
    }

    private static IEnumerable<string> ExtractStringTargets(AttributeData attribute)
    {
        foreach (var argument in attribute.ConstructorArguments)
        {
            foreach (var targetName in ExtractTargetPropertyNames(argument))
            {
                yield return targetName;
            }
        }
    }

    private static IEnumerable<string> ExtractTargetPropertyNames(TypedConstant propNamesConst)
    {
        if (propNamesConst.Kind == TypedConstantKind.Array)
        {
            return propNamesConst.Values
                .Select(v => v.Value?.ToString())
                .Where(targetName => !string.IsNullOrWhiteSpace(targetName));
        }

        var targetName = propNamesConst.Value?.ToString();
        return string.IsNullOrWhiteSpace(targetName) ? Enumerable.Empty<string>() : new[] { targetName };
    }

    private static string BuildNullGuardsForPath(string path)
    {
        var parts = path.Split('.');
        return string.Join(" && ", Enumerable.Range(1, parts.Length)
            .Select(i => "x." + string.Join(".", parts.Take(i)) + " != null"));
    }

    private static bool IsNullableValueType(ITypeSymbol type)
    {
        return type is INamedTypeSymbol named &&
               named.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
    }

    private static void EmitCollectionFilter(StringBuilder sb, INamedTypeSymbol entitySymbol, IPropertySymbol filterProp, IPropertySymbol entityProp, AttributeData collectionFilterAttr, string parentParam)
    {
        var fpName = filterProp.Name;
        var epName = entityProp.Name;

        // Get FilterOption (Any or All)
        var filterOption = GetCollectionFilterOption(collectionFilterAttr);

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
        var implementsIFilter = type.AllInterfaces.Any(iface =>
            iface.Name == "IFilter" &&
            iface.ContainingNamespace?.ToDisplayString() == "AutoFilterer.Abstractions");
        if (implementsIFilter) return true;
        
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
                        var targetNames = ExtractTargetPropertyNames(propNamesConst);

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
                    var targetNames = ExtractTargetPropertyNames(propNamesConst);

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
        var filterOption = GetCollectionFilterOption(collectionFilterAttr);

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
                sb.AppendLine($"            if (filter.{fpName} != null) source = source.Where(x => x.{epName} == filter.{fpName});");
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
