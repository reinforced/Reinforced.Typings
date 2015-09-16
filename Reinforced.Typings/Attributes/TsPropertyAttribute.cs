using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    /// Overrides property/field export settings
    /// </summary>
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]
    public class TsPropertyAttribute : TsTypedMemberAttributeBase
    {

        /// <summary>
        /// Forces property to be a nullable
        /// </summary>
        public virtual bool ForceNullable { get; set; }
    }
}
