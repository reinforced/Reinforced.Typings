using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Specifies path of reference which required to be added to result .ts file
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class TsImportAttribute : Attribute
    {
        //todo
    }
}
