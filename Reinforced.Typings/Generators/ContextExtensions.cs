using System;
using System.Linq;
using System.Reflection;
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
            return ConvertToCamelCase(regularName);
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
            return ConvertToCamelCase(regularName);
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
            return ConvertToCamelCase(regularName);
        }

        /// <summary>
        ///     Conditionally (based on attribute setting) turns member name to camelCase
        /// </summary>
        /// <param name="member">Member</param>
        /// <param name="regularName">Regular property name</param>
        /// <returns>Property name in camelCase if camelCasing enabled, initial string otherwise</returns>
        public static string CamelCaseFromAttribute(this MemberInfo member, string regularName)
        {
            var attr = ConfigurationRepository.Instance.ForMember<TsTypedMemberAttributeBase>(member);
            if (attr == null) return regularName;
            if (attr.ShouldBeCamelCased) return ConvertToCamelCase(regularName);
            return regularName;
        }

        /// <summary>
        ///     Conditionally (based on attribute setting) turns member name to PascalCase
        /// </summary>
        /// <param name="member">Member</param>
        /// <param name="regularName">Regular property name</param>
        /// <returns>Property name in PascalCase if pascalCasing enabled, initial string otherwise</returns>
        public static string PascalCaseFromAttribute(this MemberInfo member, string regularName)
        {
            var attr = ConfigurationRepository.Instance.ForMember<TsTypedMemberAttributeBase>(member);
            if (attr == null) return regularName;
            if (attr.ShouldBePascalCased) return ConvertToPascalCase(regularName);
            return regularName;
        }

        private static string ConvertToCamelCase(string s)
        {
            if (!char.IsLetter(s[0])) return s;
            if (char.IsUpper(s[0]))
            {
                return char.ToLower(s[0]) + s.Substring(1);
            }
            return s;
        }

        private static string ConvertToPascalCase(string s)
        {
            if (!char.IsLetter(s[0])) return s;
            if (char.IsLower(s[0]))
            {
                return char.ToUpper(s[0]) + s.Substring(1);
            }
            return s;
        }
    }
}