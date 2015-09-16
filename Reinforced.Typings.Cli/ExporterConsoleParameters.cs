using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Policy;
using System.Text;

namespace Reinforced.Typings.Cli
{
    /// <summary>
    /// Reinforced.Typings generator CLI parameters
    /// </summary>
    public class ExporterConsoleParameters
    {
        /// <summary>
        /// Source assemblies. 
        /// The assemblies to extract typings from
        /// </summary>
        [ConsoleHelp(@"The semicolon-separated assemblies list to extract typings from.
Example:   rtcli.exe SourceAssemblies=""C:\TestProject\Assembly1.dll;C:\TestProject\Assembly2.dll""
",Required.Reuired)]
        public string[] SourceAssemblies { get; set; }

        /// <summary>
        /// Target file where to store generated sources. 
        /// This parameter is not used when Hierarchy is true
        /// </summary>
        [ConsoleHelp(@"Target file where to store generated sources.
Not required if Hirarchy=""true"" specified. Otherwise required.
Example:   rtcli.exe TargetFile=""C:\path\to\target\file.ts""", Required.Partially)]
        public string TargetFile { get; set; }

        /// <summary>
        /// Target directory where to store generated typing files. 
        /// This parameter is not used when Hierarcy is false
        /// </summary>
        [ConsoleHelp(@"Target directory where to store generated typing files. 
Not required if Hirarchy=""false"" or not specified. Otherwise required.
Example:   rtcli.exe TargetDirectory=""C:\path\to\project\"" (regardless ending slash)", Required.Partially)]
        public string TargetDirectory { get; set; }

        /// <summary>
        /// Full list of referenced assemblies. 
        /// If not specified then the CLI will try to resolve reference assemblies from same directory as target assembly
        /// </summary>
        [ConsoleHelp(@"Semicolon-separated full list of referenced assemblies.
If not specified then the CLI will try to resolve reference assemblies from same directory as target assembly. 
Example:   rtcli.exe References=""C:\TestProject\Assembly1.dll;C:\TestProject\Assembly2.dll"" ")]
        public string[] References { get; set; }
        
        /// <summary>
        /// True to create project hierarchy in target folder. 
        /// False to store generated typings in single file
        /// </summary>
        [ConsoleHelp(@"Boolean parameter that switches hierarchy generation ability.
True to create project hierarchy in target folder. 
False (default) to store generated typings in single file. 
Example:   rtcli.exe Hierarchy=""true"" ")]
        public bool Hierarchy { get; set; }

        /// <summary>
        /// True to write warning comment about auto-generated to every file.
        /// False to do not
        /// </summary>
        [ConsoleHelp(@"Boolean parameter that controls writing of auto-generated warning comment to each generated file.
True (default) to write warning comment about auto-generated to every file. 
False to do not. 
Example:   rtcli.exe WriteWarningComment=""false"" ")]
        public bool WriteWarningComment { get; set; }

        /// <summary>
        /// If true, export will be performed in .d.ts manner (only typings, declare module etc).
        /// Otherwise, export will be performed to regulat .ts file
        /// </summary>
        [ConsoleHelp(@"Boolean parameter that controls generation mode switch between .ts/.d.ts
If true, export will be performed in .d.ts manner (only typings, declare module etc).
Otherwise, export will be performed to regulat .ts file 
Example:   rtcli.exe ExportPureTyings=""true"" ")]
        public bool ExportPureTypings { get; set; }

        /// <summary>
        /// Specifies root namespace for hierarchical export.
        /// Helps to avoid creating redundant directories when hierarchical export.
        /// </summary>
        [ConsoleHelp(@"Specifies root namespace for hierarchical export.
Helps to avoid creating redundant directories when hierarchical export.
Example:   rtcli.exe ExportPureTyings=""true"" ")]
        public string RootNamespace { get; set; }

        /// <summary>
        /// Use camelCase for methods naming
        /// </summary>
        [ConsoleHelp(@"Specifies usage of camelCase for methods.
True to use camelCase while generating methods names. False to remain methods names unchanged.
Example:   rtcli.exe CamelCaseForMethods=""true"" ")]
        public bool CamelCaseForMethods { get; set; }

        /// <summary>
        /// Use camelCase for properties naming
        /// </summary>
        [ConsoleHelp(@"Specifies usage of camelCase for properties.
True to use camelCase while generating properties names. False to remain properties names unchanged.
Example:   rtcli.exe CamelCaseForMethods=""true"" ")]
        public bool CamelCaseForProperties { get; set; }

        /// <summary>
        /// Validates input parameters
        /// </summary>
        public void Validate()
        {
            if (!Hierarchy && string.IsNullOrEmpty(TargetFile))
                throw new Exception("Target file must be specified in case of non-hierarchy generation");

            if (Hierarchy&&string.IsNullOrEmpty(TargetDirectory))
                throw new Exception("Target directory must be specified in case of hierarchy generation");

            if (SourceAssemblies==null||SourceAssemblies.Length==0)
                throw new Exception("Source assemblies is not specified. Nothing to export");
        }

        public ExporterConsoleParameters()
        {
            WriteWarningComment = true;
        }

        /// <summary>
        /// Produces command line arguments suitable for rtcli.exe
        /// </summary>
        /// <returns>Command line arguments string</returns>
        public string ExportConsoleParameters()
        {
            List<string> arguments = new List<string>();

            var props = typeof (ExporterConsoleParameters).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            const string propFormat = "{0}=\"{1}\"";
            foreach (var propertyInfo in props)
            {
                var pi = propertyInfo.GetValue(this);
                if (pi==null) continue;
                if (propertyInfo.PropertyType == (typeof (string[])))
                {
                    var strings = (string[]) pi;
                    if (strings.Length==0) continue;
                    pi = string.Join(";", strings);
                }

                arguments.Add(String.Format(propFormat, propertyInfo.Name, pi));
            }

            return String.Join(" ", arguments);
        }
    }
}
