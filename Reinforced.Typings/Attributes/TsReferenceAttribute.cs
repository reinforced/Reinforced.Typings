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
        public string Path { get; private set; }

        public TsReferenceAttribute(string path)
        {
            Path = path;
        }
    }
}
