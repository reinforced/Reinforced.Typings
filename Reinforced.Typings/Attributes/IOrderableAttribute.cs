using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    /// Configuration interface for attribute for supporting reordering from attribute
    /// </summary>
    public interface IOrderableAttribute
    {
        /// <summary>
        /// Sets order this membter will be written to output file in
        /// </summary>
        double Order { get; set; }
    }
}
