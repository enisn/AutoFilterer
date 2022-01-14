using AutoFilterer.Generators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNurse.CodeAnalysis;

public class AttributeSyntaxReceiver<TAttribute> : ISyntaxReceiver
       where TAttribute : Attribute
{
    public IList<ClassDeclarationSyntax> Classes { get; } = new List<ClassDeclarationSyntax>();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
            classDeclarationSyntax.AttributeLists.Count > 0 &&
            classDeclarationSyntax.AttributeLists
                .Any(al => al.Attributes
                    .Any(a => a.Name.ToString().EnsureEndsWith("Attribute").Equals(typeof(TAttribute).Name))))
        {
            Classes.Add(classDeclarationSyntax);
        }
    }
}