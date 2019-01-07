using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Exceptions;
using Reinforced.Typings.ReferencesInspection;
using Reinforced.Typings.Xmldoc;
// ReSharper disable CheckNamespace
namespace Reinforced.Typings
{
    /// <summary>
    ///     TsExport exporting settings
    /// </summary>
    public partial class ExportContext
    {
        /// <summary>
        /// File I/O operations frontend
        /// </summary>
        public IFilesOperations FileOperations { get; private set; }

        /// <summary>
        /// Identifies where current export is performed in terms of AST. 
        /// Context.Location could be used to conditionally add members to different places of generated source code
        /// </summary>
        public Location Location { get; private set; }

        /// <summary>
        ///     Gets the assemblies to extract typings from.
        ///     Important! TsExporter do not perform any job for loading assemblies. It is left upon a calling side.
        ///     That is because loading assemblies is highly dependent on calling side's AppDomain.
        ///     TsExporter shouldnt handle all this shit
        /// </summary>
        public Assembly[] SourceAssemblies { get; private set; }

        /// <summary>
        ///     Documentation manager
        /// </summary>
        public DocumentationManager Documentation { get; private set; }

        /// <summary>
        /// Warnings that should be displayed after build. 
        /// Feel free to add messages from generators here.
        /// </summary>
        public List<RtWarning> Warnings { get; private set; }

        /// <summary>
        /// Blueprint of type currently being exported
        /// </summary>
        public TypeBlueprint CurrentBlueprint
        {
            get
            {
                if (Location._typesStack.Count == 0) return null;
                return Location._typesStack.Peek();
            }
        }

        /// <summary>
        /// Global generation parameters
        /// </summary>
        public GlobalParameters Global { get; private set; }

        /// <summary>
        /// Generators cache
        /// </summary>
        public GeneratorManager Generators { get; private set; }

        /// <summary>
        /// Project blueprint
        /// </summary>
        public ProjectBlueprint Project { get; private set; }

       

    }
}