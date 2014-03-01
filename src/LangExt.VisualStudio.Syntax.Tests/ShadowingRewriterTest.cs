using NUnit.Framework;
using Roslyn.Compilers.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LangExt.VisualStudio.Syntax.Tests
{
    [TestFixture]
    public class ShadowingRewriterTest
    {
        void AssertRewrite<TNode>(string snippet, string expectedRewrited)
        {
            var source = string.Format(@"class Program {{ {0} }}", snippet);
            var syntaxTree = SyntaxTree.ParseText(source);
            var rootNode = syntaxTree.GetRoot();
            var rewrited = new ShadowingRewriter().Visit(rootNode);

            Assert.That(rewrited.ChildNodes().Single().ChildNodes().Single(node => node is TNode).ToString(), Is.EqualTo(expectedRewrited));
        }

        [TestCase(@"int x=0; char x='a';", @"int x=0; char x2='a';")]
        public void RewriteMethod(string snippet, string expected)
        {
            var methodDeclFormat = "void f(){{ {0} }}";
            var fullSnippet = string.Format(methodDeclFormat, snippet);
            var fullExpected = string.Format(methodDeclFormat, expected);
            AssertRewrite<MethodDeclarationSyntax>(fullSnippet, fullExpected);
        }

        [TestCase(@"int x=0; char x='a';", @"int x=0; char x2='a';")]
        public void RewriteProperty(string snippet, string expected)
        {
            var methodDeclFormat = "int Prop {{  get{{ {0} return 0; }} }}";
            var fullSnippet = string.Format(methodDeclFormat, snippet);
            var fullExpected = string.Format(methodDeclFormat, expected);
            AssertRewrite<PropertyDeclarationSyntax>(fullSnippet, fullExpected);
        }

        [TestCase(@"int x=0;", @"int x=0;")]
        [TestCase(@"int x=0; x=100;", @"int x=0; x=100;")]
        public void ItCantRewriteProperty(string snippet, string expected)
        {
            var methodDeclFormat = "void f(){{ {0} }}";
            var fullSnippet = string.Format(methodDeclFormat, snippet);
            var fullExpected = string.Format(methodDeclFormat, expected);
            AssertRewrite<MethodDeclarationSyntax>(fullSnippet, fullExpected);
        }
    }
}
