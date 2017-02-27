using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Ast.Dependency;

namespace Reinforced.Typings.ReferencesInspection
{
    sealed class ReferenceComparer : IEqualityComparer<RtReference>
    {
        public bool Equals(RtReference x, RtReference y)
        {
            return x.Path == y.Path;
        }

        public int GetHashCode(RtReference obj)
        {
            return obj.Path.GetHashCode();
        }

        private static readonly ReferenceComparer _instance = new ReferenceComparer();

        public static ReferenceComparer Instance
        {
            get { return _instance; }
        }
    }
}
