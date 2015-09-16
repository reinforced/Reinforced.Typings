using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    /// Base attribute for class members and method parameters
    /// </summary>
    public abstract class TsTypedMemberAttributeBase : TsTypedAttributeBase
    {
        /// <summary>
        /// Overrides member name
        /// </summary>
        public virtual string Name { get; set; }
    }
}
