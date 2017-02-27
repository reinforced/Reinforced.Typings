using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     This attribute is used to add import directive to file containing single TS class typing.
    ///     It is only used while splitting generated type sto different files
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum, AllowMultiple = true)]
    public class TsAddTypeImportAttribute : Attribute
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
        public TsAddTypeImportAttribute(string importTarget, string importSource, bool importRequire = false)
        {
            ImportTarget = importTarget;
            ImportSource = importSource;
            ImportRequire = importRequire;
        }
    }
}
