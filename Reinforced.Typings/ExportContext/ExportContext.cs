using System;
using System.Collections.Generic;
using System.Reflection;

namespace Reinforced.Typings
{
    /// <summary>
    ///     TsExport exporting settings
    /// </summary>
    public sealed partial class ExportContext : IWarningsCollector
    {

        private bool _isLocked;
        
        /// <summary>
        /// Instantiates new ExportContext instance (only for testing/integration)
        /// </summary>
        public ExportContext(Assembly[] sourceAssemblies, IFilesOperations fileOperationsServiceOverride = null)
        {
            FileOperations = fileOperationsServiceOverride ?? new FilesOperations();
            FileOperations.Context = this;
            Global = new GlobalParameters(sourceAssemblies);
            SourceAssemblies = sourceAssemblies;
            Location = new Location(this);
            Project = new ProjectBlueprint();
        }

        /// <summary>
        ///     There is a case when you are exporting base class as interface. It may lead to some unusual handling of generation,
        ///     so I'm using this property to denote such cases and fix it in-place
        /// </summary>
        internal bool SpecialCase { get; set; }

        internal Dictionary<string, IEnumerable<Type>> TypesToFilesMap { get; private set; }
    }
}