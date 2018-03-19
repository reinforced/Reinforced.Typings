using System;
using System.Collections.Generic;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Configuration builder for Enum type
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public class EnumConfigurationBuilder<TEnum> : IEnumConfigurationBuidler
        where TEnum : struct
    {
        internal readonly TypeBlueprint _blueprint;
        internal EnumConfigurationBuilder(ExportContext context)
        {
            _blueprint = context.Project.Blueprint(typeof(TEnum));
            Context = context;
            if (_blueprint.TypeAttribute == null) _blueprint.TypeAttribute = new TsEnumAttribute();
        }

        TsEnumAttribute IAttributed<TsEnumAttribute>.AttributePrototype
        {
            get { return _blueprint.Attr<TsEnumAttribute>(); }
        }

        Type IEnumConfigurationBuidler.EnumType
        {
            get { return typeof(TEnum); }
        }

        /// <summary>
        /// Export context
        /// </summary>
        public ExportContext Context { get; }

        ICollection<TsAddTypeReferenceAttribute> IReferenceConfigurationBuilder.References
        {
            get { return _blueprint.References; }
        }

        ICollection<TsAddTypeImportAttribute> IReferenceConfigurationBuilder.Imports
        {
            get { return _blueprint.Imports; }
        }

        string IReferenceConfigurationBuilder.PathToFile
        {
            get { return _blueprint.PathToFile; }
            set { _blueprint.PathToFile = value; }
        }

        List<TsDecoratorAttribute> IDecoratorsAggregator.Decorators
        {
            get { return _blueprint.Decorators; }
        }
    }
}