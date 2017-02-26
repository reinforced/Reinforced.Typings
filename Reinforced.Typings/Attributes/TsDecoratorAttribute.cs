using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    /// Adds decorator to class/method/parameter/property
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Field, AllowMultiple = true)]
    public class TsDecoratorAttribute : Attribute
    {
        /// <summary>
        /// Decorator text - everything that must follow after @
        /// </summary>
        public string Decorator { get; set; }

        /// <summary>
        /// Creates decorator attribute
        /// </summary>
        /// <param name="decorator">Decorator text - everything that follows after @</param>
        public TsDecoratorAttribute(string decorator)
        {
            Decorator = decorator;
        }
    }
}
