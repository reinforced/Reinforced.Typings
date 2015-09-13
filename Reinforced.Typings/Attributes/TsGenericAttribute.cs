using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    /// Denotes type for generic attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.GenericParameter)]
    public class TsGenericAttribute : TsAttributeBase
    {
        /// <summary>
        /// Overrides type name
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Overrides type
        /// </summary>
        public Type StrongType { get; set; }

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
