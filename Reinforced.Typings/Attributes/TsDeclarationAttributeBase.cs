namespace Reinforced.Typings.Attributes
{
    /// <summary>
    /// Base attribute for so-called compilation unit (class, enum, interface etc)
    /// </summary>
    public abstract class TsDeclarationAttributeBase : TsAttributeBase
    {
        /// <summary>
        /// Place to corresponding namespace
        /// </summary>
        public bool IncludeNamespace { get; set; }

        /// <summary>
        /// Overrides name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Overrides namespace
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Constructs new instance of TsDeclarationAttributeBase
        /// </summary>
        protected TsDeclarationAttributeBase()
        {
            IncludeNamespace = true;
        }
    }
}
