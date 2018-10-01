using System;

namespace Reinforced.Typings.Cli
{
    /// <summary>
    /// Denotes console help for parameter
    /// </summary>
    public class ConsoleHelpAttribute : Attribute
    {
        /// <summary>
        /// Help text
        /// </summary>
        public string HelpText { get; set; }

        public Required RequiredType { get; set; }


        public ConsoleHelpAttribute(string helpText,Required requiredType = Required.Not)
        {
            HelpText = helpText;
            RequiredType = requiredType;
        }
    }

    public enum Required
    {
        Is,
        Not,
        Partially
    }
}
