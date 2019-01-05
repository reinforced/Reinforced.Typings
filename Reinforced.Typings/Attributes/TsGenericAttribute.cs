using System;
// ReSharper disable VirtualMemberCallInConstructor

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Denotes type for generic attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.GenericParameter)]
    public class TsGenericAttribute : TsTypedAttributeBase, ISupportsInferring<Type>
    {
        private readonly InlineTypeInferers<Type> _typeInferers = new InlineTypeInferers<Type>();

        /// <summary>
        ///     Constructs new instance of TsGenericAttribute
        /// </summary>
        /// <param name="type">Raw TypeScript type name</param>
        public TsGenericAttribute(string type)
        {
            Type = type;
        }

        /// <summary>
        ///     Constructs new instance of TsGenericAttribute
        /// </summary>
        /// <param name="strongType">Type to be resolved to TypeScript name during export</param>
        public TsGenericAttribute(Type strongType)
        {
            StrongType = strongType;
        }

        /// <summary>
        /// Type inferers set instance
        /// </summary>
        public InlineTypeInferers<Type> TypeInferers
        {
            get { return _typeInferers; }
            private set { }
        }
    }
}