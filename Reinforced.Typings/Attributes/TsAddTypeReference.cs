using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Attributes
{

    /// <summary>
    /// This attribute is used to add reference directive to file containing single TS class typing.
    /// It is only used while splitting generated type sto different files
    /// </summary>
    public class TsAddTypeReferenceAttribute : Attribute
    {
        /// <summary>
        /// Type that should be referenced
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Raw reference path that will be added to target file
        /// </summary>
        public string RawPath { get; set; }

        /// <summary>
        /// Constructs new instance of TsAddTypeReferenceAttribute using referenced type
        /// </summary>
        /// <param name="type">Type reference</param>
        public TsAddTypeReferenceAttribute(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Constructs new instance of TsAddTypeReferenceAttribute using referenced type
        /// </summary>
        /// <param name="rawPath">Raw reference</param>
        public TsAddTypeReferenceAttribute(string rawPath)
        {
            RawPath = rawPath;
        }
    }
}
