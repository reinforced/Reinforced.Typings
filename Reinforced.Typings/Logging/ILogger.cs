using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Logging
{
    /// <summary>
    /// Simple logger interface. I'm using myself because dont want to add new dependencies
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Writes message to logger
        /// </summary>
        /// <param name="message"></param>
        void WriteMessage(string message);
    }
}
