using System;
using Reinforced.Typings.Ast.Dependency;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    /// Prevents class or interface or enum to be exported.
    /// Instead of that it will be used like type from third-party library.
    /// Use <see cref="RtReference"/> and <see cref="RtImport"/> attributes to specify imports that must be used
    /// when this type appears
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum)]
    public class TsThirdPartyAttribute : Attribute
    {
        /// <inheritdoc />
        public TsThirdPartyAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets full quialified name of third party type to avoid dealing with namespaces, I letters etc
        /// </summary>
        public string Name { get; internal set; }
    }

    /// <summary>
    ///     This attribute is used to add import directive to any file using third-party type (that is marked with <see cref="TsThirdPartyAttribute"/>)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum, AllowMultiple = true)]
    public class TsThirdPartyImportAttribute : Attribute
    {
        /// <summary>
        /// What we are importing from module.
        /// Everything that is placed after "import" keyword and before "from" or "= require(..)"
        /// Examples: 
        /// - "import * as shape from './Shapes'" -> "* as shape" is target <br/>
        /// - "import { Foo } from 'Bar'" -> "{ Foo }" is target <br/>
        /// - "import { Bar2 as bar } from 'Baz'" -> "{ Bar2 as bar }" is target <br/>
        /// If ImportTarget is null then side-effect import will be generated. 
        /// </summary>
        public string ImportTarget { get; set; }

        /// <summary>
        /// Import source is everything that follows after "from" keyword. 
        /// Please note that you do not have to specify quotes here! Quotes will be added automatically
        /// </summary>
        public string ImportSource { get; set; }

        /// <summary>
        /// When true, import will be generated as "import ImportTarget = require('ImportSource')"
        /// </summary>
        public bool ImportRequire { get; set; }

        /// <summary>
        /// Cosntructs new Rtimport
        /// </summary>
        /// <param name="importTarget">Target</param>
        /// <param name="importSource">Source</param>
        /// <param name="importRequire">Is import "=require(...)"</param>
        public TsThirdPartyImportAttribute(string importTarget, string importSource, bool importRequire = false)
        {
            ImportTarget = importTarget;
            ImportSource = importSource;
            ImportRequire = importRequire;
        }

        private RtImport _import;

        internal RtImport ToImport()
        {
            if (_import == null) _import = new RtImport() { Target = ImportTarget, From = ImportSource, IsRequire = ImportRequire };
            return _import;
        }
    }

    /// <summary>
    ///     This attribute is used to add reference directive to file using third-party type (that is marked with <see cref="TsThirdPartyAttribute"/>)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum, AllowMultiple = true)]
    public class TsThirdPartyReferenceAttribute : Attribute
    {
        
        /// <summary>
        ///     Constructs new instance of TsAddTypeReferenceAttribute using referenced type
        /// </summary>
        /// <param name="path">Raw reference</param>
        public TsThirdPartyReferenceAttribute(string path)
        {
            Path = path;
        }
        
        /// <summary>
        ///     Raw reference path that will be added to target file
        /// </summary>
        public string Path { get; set; }

        private RtReference _reference;

        internal RtReference ToReference()
        {
            if (_reference == null) _reference = new RtReference() { Path = Path };
            return _reference;
        }
    }
}
