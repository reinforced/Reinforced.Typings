﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Exceptions;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Generators;

namespace Reinforced.Typings
{
    /// <summary>
    ///     TsExport exporting settings
    /// </summary>
    public sealed partial class ExportContext
    {

        private bool _isLocked;
        
        /// <summary>
        /// Instantiates new ExportContext instance (only for testing/integration)
        /// </summary>
        public ExportContext(Assembly[] sourceAssemblies, IFilesOperations fileOperationsServiceOverride = null)
        {
            Warnings = new List<RtWarning>();
            FileOperations = fileOperationsServiceOverride ?? new FilesOperations();
            FileOperations.Context = this;
            Global = new GlobalParameters(sourceAssemblies);
            SourceAssemblies = sourceAssemblies;
            Location = new Location(this);
            Project = new ProjectBlueprint();
            CustomBuilders = new List<CustomExportBuilder>();
            CustomGeneratorsToFilesMap = new Dictionary<string, IEnumerable<ICustomCodeGenerator>>();
        }

        /// <summary>
        ///     There is a case when you are exporting base class as interface. It may lead to some unusual handling of generation,
        ///     so I'm using this property to denote such cases and fix it in-place
        /// </summary>
        internal bool SpecialCase { get; set; }

        internal Dictionary<string, IEnumerable<Type>> TypesToFilesMap { get; private set; }
        internal Dictionary<string, IEnumerable<ICustomCodeGenerator>> CustomGeneratorsToFilesMap { get; private set; }
    }
}