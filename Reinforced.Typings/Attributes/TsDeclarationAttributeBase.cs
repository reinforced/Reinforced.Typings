namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Base attribute for so-called compilation unit (class, enum, interface etc)
    /// </summary>
    public abstract class TsDeclarationAttributeBase : TsAttributeBase, INameOverrideAttribute, IOrderableAttribute
    {
        /// <summary>
        ///     Constructs new instance of TsDeclarationAttributeBase
        /// </summary>
        protected TsDeclarationAttributeBase()
        {
            IncludeNamespace = true;
        }

        /// <summary>
        ///     Place to corresponding namespace
        /// </summary>
        public virtual bool IncludeNamespace { get; set; }

        /// <summary>
        ///     Overrides namespace
        /// </summary>
        public virtual string Namespace { get; set; }

        /// <summary>
        ///     Overrides name
        /// </summary>
        public virtual string Name { get; set; }

        public double Order { get; set; }
    }
}