using System.Reflection;
using Reinforced.Typings.Xmldoc;

namespace Reinforced.Typings
{
    /// <summary>
    /// TsExport exporting settings
    /// </summary>
    public class ExportSettings
    {
        private bool _isLocked;
        private string _targetFile;
        private string _targetDirectory;
        private bool _writeWarningComment;
        private bool _hierarchical;
        private Assembly[] _sourceAssemblies;
        private bool _exportPureTyings;
        private string _rootNamespace;
        private bool _camelCaseForMethods;
        private bool _camelCaseForProperties;
        private string _documentationFilePath;
        private bool _generateDocumentation;

        /// <summary>
        /// The assemblies to extract typings from. 
        /// Important! TsExporter do not perform any job for loading assemblies. It is left upon a calling side. 
        /// That is because loading assemblies is highly dependent on calling side's AppDomain. 
        /// TsExporter shouldnt handle all this shit
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
        /// True to create project hierarchy in target folder. 
        /// False to store generated typings in single file
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
        /// True to write warning comment about auto-generated to every file.
        /// False to do not
        /// </summary>
        public bool WriteWarningComment
        {
            get { return _writeWarningComment; }
            set
            {
                if (_isLocked) return;
                _writeWarningComment = value;
            }
        }

        /// <summary>
        /// Target directory where to store generated typing files. 
        /// This parameter is not used when Hierarcy is false
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
        /// Target file where to store generated sources. 
        /// This parameter is not used when Hierarchy is true
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
        /// If true, export will be performed in .d.ts manner (only typings, declare module etc).
        /// Otherwise, export will be performed to regulat .ts file
        /// </summary>
        public bool ExportPureTypings
        {
            get { return _exportPureTyings; }
            set
            {
                if (_isLocked) return;
                _exportPureTyings = value;
            }
        }

        /// <summary>
        /// Specifies root namespace for hierarchical export.
        /// Helps to avoid creating redundant directories when hierarchical export.
        /// </summary>
        public string RootNamespace
        {
            get { return _rootNamespace; }
            set
            {
                if (_isLocked) return;
                _rootNamespace = value;
            }
        }

        /// <summary>
        /// Use camelCase for methods naming
        /// </summary>
        public bool CamelCaseForMethods
        {
            get { return _camelCaseForMethods; }
            set
            {
                if (_isLocked) return;
                _camelCaseForMethods = value;
            }
        }

        /// <summary>
        /// Use camelCase for properties naming
        /// </summary>
        public bool CamelCaseForProperties
        {
            get { return _camelCaseForProperties; }
            set
            {
                if (_isLocked) return;
                _camelCaseForProperties = value;
            }
        }

        /// <summary>
        /// Path to assembly's XMLDOC file
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
        /// Enables or disables documentation generator
        /// </summary>
        public bool GenerateDocumentation
        {
            get { return _generateDocumentation; }
            set
            {
                if (_isLocked) return;
                _generateDocumentation = value;
            }
        }

        internal string References { get; set; }


        internal void Lock()
        {
            _isLocked = true;
        }


        internal void Unlock()
        {
            _isLocked = false;
        }

        internal DocumentationManager Documentation { get; set; }
    }
}
