using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Parameter configuration builder
    /// </summary>
    public class ParameterConfigurationBuilder : IExportConfiguration<TsParameterAttribute>, IIgnorable
    {
        internal ParameterConfigurationBuilder()
        {
            AttributePrototype = new TsParameterAttribute();
        }

        private TsParameterAttribute AttributePrototype { get; set; }

        TsParameterAttribute IExportConfiguration<TsParameterAttribute>.AttributePrototype
        {
            get { return AttributePrototype; }
        }

        bool IIgnorable.Ignore { get; set; }
    }
}