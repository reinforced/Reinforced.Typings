using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    /// Denotes parameter name and constant value for constructor's :base call
    /// We need this attribute because it is programmatically impossible to determine :base call parameters
    /// via reflection. So in this case we need some help from user's side
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor,AllowMultiple = true)]
    public class TsBaseParamAttribute : Attribute
    {
        /// <summary>
        /// Parameters for super() call
        /// Here should be stored TypeScript expressions
        /// </summary>
        public string[] Values { get; set; }

        /// <summary>
        /// Creates instance of TsBaseParamAttribute
        /// </summary>
        /// <param name="values">Set of TypeScript expressions to be supplied for super() call</param>
        public TsBaseParamAttribute(params string[] values)
        {
            Values = values;
        }
    }
}
