using Reinforced.Typings.Exceptions;

namespace Reinforced.Typings.Cli
{
    class ErrorMessages
    {
        /// <summary>
        /// Error of fluent method loading
        /// </summary>
        public static readonly ErrorMessage RTW0009_CannotFindFluentMethod = new ErrorMessage(0009, "Cannot find configured fluent method '{0}'", "Type loading");
        
        /// <summary>
        /// Error of suppressed warning code parsing
        /// </summary>
        public static readonly ErrorMessage RTW0010_CannotParseWarningCode = new ErrorMessage(0010, "Cannot parse warning code '{0}'", "Parameters parsering");
        
        /// <summary>
        /// Error of configuration parameter parsing
        /// </summary>
        public static readonly ErrorMessage RTW0011_UnrecognizedConfigurationParameter = new ErrorMessage(0011, "Unrecognized configuration parameter: {0}", "Parameters parsering");
        
        /// <summary>
        /// Error of configuration parameter value parsing
        /// </summary>
        public static readonly ErrorMessage RTW0012_UnrecognizedConfigurationParameterValue = new ErrorMessage(0012, "Unrecognized configuration parameter value: {0}", "Parameters parsering");
        
        /// <summary>
        /// Warning of potentially incorrect assembly resolve
        /// </summary>
        public static readonly ErrorMessage RTW0013_AssemblyMayNotBeResolvedIncorrectly = new ErrorMessage(0013, "Assembly {0} may be resolved incorrectly", "Assembly loading");
        
        /// <summary>
        /// Warning of failed assembly load
        /// </summary>
        public static readonly ErrorMessage RTW0014_AssemblyFailedToLoad = new ErrorMessage(0014, "Assembly {0} failed to load: {1}", "Assembly loading");
    }
}