using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Parameter configuration builder
    /// </summary>
    public class ParameterConfigurationBuilder : IExportConfiguration<TsParameterAttribute>, IIgnorable
    {
        bool IIgnorable.Ignore { get; set; }
        private TsParameterAttribute AttributePrototype { get; set; }
        TsParameterAttribute IExportConfiguration<TsParameterAttribute>.AttributePrototype
        {
            get { return this.AttributePrototype; }
        }

        internal ParameterConfigurationBuilder()
        {
           AttributePrototype = new TsParameterAttribute();
        }
    }
}
