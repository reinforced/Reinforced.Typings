using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Fluent.Interfaces
{
    public interface IOrderableMember
    {
        /// <summary>
        /// Sets order this membter will be written to output file in
        /// </summary>
        double MemberOrder { get; set; }
    }
}
