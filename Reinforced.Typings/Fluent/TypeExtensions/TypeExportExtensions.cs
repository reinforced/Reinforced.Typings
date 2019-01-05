using System;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Exceptions;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    public static partial class TypeExportExtensions
    {
        /// <summary>
        /// Sets order this membter will be written to output file in
        /// </summary>
        /// <param name="conf">Configurator</param>
        /// <param name="order">Order of member</param>
        /// <returns>Fluent</returns>
        public static TypeExportBuilder Order(this TypeExportBuilder conf, double order)
        {
            conf.Blueprint.TypeAttribute.Order = order;
            return conf;
        }

        /// <summary>
        ///     Configures exporter to flatten inheritance hierarchy for supplied type
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="until">
        /// All classes "deeper" than specified (including) will not be considered as exportable members donors. 
        /// By default this parameter is equal to typeof(object)
        /// </param>
        public static T FlattenHierarchy<T>(this T conf, Type until = null) where T: TypeExportBuilder
        {
            if (!conf.Blueprint.TypeAttribute.FlattenHierarchy)
            {
                if (!conf.Blueprint.CanFlatten())
                {
                    ErrorMessages.RTE0015_CannotFlatten.Throw(conf.Blueprint.Type.FullName);
                }

                conf.Blueprint.NotifyFlattenTouched();
            }

            conf.Blueprint.TypeAttribute.FlattenHierarchy = true;
            if (until != null)
            {
                conf.Blueprint.TypeAttribute.FlattenLimiter = until;
            }
            return conf;
        }

        /// <summary>
        /// Adds decorator to member
        /// </summary>
        /// <param name="conf">Member configurator</param>
        /// <param name="decorator">Decorator to add (everything that must follow after "@")</param>
        /// <param name="order">Order of appearence</param>
        /// <returns>Fluent</returns>
        public static TypeExportBuilder Decorator(this TypeExportBuilder conf, string decorator, double order = 0)
        {
            conf.Blueprint.Decorators.Add(new TsDecoratorAttribute(decorator, order));
            return conf;
        }
    }
}