namespace Reinforced.Typings.Exceptions
{
    /// <summary>
    /// Represents warning message that could be displayed during build. 
    /// Warnings can be added to global warnings collection located at ExportContext.Warnings. 
    /// ExportContext instance can be found inside every TsCodeGeneratorBase
    /// </summary>
    public class RtWarning
    {
        /// <summary>
        /// Warning code
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Warning subcategory
        /// </summary>
        public string Subcategory { get; set; }

        /// <summary>
        /// Warning detailed text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Instantiates new RtWarning that is suitable be added to warnings collection.  
        /// </summary>
        /// <param name="code">Warning code</param>
        /// <param name="subcategory">Warning subcategory (optional). Important! Warning subcategory should not contain word "warning" and ":" symbol</param>
        /// <param name="text">Warning text</param>
        public RtWarning(int code, string subcategory = null, string text = null)
        {
            Code = code;
            Subcategory = subcategory;
            Text = text;
        }
    }
}
