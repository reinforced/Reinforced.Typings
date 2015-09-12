namespace Reinforced.WebTypings
{
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

        protected TsDeclarationAttributeBase()
        {
            IncludeNamespace = true;
        }
    }
}
