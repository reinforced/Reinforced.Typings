using Reinforced.Typings.Fluent.Interfaces;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Extensions for fluent configuration
    /// </summary>
    public static partial class TypingsConfigurationExtensions
    {
        /// <summary>
        ///     Ignores specified mambers during exporting
        /// </summary>
        public static T Ignore<T>(this T conf) where T : IIgnorable
        {
            conf.Ignore = true;
            return conf;
        }

        
        #region Auto export control - temporary disabled

        ///// <summary>
        ///// Makes generator automatically look up for type methods and export them. 
        ///// Please note that this configuration setting will also export static and private members with corresponding modifiers.
        ///// </summary>
        ///// <param name="conf">Configuration</param>
        ///// <param name="export">Export or not</param>
        //public static T AutomaticallyExportMethods<T>(this T conf, bool export = true) where T : IExportConfiguration<IAutoexportSwitchAttribute>
        //{
        //    conf.AttributePrototype.AutoExportMethods = export;
        //    return conf;
        //}

        ///// <summary>
        ///// Makes generator automatically look up for type properties and export them. 
        ///// Please note that this configuration setting will also export static and private members with corresponding modifiers.
        ///// </summary>
        ///// <param name="conf">Configuration</param>
        ///// <param name="export">Export or not</param>
        //public static T AutomaticallyExportProperties<T>(this T conf, bool export = true) where T : IExportConfiguration<IAutoexportSwitchAttribute>
        //{
        //    conf.AttributePrototype.AutoExportProperties = export;
        //    return conf;
        //}

        ///// <summary>
        ///// Makes generator automatically look up for type fields and export them as TS fields. 
        ///// Please note that this configuration setting will also export static and private members with corresponding modifiers.
        ///// </summary>
        ///// <param name="conf">Configuration</param>
        ///// <param name="export">Export or not</param>
        //public static T AutomaticallyExportFields<T>(this T conf, bool export = false) where T : IExportConfiguration<TsClassAttribute>
        //{
        //    conf.AttributePrototype.AutoExportFields = export;
        //    return conf;
        //}

        ///// <summary>
        ///// Makes generator automatically look up for type constructors and export them as empty constructors. 
        ///// </summary>
        ///// <param name="conf">Configuration</param>
        ///// <param name="export">Export or not</param>
        //public static T AutomaticallyExportConstructors<T>(this T conf, bool export = false) where T : IExportConfiguration<TsClassAttribute>
        //{
        //    conf.AttributePrototype.AutoExportConstructors = export;
        //    return conf;
        //}

        ///// <summary>
        ///// Sets default code generator for each method among class is being exported
        ///// </summary>
        ///// <param name="conf">Configuration</param>
        //public static IExportConfiguration<TsClassAttribute> DefaultMethodCodeGenerator<T>(this IExportConfiguration<TsClassAttribute> conf) 
        //    where T : ITsCodeGenerator<MethodInfo>
        //{
        //    conf.AttributePrototype.DefaultMethodCodeGenerator = typeof(T);
        //    return conf;
        //}

        #endregion
    }
}