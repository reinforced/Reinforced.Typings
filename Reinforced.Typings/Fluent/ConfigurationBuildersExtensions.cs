using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Extensions for configuration builders
    /// </summary>
    public static class ConfigurationBuildersExtensions
    {
        #region Properties
        public static IExportConfiguration<TsPropertyAttribute> WithProperty<T, TData>(this TypeConfigurationBuilder<T> tc, Expression<Func<T, TData>> property)
        {
            var prop = LambdaHelpers.ParsePropertyLambda(property);
            ITypeConfigurationBuilder tcb = tc;
            return (IExportConfiguration<TsPropertyAttribute>)tcb.MembersConfiguration.GetOrCreate(prop, () => new PropertyExportConfiguration());
        }

        public static T WithProperties<T>(this T tc, Func<PropertyInfo, bool> predicate) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetProperties(TypeExtensions.MembersFlags).Where(predicate);
            return ApplyMembersConfiguration(tc, prop);
        }

        public static T WithProperties<T>(this T tc, Func<PropertyInfo, bool> predicate, Action<IExportConfiguration<TsPropertyAttribute>> configuration) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetProperties(TypeExtensions.MembersFlags).Where(predicate);
            return ApplyMembersConfiguration(tc, prop, configuration);
        }

        public static T WithProperties<T>(this T tc, BindingFlags bindingFlags, Action<IExportConfiguration<TsPropertyAttribute>> configuration)
            where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetProperties(bindingFlags);
            return ApplyMembersConfiguration(tc, prop, configuration);
        }

        public static T WithProperties<T>(this T tc, BindingFlags bindingFlags) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetProperties(bindingFlags);
            return ApplyMembersConfiguration(tc, prop);
        }

        public static T WithProperties<T>(this T tc) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetProperties(TypeExtensions.MembersFlags);
            return ApplyMembersConfiguration(tc, prop);
        }

        public static T WithPublicProperties<T>(this T tc) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return ApplyMembersConfiguration(tc, prop);
        }

        private static T ApplyMembersConfiguration<T>(T tc, IEnumerable<MemberInfo> prop, Action<IExportConfiguration<TsPropertyAttribute>> configuration = null)
            where T : ITypeConfigurationBuilder
        {
            foreach (var propertyInfo in prop)
            {
                var conf = (IExportConfiguration<TsPropertyAttribute>)tc.MembersConfiguration.GetOrCreate(propertyInfo, () => new PropertyExportConfiguration());
                if (configuration != null) configuration(conf);
            }
            return tc;
        }

        private static T ApplyMethodsConfiguration<T>(T tc, IEnumerable<MethodInfo> methds,
            Action<IExportConfiguration<TsFunctionAttribute>> configuration = null) where T : ITypeConfigurationBuilder
        {
            foreach (var methodInfo in methds)
            {
                var conf = (IExportConfiguration<TsFunctionAttribute>)tc.MembersConfiguration.GetOrCreate(methodInfo, () => new MethodExportConfiguration());
                if (configuration != null) configuration(conf);
            }
            return tc;
        }
        #endregion

        #region Fields
        public static IExportConfiguration<TsPropertyAttribute> WithField<T, TData>(this TypeConfigurationBuilder<T> tc,Expression<Func<T, TData>> field)
        {
            var prop = LambdaHelpers.ParseFieldLambda(field);
            ITypeConfigurationBuilder tcb = tc;
            return (IExportConfiguration<TsPropertyAttribute>)tcb.MembersConfiguration.GetOrCreate(prop, () => new PropertyExportConfiguration());
        }

        public static T WithFields<T>(this T tc, Func<FieldInfo, bool> predicate, Action<IExportConfiguration<TsPropertyAttribute>> configuration) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetFields(TypeExtensions.MembersFlags).Where(predicate);
            return ApplyMembersConfiguration(tc, prop, configuration);
        }

        public static T WithFields<T>(this T tc, Func<FieldInfo, bool> predicate) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetFields(TypeExtensions.MembersFlags).Where(predicate);
            return ApplyMembersConfiguration(tc, prop);
        }

        public static T WithFields<T>(this T tc, BindingFlags bindingFlags) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetFields(bindingFlags);
            return ApplyMembersConfiguration(tc, prop);
        }
        public static T WithFields<T>(this T tc) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetFields(TypeExtensions.MembersFlags);
            return ApplyMembersConfiguration(tc, prop);
        }
        public static T WithPublicFields<T>(this T tc) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            return ApplyMembersConfiguration(tc, prop);
        }

        public static T WithFields<T>(this T tc, BindingFlags bindingFlags, Action<IExportConfiguration<TsPropertyAttribute>> configuration) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetFields(bindingFlags);
            return ApplyMembersConfiguration(tc, prop, configuration);
        }
        #endregion

        #region Methods
        public static IExportConfiguration<TsFunctionAttribute> WithMethod<T, TData>(this TypeConfigurationBuilder<T> tc, Expression<Func<T, TData>> method)
        {
            var prop = LambdaHelpers.ParseMethodLambda(method);
            ITypeConfigurationBuilder tcb = tc;
            var methodConf = (IExportConfiguration<TsFunctionAttribute>)tcb.MembersConfiguration.GetOrCreate(prop, () => new MethodExportConfiguration());
            //todo parameters!
            return methodConf;
        }

        public static IExportConfiguration<TsFunctionAttribute> WithMethod<T>(this TypeConfigurationBuilder<T> tc, Expression<Action<T>> method)
        {
            var prop = LambdaHelpers.ParseMethodLambda(method);
            ITypeConfigurationBuilder tcb = tc;
            var methodConf = (IExportConfiguration<TsFunctionAttribute>)tcb.MembersConfiguration.GetOrCreate(prop, () => new MethodExportConfiguration());
            //todo parameters!
            return methodConf;
        }

        public static T WithMethods<T>(this T tc, Func<MethodInfo, bool> predicate, Action<IExportConfiguration<TsFunctionAttribute>> configuration) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetMethods(TypeExtensions.MembersFlags).Where(predicate);
            return ApplyMethodsConfiguration(tc, prop, configuration);
        }

        public static T WithMethods<T>(this T tc, BindingFlags bindingFlags, Action<IExportConfiguration<TsFunctionAttribute>> configuration) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetMethods(bindingFlags);
            return ApplyMethodsConfiguration(tc, prop, configuration);
        }

        public static T WithMethods<T>(this T tc, Func<MethodInfo, bool> predicate) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetMethods(TypeExtensions.MembersFlags).Where(predicate);
            return ApplyMethodsConfiguration(tc, prop);
        }

        public static T WithMethods<T>(this T tc, BindingFlags bindingFlags) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetMethods(bindingFlags);
            return ApplyMethodsConfiguration(tc, prop);
        }

        public static T WithMethods<T>(this T tc) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetMethods(TypeExtensions.MembersFlags);
            return ApplyMethodsConfiguration(tc, prop);
        }
        public static T WithPublicMethods<T>(this T tc) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            return ApplyMethodsConfiguration(tc, prop);
        }
        #endregion

        #region Interfaces

        public static InterfaceConfigurationBuilder<T> ExportAsInterface<T>(this ConfigurationBuilder builder)
        {
            return (InterfaceConfigurationBuilder<T>)builder.TypeConfigurationBuilders.GetOrCreate(typeof(T), () => new InterfaceConfigurationBuilder<T>());
        }

        public static void ExportAsInterfaces(this ConfigurationBuilder builder, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                var tp = type;
                builder.TypeConfigurationBuilders.GetOrCreate(type, () =>
                {
                    var t = typeof(InterfaceConfigurationBuilder<>).MakeGenericType(tp);
                    return (ITypeConfigurationBuilder)Activator.CreateInstance(t);
                });
            }
        }

        public static void ExportAsInterfaces(this ConfigurationBuilder builder, IEnumerable<Type> types, Action<IInterfaceConfigurationBuilder> configuration)
        {
            foreach (var type in types)
            {
                var tp = type;
                var conf = (IInterfaceConfigurationBuilder)builder.TypeConfigurationBuilders.GetOrCreate(type, () =>
                {
                    var t = typeof(InterfaceConfigurationBuilder<>).MakeGenericType(tp);
                    return (ITypeConfigurationBuilder)Activator.CreateInstance(t);
                });
                configuration(conf);
            }
        }

        #endregion

        #region Classes
        public static ClassConfigurationBuilder<T> ExportAsClass<T>(this ConfigurationBuilder builder)
        {
            return (ClassConfigurationBuilder<T>)builder.TypeConfigurationBuilders.GetOrCreate(typeof(T), () => new ClassConfigurationBuilder<T>());
        }

        public static void ExportAsClasses(this ConfigurationBuilder builder, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                var tp = type;
                builder.TypeConfigurationBuilders.GetOrCreate(type, () =>
                {
                    var t = typeof(ClassConfigurationBuilder<>).MakeGenericType(tp);
                    return (ITypeConfigurationBuilder)Activator.CreateInstance(t);
                });
            }
        }

        public static void ExportAsClasses(this ConfigurationBuilder builder, IEnumerable<Type> types, Action<IClassConfigurationBuilder> configuration)
        {
            foreach (var type in types)
            {
                var tp = type;
                var conf = (IClassConfigurationBuilder)builder.TypeConfigurationBuilders.GetOrCreate(type, () =>
                {
                    var t = typeof(ClassConfigurationBuilder<>).MakeGenericType(tp);
                    return (IClassConfigurationBuilder)Activator.CreateInstance(t);
                });
                configuration(conf);
            }
        }
        #endregion

        #region Enums
        public static EnumConfigurationBuilder<T> ExportAsEnum<T>(this ConfigurationBuilder builder)
            where T:struct 
        {
            return (EnumConfigurationBuilder<T>)builder.EnumConfigurationBuilders.GetOrCreate(typeof(T), () => new EnumConfigurationBuilder<T>());
        }

        public static void ExportAsEnums(this ConfigurationBuilder builder, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                var tp = type;
                builder.EnumConfigurationBuilders.GetOrCreate(type, () =>
                {
                    var t = typeof(EnumConfigurationBuilder<>).MakeGenericType(tp);
                    return (IEnumConfigurationBuidler)Activator.CreateInstance(t);
                });
            }
        }

        public static void ExportAsEnums(this ConfigurationBuilder builder, IEnumerable<Type> types, Action<IEnumConfigurationBuidler> configuration)
        {
            foreach (var type in types)
            {
                var tp = type;
                var conf = (IEnumConfigurationBuidler)builder.EnumConfigurationBuilders.GetOrCreate(type, () =>
                {
                    var t = typeof(EnumConfigurationBuilder<>).MakeGenericType(tp);
                    return (IEnumConfigurationBuidler)Activator.CreateInstance(t);
                });
                configuration(conf);
            }
        }

        public static EnumValueExportConfiguration Value<T>(this EnumConfigurationBuilder<T> conf, T value)
            where T:struct 
        {
            var n = Enum.GetName(typeof(T), value);
            var field = typeof (T).GetField(n);
            IEnumConfigurationBuidler ecb = conf;
            var c = ecb.ValueExportConfigurations.GetOrCreate(field, () => new EnumValueExportConfiguration());
            return c;
        }

        public static EnumValueExportConfiguration Value(this IEnumConfigurationBuidler conf, string propertyName)
        {
            var field = conf.EnumType.GetField(propertyName);
            var c = conf.ValueExportConfigurations.GetOrCreate(field, () => new EnumValueExportConfiguration());
            return c;
        }
        #endregion
    }
}
