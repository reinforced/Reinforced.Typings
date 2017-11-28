using System.Reflection;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Configuration for enum value export configuration
    /// </summary>
    public class EnumValueExportConfiguration : IMemberExportConfiguration<TsValueAttribute,FieldInfo>, IIgnorable
    {
        internal EnumValueExportConfiguration(FieldInfo member)
        {
            Member = member;
            AttributePrototype = member.RetrieveOrCreateCustomAttribute<TsValueAttribute>();
        }

        internal TsValueAttribute AttributePrototype { get; set; }

        TsValueAttribute IAttributed<TsValueAttribute>.AttributePrototype
        {
            get { return AttributePrototype; }
        }

        bool IIgnorable.Ignore { get; set; }

        /// <summary>
        /// Exporting member
        /// </summary>
        public FieldInfo Member { get; private set; }
    }
}