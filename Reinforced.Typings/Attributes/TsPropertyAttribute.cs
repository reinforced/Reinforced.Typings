using System;
using System.Reflection;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Overrides property/field export settings
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class TsPropertyAttribute : TsTypedMemberAttributeBase, IOrderableAttribute,
        ISupportsInferring<MemberInfo>

    {
        private readonly InlineTypeInferers<MemberInfo> _typeInferers = new InlineTypeInferers<MemberInfo>();

        /// <summary>
        ///     Forces property to be a nullable
        ///     E.g. `field:boolean` becomes `field?:boolean` when you specify `[TsProperty(ForceNullable = true)]` in attribute configuration
        /// </summary>
        internal virtual bool? NilForceNullable { get; set; }

        /// <summary>
        ///     Forces property to be a nullable
        ///     E.g. `field:boolean` becomes `field?:boolean` when you specify `[TsProperty(ForceNullable = true)]` in attribute configuration
        /// </summary>
        public virtual bool ForceNullable { get { return NilForceNullable ?? false; } set { NilForceNullable = value; } }

        /// <inheritdoc />
        public double Order { get; set; }


        /// <summary>
        /// Type inferers set instance
        /// </summary>
        public InlineTypeInferers<MemberInfo> TypeInferers
        {
            get { return _typeInferers; }
            private set { }
        }
    }
}