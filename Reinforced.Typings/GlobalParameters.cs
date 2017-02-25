using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings
{
    /// <summary>
    /// Collections of global TS generation parameters
    /// </summary>
    public class GlobalParameters
    {
        private bool _isLocked;
        private bool _writeWarningComment;
        private string _rootNamespace;
        private bool _camelCaseForMethods;
        private bool _camelCaseForProperties;
        private bool _generateDocumentation;
        private bool _exportPureTyings;
        private string _tabSymbol;
        private bool _useModules;
        private bool _discardNamespacesWhenUsingModules;

        public GlobalParameters()
        {
            _tabSymbol = "\t";
            _writeWarningComment = true;
        }

        /// <summary>
        ///     Boolean parameter that controls writing of "auto-generated warning" comment to each generated file.
        /// It meant the comment like "// This code was generated blah blah blah..."
        /// 'true' (default) to write warning comment about auto-generated to every file.
        /// 'false' to do not.
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
        ///     Specifies root namespace for hierarchical export.
        ///     Helps to avoid creating redundant directories when hierarchical export.
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
        ///     Use camelCase for methods naming
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
        ///     Use camelCase for properties naming
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
        ///     Enables or disables documentation generator
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

        internal void Lock()
        {
            _isLocked = true;
        }

        internal void Unlock()
        {
            _isLocked = false;
        }

        /// <summary>
        /// Specifies symbol used for tabulation
        /// </summary>
        public string TabSymbol
        {
            get { return _tabSymbol; }
            set
            {
                if (_isLocked) return;
                _tabSymbol = value;
            }
        }

        /// <summary>
        /// Switches RT to using TS modules system (--module) parameter and import references
        /// </summary>
        public bool UseModules
        {
            get { return _useModules; }
            set
            {
                if (_isLocked) return;
                _useModules = value;
            }
        }

        /// <summary>
        /// When true, RT will ignore classes' namespaces when arraging classes and interfaces among files. 
        /// This parameter only makes difference when using (--module)
        /// </summary>
        public bool DiscardNamespacesWhenUsingModules
        {
            get { return _discardNamespacesWhenUsingModules; }
            set
            {
                if (_isLocked) return;
                _discardNamespacesWhenUsingModules = value;
            }
        }


        /// <summary>
        ///     If true, export will be performed in .d.ts manner (only typings, declare module etc).
        ///     Otherwise, export will be performed to regulat .ts file
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

    }
}
