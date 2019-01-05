using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Attributes;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Fluent export configuration builder for enum value
    /// </summary>
    public class EnumValueExportBuilder 
    {
        internal TypeBlueprint _containingTypeBlueprint;
        internal FieldInfo _member;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        internal EnumValueExportBuilder(TypeBlueprint containingTypeBlueprint, FieldInfo member)
        {
            _containingTypeBlueprint = containingTypeBlueprint;
            _member = member;
        }

        internal bool Ignore
        {
            get { return _containingTypeBlueprint.IsIgnored(_member); }
            set
            {
                if (value) _containingTypeBlueprint.Ignored.Add(_member);
                else if (_containingTypeBlueprint.Ignored.Contains(_member)) _containingTypeBlueprint.Ignored.Remove(_member);
            }
        }

        internal List<TsDecoratorAttribute> Decorators
        {
            get { return _containingTypeBlueprint.DecoratorsListFor(_member); }
        }

        internal TsValueAttribute Attr
        {
            get { return _containingTypeBlueprint.ForEnumValue(_member); }
        }
    }
}