using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum, AllowMultiple = true)]
    public class TsAddTypeImportAttribute : Attribute
    {
        //todo
    }
}
