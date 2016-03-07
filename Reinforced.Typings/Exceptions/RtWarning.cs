using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Exceptions
{
    /// <summary>
    /// Represents warning message that could be displayed during build
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

        public RtWarning(int code, string subcategory = null, string text = null)
        {
            Code = code;
            Subcategory = subcategory;
            Text = text;
        }
    }
}
