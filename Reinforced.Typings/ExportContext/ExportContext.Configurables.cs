using System;
using System.Collections.Generic;
using System.Linq;
using Reinforced.Typings.Fluent;
// ReSharper disable CheckNamespace
namespace Reinforced.Typings
{
    /// <summary>
    ///     TsExport exporting settings
    /// </summary>
    public partial class ExportContext
    {

        private bool _hierarchical;
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

        private string _targetDirectory;
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
        
        private string _targetFile;
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
        
        private Action<ConfigurationBuilder> _configurationMethod;
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

        private string _documentationFilePath;
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
        
        private HashSet<int> _suppressedWarningCodes = new HashSet<int>();
        /// <summary>
        ///     Gets or sets the list of suppressed warning codes
        /// </summary>
        public IEnumerable<int> SuppressedWarningCodes
        {
            get { return _suppressedWarningCodes; }
            set
            {
                if (_isLocked) return;
                _suppressedWarningCodes = new HashSet<int>(value);
            }
        }

    }
}