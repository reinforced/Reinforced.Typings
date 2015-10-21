using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Attributes
{
    public interface INameOverrideAttribute
    {
        string Name { get; set; }
    }

    public interface ICamelCaseableAttribute
    {
        bool ShouldBeCamelCased { get; set; }
    }
}
