using System;
using System.Reflection;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Overrides function export
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TsFunctionAttribute : TsTypedMemberAttributeBase, IOrderableAttribute, ISupportsInferring<MethodInfo>
    {
        private readonly InlineTypeInferers<MethodInfo> _typeInferers = new InlineTypeInferers<MethodInfo>();

        /// <inheritdoc />
        public double Order { get; set; }

        /// <summary>
        /// Inline function code to be converted to RtRaw and used as function body
        /// </summary>
        public string Implementation { get; set; }

        /// <summary>
        /// Type inferers set instance
        /// </summary>
        public InlineTypeInferers<MethodInfo> TypeInferers
        {
            get { return _typeInferers; }
            private set { }
        }
    }
}