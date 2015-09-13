using System;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    /// Various extensions for settings
    /// </summary>
    public static class SettingsExtensions
    {
        /// <summary>
        /// Returns declaration format for supplied Type (export/declare/nothing)
        /// </summary>
        /// <param name="settings">Export settings</param>
        /// <param name="element">Type</param>
        /// <returns>Declaration format - just supply your compilation unit type and name</returns>
        public static string GetDeclarationFormat(this ExportSettings settings,Type element)
        {
            var ns = element.GetNamespace();
            bool needsExports = !string.IsNullOrEmpty(ns);
            if (settings.ExportPureTypings)
            {
                if (!needsExports) return "declare {0} "; //no export directive allowed in .d.ts
                return "export {0} ";
            }
            if (needsExports) return "export {0} ";
            return "{0} ";
        }
    }
}
