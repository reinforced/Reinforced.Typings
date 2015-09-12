using System;

namespace Reinforced.WebTypings
{
    /// <summary>
    /// Specifies path of reference which required to be added to result .ts file
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly,AllowMultiple = true)]
    public class TsReference : Attribute
    {
        /// <summary>
        /// Path to referenced TS file
        /// </summary>
        public string Path { get; private set; }

        public TsReference(string path)
        {
            Path = path;
        }
    }
}
