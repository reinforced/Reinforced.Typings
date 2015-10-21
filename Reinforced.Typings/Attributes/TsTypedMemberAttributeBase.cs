using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    /// Base attribute for class members and method parameters
    /// </summary>
    public abstract class TsTypedMemberAttributeBase : TsTypedAttributeBase, INameOverrideAttribute, ICamelCaseableAttribute
    {
        /// <summary>
        /// Overrides member name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// When true them member name will be camelCased regardless configuration setting
        /// </summary>
        public bool ShouldBeCamelCased { get; set; }
    }
}
