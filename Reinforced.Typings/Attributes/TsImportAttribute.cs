using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Ast.Dependency;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Specifies path of reference which required to be added to result .ts file
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class TsImportAttribute : Attribute
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
        /// Please not the you do not have to specify quotes here! Quotes will be added automatically
        /// </summary>
        public string ImportSource { get; set; }

        /// <summary>
        /// When true, import will be generated as "import ImportTarget = require('ImportSource')"
        /// </summary>
        public bool ImportRequire { get; set; }

        private RtImport _import;

        internal RtImport ToImport()
        {
            if (_import==null) _import = new RtImport() {Target = ImportTarget, From = ImportSource, IsRequire = ImportRequire};
            return _import;
        }
    }
}
