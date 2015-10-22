using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Configuration for enum value export configuration
    /// </summary>
    public class EnumValueExportConfiguration : IExportConfiguration<TsValueAttribute>, IIgnorable
    {
        internal EnumValueExportConfiguration()
        {
            AttributePrototype = new TsValueAttribute();
        }

        internal TsValueAttribute AttributePrototype { get; set; }

        TsValueAttribute IExportConfiguration<TsValueAttribute>.AttributePrototype
        {
            get { return AttributePrototype; }
        }

        bool IIgnorable.Ignore { get; set; }
    }
}