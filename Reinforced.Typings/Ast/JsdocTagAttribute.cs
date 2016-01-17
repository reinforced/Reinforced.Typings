using System;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// JSDOC tag attribute to convert enum name back to JSDOC tag
    /// </summary>
    internal class JsdocTagAttribute : Attribute
    {
        /// <summary>
        /// Raw tag name
        /// </summary>
        public string RawTagName { get; set; }

        public JsdocTagAttribute(string rawTagName)
        {
            RawTagName = rawTagName;
        }
    }
}
