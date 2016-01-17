using System;
using System.Linq;
using System.Reflection;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    ///     Various extensions for settings
    /// </summary>
    public static class SettingsExtensions
    {
        
        /// <summary>
        ///     Conditionally (based on settings) turns method name to camelCase
        /// </summary>
        /// <param name="settings">Settings object</param>
        /// <param name="regularName">Regular method name</param>
        /// <returns>Method name in camelCase if camelCasing enabled, initial string otherwise</returns>
        public static string ConditionallyConvertMethodNameToCamelCase(this ExportSettings settings, string regularName)
        {
            if (!settings.CamelCaseForMethods) return regularName;
            return ConvertToCamelCase(regularName);
        }

        /// <summary>
        ///     Conditionally (based on settings) turns property name to camelCase
        /// </summary>
        /// <param name="settings">Settings object</param>
        /// <param name="regularName">Regular property name</param>
        /// <returns>Property name in camelCase if camelCasing enabled, initial string otherwise</returns>
        public static string ConditionallyConvertPropertyNameToCamelCase(this ExportSettings settings,
            string regularName)
        {
            if (!settings.CamelCaseForProperties) return regularName;
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

        private static string ConvertToCamelCase(string s)
        {
            if (!char.IsLetter(s[0])) return s;
            if (char.IsUpper(s[0]))
            {
                return char.ToLower(s[0]) + s.Substring(1);
            }
            return s;
        }
    }
}