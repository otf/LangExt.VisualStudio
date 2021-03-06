﻿using NUnit.Framework;
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
            var selectedNode = rewrited.ChildNodes().Single().ChildNodes().Single(node => node is TNode);
            Assert.That(selectedNode.ToString(), Is.EqualTo(expectedRewrited));
        }

        void AssertRewriteMethod(string snippet, string expected)
        {
            var methodDeclFormat = "void f(){{ {0} }}";
            var fullSnippet = string.Format(methodDeclFormat, snippet);
            var fullExpected = string.Format(methodDeclFormat, expected);
            AssertRewrite<MethodDeclarationSyntax>(fullSnippet, fullExpected);
        }

        void AssertRewriteProperty(string snippet, string expected)
        {
            var methodDeclFormat = "int Prop {{  get{{ {0} return 0; }} }}";
            var fullSnippet = string.Format(methodDeclFormat, snippet);
            var fullExpected = string.Format(methodDeclFormat, expected);
            AssertRewrite<PropertyDeclarationSyntax>(fullSnippet, fullExpected);
        }

        void AssertRewriteEvent(string snippet, string expected)
        {
            var methodDeclFormat = "event EventHandler Event1{{ add {{ {0} }} }}";
            var fullSnippet = string.Format(methodDeclFormat, snippet);
            var fullExpected = string.Format(methodDeclFormat, expected);
            AssertRewrite<EventDeclarationSyntax>(fullSnippet, fullExpected);
        }

        [TestCase(@"int x=0; char x='a';", @"int x=0; char x2='a';")]
        [TestCase(@"int x=0; for(char x='a'; ; ) { long x=3; }", @"int x=0; for(char x2='a'; ; ) { long x3=3; }")]
        [TestCase(@"int x=0; for(char x='a'; ; );", @"int x=0; for(char x2='a'; ; );")]
        public void ItShouldRewriteMethod(string snippet, string expected)
        {
            AssertRewriteMethod(snippet, expected);
        }


        [TestCase(@"int x=0; char x='a';", @"int x=0; char x2='a';")]
        public void ItShouldRewriteProperty(string snippet, string expected)
        {
            AssertRewriteProperty(snippet, expected);
        }

        [TestCase(@"int x=0; char x='a';", @"int x=0; char x2='a';")]
        public void ItShouldRewriteEvent(string snippet, string expected)
        {
            AssertRewriteEvent(snippet, expected);
        }

        [TestCase(@"int x=0;", @"int x=0;")]
        [TestCase(@"int x=0; x=100;", @"int x=0; x=100;")]
        public void ItShouldNotRewriteProperty(string snippet, string expected)
        {
            AssertRewriteMethod(snippet, expected);
        }
    }
}
