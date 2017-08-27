using System;
using System.Reflection;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Overrides settings for exporting parameters
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class TsParameterAttribute : TsTypedMemberAttributeBase, ISupportsInferring<ParameterInfo>
    {
        private readonly InlineTypeInferers<ParameterInfo> _typeInferers = new InlineTypeInferers<ParameterInfo>();

        /// <summary>
        ///     Specifies default value
        /// </summary>
        public virtual object DefaultValue { get; set; }

        /// <summary>
        /// Type inferers set instance
        /// </summary>
        public InlineTypeInferers<ParameterInfo> TypeInferers
        {
            get { return _typeInferers; }
        }
    }
}