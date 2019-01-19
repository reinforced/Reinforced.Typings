using System;
using System.Linq;
using System.Reflection;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Exceptions;
using Reinforced.Typings.ReferencesInspection;

namespace Reinforced.Typings
{
    /// <summary>
    /// Collections of global TS generation parameters
    /// </summary>
    public class GlobalParameters
    {
        private bool _isLocked;
        
        private readonly TsGlobalAttribute _attr;

        internal GlobalParameters(Assembly[] sourceAssemblies)
        {
            var tsGlobal = sourceAssemblies.Select(c => c.GetCustomAttribute<TsGlobalAttribute>())
                .Where(c => c != null)
                .OrderByDescending(c => c.Priority)
                .FirstOrDefault();
            _attr = tsGlobal ?? new TsGlobalAttribute();
        }

        /// <summary>
        ///     Boolean parameter that controls writing of "auto-generated warning" comment to each generated file.
        /// It meant the comment like "// This code was generated blah blah blah..."
        /// 'true' (default) to write warning comment about auto-generated to every file.
        /// 'false' to do not.
        /// </summary>
        public bool WriteWarningComment
        {
            get { return _attr == null ? true : _attr.WriteWarningComment; }
            set
            {
                if (_isLocked) return;
                _attr.WriteWarningComment = value;
            }
        }

        /// <summary>
        ///     Specifies root namespace for hierarchical export.
        ///     Helps to avoid creating redundant directories when hierarchical export.
        /// </summary>
        public string RootNamespace
        {
            get { return _attr.RootNamespace; }
            set
            {
                if (_isLocked) return;
                _attr.RootNamespace = value;
            }
        }

        /// <summary>
        ///     Use camelCase for methods naming
        /// </summary>
        public bool CamelCaseForMethods
        {
            get { return _attr.CamelCaseForMethods; }
            set
            {
                if (_isLocked) return;
                _attr.CamelCaseForMethods = value;
            }
        }

        /// <summary>
        ///     Use camelCase for properties naming
        /// </summary>
        public bool CamelCaseForProperties
        {
            get { return _attr.CamelCaseForProperties; }
            set
            {
                if (_isLocked) return;
                _attr.CamelCaseForProperties = value;
            }
        }

        /// <summary>
        ///     Enables or disables documentation generator
        /// </summary>
        public bool GenerateDocumentation
        {
            get { return _attr.GenerateDocumentation; }
            set
            {
                if (_isLocked) return;
                _attr.GenerateDocumentation = value;
            }
        }

        /// <summary>
        ///    Gets or sets whether all nullable properties must be exported as optional
        /// </summary>
        public bool AutoOptionalProperties
        {
            get { return _attr.AutoOptionalProperties; }
            set
            {
                if (_isLocked) return;
                _attr.AutoOptionalProperties = value;
            }
        }

        ///// <summary>
        /////     Enables or disables documentation generator
        ///// </summary>
        //public bool StrictNullChecks
        //{
        //    get { return _strictNullChecks; }
        //    set
        //    {
        //        if (_isLocked) return;
        //        _strictNullChecks = value;
        //    }
        //}

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
            get { return _attr.TabSymbol; }
            set
            {
                if (_isLocked) return;
                _attr.TabSymbol = value;
            }
        }

        /// <summary>
        /// Specifies string used as the line terminator.
        /// </summary>
        public string NewLine {
            get { return _attr.NewLine; }
            set {
                if(_isLocked) return;
                _attr.NewLine = value;
            }
        }

        /// <summary>
        /// Switches RT to using TS modules system (--module) parameter and import references
        /// </summary>
        public bool UseModules
        {
            get { return _attr.UseModules; }
            set
            {
                if (_isLocked) return;
                _attr.UseModules = value;
            }
        }

        /// <summary>
        /// When true, RT will ignore classes' namespaces when arraging classes and interfaces among files. 
        /// This parameter only makes difference when using (--module)
        /// </summary>
        public bool DiscardNamespacesWhenUsingModules
        {
            get { return _attr.DiscardNamespacesWhenUsingModules; }
            set
            {
                if (_isLocked) return;
                _attr.DiscardNamespacesWhenUsingModules = value;
            }
        }


        /// <summary>
        ///     If true, export will be performed in .d.ts manner (only typings, declare module etc).
        ///     Otherwise, export will be performed to regulat .ts file
        /// </summary>
        public bool ExportPureTypings
        {
            get { return _attr.ExportPureTypings; }
            set
            {
                if (_isLocked) return;
                _attr.ExportPureTypings = value;
            }
        }

        /// <summary>
        /// Gets or sets type of <see cref="Reinforced.Typings.ReferencesInspection.ReferenceProcessorBase"/> to be used while exporting files
        /// </summary>
        public Type ReferencesProcessorType
        {
            get { return _attr.ReferenceProcessorType; }
            set
            {
                if (_isLocked) return;
                if (!typeof(ReferenceProcessorBase)._IsAssignableFrom(value))
                {
                    ErrorMessages.RTE0016_InvalidRefProcessorType.Throw(value);
                }
                _attr.ReferenceProcessorType = value;
            }
        }

        /// <summary>
        ///     Gets or sets whether it is needed to sort exported members alphabetically
        /// </summary>
        public bool ReorderMembers
        {
            get { return _attr.ReorderMembers; }
            set
            {
                if (_isLocked) return;
                _attr.ReorderMembers = value;
            }
        }
    }
}
