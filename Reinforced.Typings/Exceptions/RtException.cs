using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Exceptions
{
    /// <summary>
    /// Base class for RT exception. 
    /// All the RT exceptions will be provided to VisualStudio's errors tab
    /// </summary>
    public class RtException : Exception
    {
        /// <summary>
        /// Internal error code
        /// </summary>
        public int Code { get; private set; }

        /// <summary>
        /// Error subcategory
        /// </summary>
        public string Subcategory { get; private set; }

        /// <summary>
        /// Constructs new RT exception
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="code">Error code</param>
        /// <param name="subcategory">Error subcategory (optional)</param>
        public RtException(string message, int code, string subcategory = "") : base(message)
        {
            Code = code;
            Subcategory = subcategory;
        }
    }
}
