using System;
using System.Reflection;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    /// Default code generator for fields
    /// </summary>
    public class FieldCodeGenerator : PropertyCodeGenerator
    {
        /// <summary>
        /// That's it - overriden GetType for property since properties and fields are exported to TypeScript almost same way.
        /// </summary>
        /// <param name="mi">Member info (Fields behind)</param>
        /// <returns>Field type</returns>
        protected override Type GetType(MemberInfo mi)
        {
            FieldInfo pi = (FieldInfo)mi;
            return pi.FieldType;
        }
    }
}
