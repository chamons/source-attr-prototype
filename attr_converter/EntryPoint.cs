using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace attr_converter
{
    // 1. Generate new .NET style nodes for existing legacy style ones
    // 2. Imply Catalyst for iOS
    // 3. Generate child nodes based on parent elements
    // 4. Remove existing attributes nodes (I don't think we can modify existing code?)

    // Downsides
    // Must declare everything we're extending as partial
    [Generator]
    public class SourceGenerator : ISourceGenerator
    {
        StringBuilder Generated = new ();

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications (() => new SyntaxReceiver());
        }

#if DEBUG
        public void Print (string text)
        {
            // stdout is ignored and stderr will fail builds, so scribble to our output
            Generated.Append ($"class Print___{text}___{Guid.NewGuid().ToString().Replace('-', '_')} {{}}");
        }
#endif

        public void Execute(GeneratorExecutionContext context)
        {
            var receiver = (SyntaxReceiver)context.SyntaxReceiver;
    
            foreach (var klass in receiver.ClassCandidates) {                
                if (klass.AttributeLists.SelectMany(a => a.Attributes).Any(a => IsAvailabilityAttribute(a.Name.ToString()))) {    
                    Generated.Append ($"[ObjCRuntime.Introduced (ObjCRuntime.PlatformName.iOS, 20, 0)]partial class {klass.Identifier} {{}}");
                }
            }
            context.AddSource($"attributes.g.cs", SourceText.From(Generated.ToString(), Encoding.UTF8));
        }

        bool IsAvailabilityAttribute (string name) 
        {
            switch (name) {
                case "Introduced":
                case "NoMac":
                case "NoiOS":
                // TODO - Add rest
                    return true;
                default:
                    return false;
            }
        }
    }

    class SyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> ClassCandidates { get; } = new ();
        public List<MemberDeclarationSyntax> MemberCandidates { get; } = new ();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is MemberDeclarationSyntax memberDeclarationSyntax && memberDeclarationSyntax.AttributeLists.Count > 0) {
                if (memberDeclarationSyntax.Parent is TypeDeclarationSyntax) {
                    if (memberDeclarationSyntax is MethodDeclarationSyntax ||  memberDeclarationSyntax is PropertyDeclarationSyntax) {
                        MemberCandidates.Add (memberDeclarationSyntax);
                    }
                }
            }
            if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax  && classDeclarationSyntax.AttributeLists.Count > 0) {
                ClassCandidates.Add (classDeclarationSyntax);
            }
        }
    }
}