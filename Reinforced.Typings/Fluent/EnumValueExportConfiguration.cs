using System.Reflection;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Configuration for enum value export configuration
    /// </summary>
    public class EnumValueExportConfiguration : IMemberExportConfiguration<TsValueAttribute, FieldInfo>, IIgnorable
    {
        private readonly TypeBlueprint _blueprint;
        internal EnumValueExportConfiguration(FieldInfo member, TypeBlueprint blueprint)
        {
            Member = member;
            _blueprint = blueprint;
            _blueprint.ForEnumValue(Member, true);
        }

        TsValueAttribute IAttributed<TsValueAttribute>.AttributePrototype
        {
            get { return _blueprint.ForEnumValue(Member, true); }
        }

        bool IIgnorable.Ignore
        {
            get { return _blueprint.IsIgnored(Member); }
            set
            {
                if (value) _blueprint.Ignored.Add(Member);
                else if (_blueprint.Ignored.Contains(Member)) _blueprint.Ignored.Remove(Member);
            }
        }

        /// <summary>
        /// Exporting member
        /// </summary>
        public FieldInfo Member { get; private set; }
    }
}