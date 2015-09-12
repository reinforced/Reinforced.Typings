using System;
using System.Reflection;

namespace Reinforced.WebTypings.Generators
{
    public class FieldCodeGenerator : PropertyCodeGenerator
    {
        protected override Type GetType(MemberInfo mi)
        {
            FieldInfo pi = (FieldInfo)mi;
            return pi.FieldType;
        }
    }
}
