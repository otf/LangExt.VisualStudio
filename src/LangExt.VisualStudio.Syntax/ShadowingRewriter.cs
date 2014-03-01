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
            var id = node.Identifier.ValueText;

            if (!identities.ContainsKey(id) || identities[id] < 2)
                return base.VisitIdentifierName(node);

            return base.VisitIdentifierName(node.WithIdentifier(CSharpSyntax.Identifier(id + identities[id])));
        }

        public override SyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            var id = node.Identifier.ValueText;

            if (node.Ancestors().Any(anc => anc is FieldDeclarationSyntax))
                return base.VisitVariableDeclarator(node);

            if (!identities.ContainsKey(id))
            {
                identities[id] = 1;
                return base.VisitVariableDeclarator(node);
            }

            ++identities[id];
            return base.VisitVariableDeclarator(node.WithIdentifier(CSharpSyntax.Identifier(id + identities[id])));
        }
    }
}
