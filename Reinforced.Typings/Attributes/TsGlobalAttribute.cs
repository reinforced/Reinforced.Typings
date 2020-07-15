using System;
using Reinforced.Typings.Fluent;


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

        ///// <summary>
        ///// Set to true and all nullable value types will be revealed to "type | null"
        ///// </summary>
        //public bool StrictNullChecks { get; set; }

        /// <summary>
        /// Sets order of applying paramters from this attribute
        /// </summary>
        public double Priority { get; set; }

        /// <summary>
        /// Type of <see cref="Reinforced.Typings.ReferencesInspection.ReferenceProcessorBase"/> to be used to
        /// refilter/reorder references and imports while exporting files
        /// </summary>
        public Type ReferenceProcessorType { get; set; }

        /// <summary>
        ///  Gets or sets whether members reordering (aphabetical, constructors-fields-properties-methods) is enabled
        ///  Warning! Enabling this option discards <see cref="MemberExportExtensions.Order(Reinforced.Typings.Fluent.MethodExportBuilder,double)"/> calls as well as "Order" member attributes property
        /// </summary>
        public bool ReorderMembers { get; set; }

        /// <summary>
        /// Gets or sets whether all nullable properties must be exported as optional
        /// </summary>
        public bool AutoOptionalProperties { get; set; }

        /// <summary>
        /// Gets or sets whether unresolved types must be exported as 'unknown' instead of 'any'
        /// </summary>
        public bool UnresolvedToUnknown { get; set; }

        /// <summary>
        /// Default constructor for TsGlobal attribute
        /// </summary>
        public TsGlobalAttribute()
        {
            WriteWarningComment = true;
            TabSymbol = "\t";
        }

        /// <summary>
        /// Gets or sets type of AST visitor that will be used to write code to output.
        /// Visitor has to be child class of <see cref="Reinforced.Typings.Visitors.TextExportingVisitor"/>
        /// </summary>
        public Type VisitorType { get; set; }

        /// <summary>
        /// Gets or sets whether RT must automatically treat methods returning Task as async methods
        /// </summary>
        public bool AutoAsync { get; set; }
    }
}
