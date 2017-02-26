using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Exceptions;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Xmldoc;

namespace Reinforced.Typings
{
    /// <summary>
    ///     TsExport exporting settings
    /// </summary>
    public class ExportContext
    {
        
        private Action<ConfigurationBuilder> _configurationMethod;
        private string _documentationFilePath;
        private bool _hierarchical;
        private bool _isLocked;
       
        private Assembly[] _sourceAssemblies;
        private string _targetDirectory;
        private string _targetFile;
        
        private IFilesOperations _fileOperations;

        public IFilesOperations FileOperations
        {
            get { return _fileOperations; }
            set
            {
                _fileOperations = value;
                _fileOperations.Context = this;
            }
        }

        /// <summary>
        /// Instantiates new ExportContext instance (only for testing/integration)
        /// </summary>
        public ExportContext(IFilesOperations fileOperationsServiceOverride = null)
        {
            Location = new Location();
            Warnings = new List<RtWarning>();
            _fileOperations = fileOperationsServiceOverride;
            if (_fileOperations == null) _fileOperations = new FilesOperations();
            _fileOperations.Context = this;
            Global = new GlobalParameters();
        }

        /// <summary>
        /// Identifies where current export is performed in terms of AST. 
        /// Context.Location could be used to conditionally add members to different places of generated source code
        /// </summary>
        public Location Location { get; private set; }

        /// <summary>
        ///     The assemblies to extract typings from.
        ///     Important! TsExporter do not perform any job for loading assemblies. It is left upon a calling side.
        ///     That is because loading assemblies is highly dependent on calling side's AppDomain.
        ///     TsExporter shouldnt handle all this shit
        /// </summary>
        public Assembly[] SourceAssemblies
        {
            get { return _sourceAssemblies; }
            set
            {
                if (_isLocked) return;
                _sourceAssemblies = value;
            }
        }

        /// <summary>
        ///     True to create project hierarchy in target folder.
        ///     False to store generated typings in single file
        /// </summary>
        public bool Hierarchical
        {
            get { return _hierarchical; }
            set
            {
                if (_isLocked) return;
                _hierarchical = value;
            }
        }

        /// <summary>
        ///     Target directory where to store generated typing files.
        ///     This parameter is not used when Hierarcy is false
        /// </summary>
        public string TargetDirectory
        {
            get { return _targetDirectory; }
            set
            {
                if (_isLocked) return;
                _targetDirectory = value;
            }
        }

        /// <summary>
        ///     Target file where to store generated sources.
        ///     This parameter is not used when Hierarchy is true
        /// </summary>
        public string TargetFile
        {
            get { return _targetFile; }
            set
            {
                if (_isLocked) return;
                _targetFile = value;
            }
        }

       
        /// <summary>
        ///     Path to assembly's XMLDOC file
        /// </summary>
        public string DocumentationFilePath
        {
            get { return _documentationFilePath; }
            set
            {
                if (_isLocked) return;
                _documentationFilePath = value;
            }
        }

        /// <summary>
        ///     Fluent configuration method
        /// </summary>
        public Action<ConfigurationBuilder> ConfigurationMethod
        {
            get { return _configurationMethod; }
            set
            {
                if (_isLocked) return;
                _configurationMethod = value;
            }
        }
        
        /// <summary>
        ///     Documentation manager
        /// </summary>
        public DocumentationManager Documentation { get; internal set; }

        /// <summary>
        /// Warnings that should be displayed after build. 
        /// Feel free to add messages from generators here.
        /// </summary>
        public List<RtWarning> Warnings { get; private set; }

        #region Internals

        /// <summary>
        ///     There is a case when you are exporting base class as interface. It may lead to some unusual handling of generation,
        ///     so I'm using this property to denote such cases and fix it in-place
        /// </summary>
        internal bool SpecialCase { get; set; }

        internal void Lock()
        {
            _isLocked = true;
            Global.Lock();
        }

        internal void Unlock()
        {
            _isLocked = false;
            Global.Unlock();
        }

        internal string CurrentNamespace { get; set; }

        public GlobalParameters Global { get; private set; }

        #endregion



    }
}