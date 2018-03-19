using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    ///     Various extensions for settings
    /// </summary>
    public static class ContextExtensions
    {
        /// <summary>
        ///     Conditionally (based on settings) turns method name to camelCase
        /// </summary>
        /// <param name="context">Settings object</param>
        /// <param name="regularName">Regular method name</param>
        /// <returns>Method name in camelCase if camelCasing enabled, initial string otherwise</returns>
        public static string ConditionallyConvertMethodNameToCamelCase(this ExportContext context, string regularName)
        {
            if (!context.Global.CamelCaseForMethods) return regularName;
            return TypeBlueprint.ConvertToCamelCase(regularName);
        }

        /// <summary>
        ///     Conditionally (based on settings) turns method name to PascalCase
        /// </summary>
        /// <param name="context">Settings object</param>
        /// <param name="regularName">Regular method name</param>
        /// <returns>Method name in camelCase if camelCasing enabled, initial string otherwise</returns>
        public static string ConditionallyConvertMethodNameToPascalCase(this ExportContext context, string regularName)
        {
            if (!context.Global.CamelCaseForMethods) return regularName;
            return TypeBlueprint.ConvertToCamelCase(regularName);
        }

        /// <summary>
        ///     Conditionally (based on settings) turns property name to camelCase
        /// </summary>
        /// <param name="context">Settings object</param>
        /// <param name="regularName">Regular property name</param>
        /// <returns>Property name in camelCase if camelCasing enabled, initial string otherwise</returns>
        public static string ConditionallyConvertPropertyNameToCamelCase(this ExportContext context,
            string regularName)
        {
            if (!context.Global.CamelCaseForProperties) return regularName;
            return TypeBlueprint.ConvertToCamelCase(regularName);
        }
    }
}