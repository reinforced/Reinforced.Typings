using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    /// Specifies path of reference which required to be added to result .ts file
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly,AllowMultiple = true)]
    public class TsReferenceAttribute : Attribute
    {
        /// <summary>
        /// Path to referenced TS file
        /// </summary>
        public virtual string Path { get; private set; }

        /// <summary>
        /// Constructs new instance of TsReferenceAttribute
        /// </summary>
        /// <param name="path">Path that should be written as file to reference tag</param>
        public TsReferenceAttribute(string path)
        {
            Path = path;
        }
    }
}
