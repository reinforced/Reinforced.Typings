using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    /// Sets global parameters for RT export.
    /// Warning! Use Priority property to control [TsGlobal] processing order. 
    /// When exporting multiple assemblies and several ones will contain [TsGlobal] then the one with 
    /// highest priority will be used. Global parameters configured from fluent configuration 
    /// using builder.Global method always has highest priority
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class TsGlobalAttribute : Attribute
    {
        /// <summary>
        ///     Boolean parameter that controls writing of "auto-generated warning" comment to each generated file.
        /// It meant the comment like "// This code was generated blah blah blah..."
        /// 'true' (default) to write warning comment about auto-generated to every file.
        /// 'false' to do not.
        /// </summary>
        public bool WriteWarningComment { get; set; }

        /// <summary>
        ///     Specifies root namespace for hierarchical export.
        ///     Helps to avoid creating redundant directories when hierarchical export.
        /// </summary>
        public string RootNamespace { get; set; }

        /// <summary>
        ///     Use camelCase for methods naming
        /// </summary>
        public bool CamelCaseForMethods { get; set; }

        /// <summary>
        ///     Use camelCase for properties naming
        /// </summary>
        public bool CamelCaseForProperties { get; set; }

        /// <summary>
        ///     Enables or disables documentation generator
        /// </summary>
        public bool GenerateDocumentation { get; set; }

        /// <summary>
        /// Specifies symbol used for tabulation
        /// </summary>
        public string TabSymbol { get; set; }

        /// <summary>
        /// Switches RT to using TS modules system (--module tsc.exe parameter) and import references
        /// </summary>
        public bool UseModules { get; set; }

        /// <summary>
        /// When true, RT will ignore classes' namespaces when arraging classes and interfaces among files. 
        /// This parameter only makes difference when using (--module)
        /// </summary>
        public bool DiscardNamespacesWhenUsingModules { get; set; }

        /// <summary>
        ///     If true, export will be performed in .d.ts manner (only typings, declare module etc).
        ///     Otherwise, export will be performed to regulat .ts file
        /// </summary>
        public bool ExportPureTypings { get; set; }

        /// <summary>
        /// Sets order of applying paramters from this attribute
        /// </summary>
        public double Priority { get; set; }

    }
}
