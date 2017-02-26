using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Attributes
{
    public interface IOrderableAttribute
    {
        /// <summary>
        /// Sets order this membter will be written to output file in
        /// </summary>
        double Order { get; set; }
    }
}
