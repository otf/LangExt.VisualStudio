using Roslyn.Compilers.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpSyntax = Roslyn.Compilers.CSharp.Syntax;

namespace LangExt.VisualStudio.Syntax
{
    public class ShadowingRewriter : SyntaxRewriter
    {
        readonly Dictionary<string, int> identities = new Dictionary<string, int>();

        public ShadowingRewriter()
            : base()
        {
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            identities.Clear();
            return base.VisitMethodDeclaration(node);
        }

        public override SyntaxNode VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            identities.Clear();
            return base.VisitAccessorDeclaration(node);
        }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (identities.ContainsKey(node.Identifier.ValueText) && 1 < identities[node.Identifier.ValueText])
                return base.VisitIdentifierName(node.WithIdentifier(CSharpSyntax.Identifier(node.Identifier.ValueText + identities[node.Identifier.ValueText])));
            else
                return base.VisitIdentifierName(node);
        }

        public override SyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            if (node.Ancestors().Any(anc => anc is FieldDeclarationSyntax))
                return base.VisitVariableDeclarator(node); ;

            if (identities.ContainsKey(node.Identifier.ValueText))
            {
                ++identities[node.Identifier.ValueText];
                return base.VisitVariableDeclarator(node.WithIdentifier(CSharpSyntax.Identifier(node.Identifier.ValueText + identities[node.Identifier.ValueText])));

            }
            else
            {
                identities[node.Identifier.ValueText] = 1;
                return base.VisitVariableDeclarator(node);
            }
        }
    }
}
