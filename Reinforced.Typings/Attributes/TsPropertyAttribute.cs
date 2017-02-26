using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Overrides property/field export settings
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class TsPropertyAttribute : TsTypedMemberAttributeBase, IOrderableAttribute
    {
        /// <summary>
        ///     Forces property to be a nullable
        ///     E.g. `field:boolean` becomes `field?:boolean` when you specify `[TsProperty(ForceNullable = true)]` in attribute configuration
        /// </summary>
        public virtual bool ForceNullable { get; set; }

        public double Order { get; set; }
    }
}