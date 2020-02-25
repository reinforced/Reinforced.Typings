using System.Xml.Serialization;

namespace Reinforced.Typings.Cli
{
    [XmlRoot]
    public class Regexes
    {
        [XmlArray()]

        [XmlArrayItem("Regex")]
        public AssemblyRegex[] Items { get; set; }
    }
    /// <summary>
    /// Assembly regex replacement configuration group
    /// </summary>
    public class AssemblyRegex
    {
        /// <summary>
        /// Assembly path pattern to match
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Assembly pattern replacement
        /// </summary>
        public string Replace { get; set; }
    }
    
}
