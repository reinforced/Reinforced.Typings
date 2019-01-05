using Reinforced.Typings.Attributes;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    public static partial class MemberExportExtensions
    {
        /// <summary>
        /// Adds decorator to member
        /// </summary>
        /// <param name="conf">Member configurator</param>
        /// <param name="decorator">Decorator to add (everything that must follow after "@")</param>
        /// <param name="order">Order of appearence</param>
        /// <returns>Fluent</returns>
        public static EnumValueExportBuilder Decorator(this EnumValueExportBuilder conf, string decorator, double order = 0)
        {
            conf.Decorators.Add(new TsDecoratorAttribute(decorator, order));
            return conf;
        }

        /// <summary>
        ///     Overrides name of exported type
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="name">Custom name to be used</param>
        public static EnumValueExportBuilder OverrideName(this EnumValueExportBuilder conf, string name)
        {
            conf.Attr.Name = name;
            return conf;
        }

        /// <summary>
        ///     Ignores specified mambers during exporting
        /// </summary>
        public static EnumValueExportBuilder Ignore(this EnumValueExportBuilder conf, bool ignore = true)
        {
            conf.Ignore = ignore;
            return conf;
        }
    }
}