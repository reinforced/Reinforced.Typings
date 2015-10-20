using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Configuration for enum value export configuration
    /// </summary>
    public class EnumValueExportConfiguration : IExportConfiguration<TsValueAttribute>, IIgnorable
    {
        bool IIgnorable.Ignore { get; set; }

        private TsValueAttribute AttributePrototype { get; set; }

        TsValueAttribute IExportConfiguration<TsValueAttribute>.AttributePrototype
        {
            get { return this.AttributePrototype; }
        }

        internal EnumValueExportConfiguration()
        {
            AttributePrototype = new TsValueAttribute();
        }
    }
}