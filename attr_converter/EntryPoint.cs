using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace attr_converter
{
    [Generator]
    public class SourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // find the main method
            var mainMethod = context.Compilation.GetEntryPoint(context.CancellationToken);

            // build up the source code
            string source = $@"
using System;

namespace {mainMethod.ContainingNamespace.ToDisplayString()}
{{
    public static partial class {mainMethod.ContainingType.Name}
    {{
        static partial void HelloFrom(string name)
        {{
            Console.WriteLine($""Generator says: Hi from '{{name}}'"");
        }}
    }}
}}
        ";
            // add the source code to the compilation
            context.AddSource("generatedSource", source);
        }
    }
}