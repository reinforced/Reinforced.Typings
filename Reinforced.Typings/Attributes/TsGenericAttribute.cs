using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    /// Denotes type for generic attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.GenericParameter)]
    public class TsGenericAttribute : TsTypedAttributeBase
    {
        /// <summary>
        /// Constructs new instance of TsGenericAttribute
        /// </summary>
        /// <param name="type">Raw TypeScript type name</param>
        public TsGenericAttribute(string type)
        {
            Type = type;
        }

        /// <summary>
        /// Constructs new instance of TsGenericAttribute
        /// </summary>
        /// <param name="strongType">Type to be resolved to TypeScript name during export</param>
        public TsGenericAttribute(Type strongType)
        {
            StrongType = strongType;
        }
    }
}
