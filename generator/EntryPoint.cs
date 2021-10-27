using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace generator
{
    partial class Program
    {
        static void Main(string[] args)
        {
            string BindingPath = Path.Combine (AppContext.BaseDirectory, "../../../../binding/bin/Debug/net5.0/binding.dll");
            
            string[] runtimeAssemblies = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll");
            var paths = new List<string>(runtimeAssemblies);
            paths.Add (BindingPath);
            
            var resolver = new PathAssemblyResolver(paths);

            using (var mlc = new MetadataLoadContext(resolver))
            {
                Assembly assembly = mlc.LoadFromAssemblyPath(BindingPath);
                foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.Name.EndsWith("Attribute"))) {
                    PrintAttributes (type);
                    Console.WriteLine ($"{type.Name}:");
                    foreach (var member in type.GetMembers()) {                        
                        PrintAttributes (member, tab: true);
                        Console.WriteLine ($"\t{member.Name}");
                        Console.WriteLine();
                    }
                }
            }
        }

        static void PrintAttributes (MemberInfo member, bool tab = false)
        {
            foreach (var attr in member.GetCustomAttributesData()) {
                switch (attr.AttributeType.Name) {
                    case "IntroducedAttribute":
                    case "NoMacAttribute":
                    case "NoiOSAttribute":
                        string name = attr.AttributeType.Name.Substring(0, attr.AttributeType.Name.Length - 9 /* Attribute */);
                        if (attr.ConstructorArguments.Count > 1) {
                            Console.WriteLine ($"{(tab ? "\t" : "")}[{name} ({attr.ConstructorArguments[1].Value}, {attr.ConstructorArguments[2].Value})]");
                        }
                        else {
                            Console.WriteLine ($"{(tab ? "\t" : "")}[{name}]");
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
