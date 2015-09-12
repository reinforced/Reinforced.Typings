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

        public TsGenericAttribute(string type)
        {
            Type = type;
        }

        public TsGenericAttribute(Type strongType)
        {
            StrongType = strongType;
        }
    }
}
