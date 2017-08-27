using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Base attribute for typed members/parameters
    /// </summary>
    public abstract class TsTypedAttributeBase : TsAttributeBase
    {
        /// <summary>
        ///     Overrides member type name in resulting TypeScript. 
        ///     Supplied as string. Helpful when property type is not present in your project. 
        ///     E.g. - JQquery object.
        /// </summary>
        public virtual string Type { get; set; }

        /// <summary>
        ///     Similar to `Type`, but you can specify .NET type using typeof. 
        ///     It is useful e.g. for delegates
        /// </summary>
        public virtual Type StrongType { get; set; }

    }
}