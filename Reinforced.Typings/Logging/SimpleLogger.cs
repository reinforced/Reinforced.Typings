using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Logging
{
    /// <summary>
    /// Simple generation process logger
    /// </summary>
    public static class SimpleLogger
    {
        /// <summary>
        /// Sets logger for generation
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="verbosity"></param>
        public static void SetLogger(ILogger logger, int verbosity)
        {
            _verbosity = verbosity;
            _logger = logger;

        }
        private static int _verbosity;

        private static ILogger _logger;

        /// <summary>
        /// Puts message to logger
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="verbosity">Verbosity</param>
        public static void Log(int verbosity, string message)
        {
            if (verbosity <= _verbosity)
            {
                _logger.WriteMessage(message);
            }
        }

        internal static void Log(this string s, int verbosity = 1)
        {
            if (verbosity <= _verbosity)
            {
                Log(verbosity, s);
            }
        }

        internal static void Log(this string s, int verbosity = 1, params object[] parameters)
        {
            if (verbosity <= _verbosity)
            {
                Log(verbosity, String.Format(s, parameters));
            }
        }

        internal static void LogArray(this string s, IEnumerable parameters, int verbosity = 1)
        {
            if (verbosity <= _verbosity)
            {
                string p = String.Join(", ", parameters);
                Log(verbosity, String.Format(s, p));
            }
        }
    }
}
