using System;
using System.Reflection;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Overrides function export
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TsFunctionAttribute : TsTypedMemberAttributeBase, ISupportsInferring<MethodInfo>
    {
        private readonly InlineTypeInferers<MethodInfo> _typeInferers = new InlineTypeInferers<MethodInfo>();

        /// <summary>
        /// Sets order this membter will be written to output file in
        /// </summary>
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
            // ReSharper disable once ValueParameterNotUsed
            private set { }
        }
    }
}