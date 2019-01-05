using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent.New
{
    public class TypeExportBuilder
    {
        internal TypeBlueprint _blueprint;
        internal TypeExportBuilder(TypeBlueprint blueprint)
        {
            _blueprint = blueprint;
        }
    }
}
