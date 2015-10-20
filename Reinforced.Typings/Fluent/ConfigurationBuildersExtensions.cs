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

        #region Properties

        /// <summary>
        /// Include specified property to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="property">Property to include</param>
        /// <returns>Fluent</returns>
        public static IExportConfiguration<TsPropertyAttribute> WithProperty<T, TData>(this TypeConfigurationBuilder<T> tc, Expression<Func<T, TData>> property)
        {
            var prop = LambdaHelpers.ParsePropertyLambda(property);
            ITypeConfigurationBuilder tcb = tc;
            return (IExportConfiguration<TsPropertyAttribute>)tcb.MembersConfiguration.GetOrCreate(prop, () => new PropertyExportConfiguration());
        }

        /// <summary>
        /// Include specified properties to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="predicate">Predicate function that should mathc for properties to include</param>
        /// <returns>Fluent</returns>
        public static T WithProperties<T>(this T tc, Func<PropertyInfo, bool> predicate) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetProperties(TypeExtensions.MembersFlags).Where(predicate);
            return ApplyMembersConfiguration(tc, prop);
        }

        /// <summary>
        /// Include specified properties to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="predicate">Predicate function for properties to include</param>
        /// <param name="configuration">Configuration to be applied to each property</param>
        /// <returns>Fluent</returns>
        public static T WithProperties<T>(this T tc, Func<PropertyInfo, bool> predicate, Action<IExportConfiguration<TsPropertyAttribute>> configuration) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetProperties(TypeExtensions.MembersFlags).Where(predicate);
            return ApplyMembersConfiguration(tc, prop, configuration);
        }

        /// <summary>
        /// Include specified properties to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="bindingFlags">BindingFlags describing properties to include</param>
        /// <param name="configuration">Configuration to be applied to each property</param>
        /// <returns>Fluent</returns>
        public static T WithProperties<T>(this T tc, BindingFlags bindingFlags, Action<IExportConfiguration<TsPropertyAttribute>> configuration)
            where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetProperties(bindingFlags);
            return ApplyMembersConfiguration(tc, prop, configuration);
        }

        /// <summary>
        /// Include specified properties to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="bindingFlags">BindingFlags describing properties to include</param>
        /// <returns>Fluent</returns>
        public static T WithProperties<T>(this T tc, BindingFlags bindingFlags) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetProperties(bindingFlags);
            return ApplyMembersConfiguration(tc, prop);
        }

        /// <summary>
        /// Include all properties to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <returns>Fluent</returns>
        public static T WithAllProperties<T>(this T tc) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetProperties(TypeExtensions.MembersFlags);
            return ApplyMembersConfiguration(tc, prop);
        }

        /// <summary>
        /// Include all public properties to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <returns>Fluent</returns>
        public static T WithPublicProperties<T>(this T tc) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return ApplyMembersConfiguration(tc, prop);
        }
        #endregion

        #region Fields
        /// <summary>
        /// Include specified field to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="field">Field to include</param>
        /// <returns>Fluent</returns>
        public static IExportConfiguration<TsPropertyAttribute> WithField<T, TData>(this TypeConfigurationBuilder<T> tc,Expression<Func<T, TData>> field)
        {
            var prop = LambdaHelpers.ParseFieldLambda(field);
            ITypeConfigurationBuilder tcb = tc;
            return (IExportConfiguration<TsPropertyAttribute>)tcb.MembersConfiguration.GetOrCreate(prop, () => new PropertyExportConfiguration());
        }

        /// <summary>
        /// Include specified fields to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="predicate">Predicate function that should mathc for fields to include</param>
        /// <param name="configuration">Configuration to be applied to each field</param>
        /// <returns>Fluent</returns>
        public static T WithFields<T>(this T tc, Func<FieldInfo, bool> predicate, Action<IExportConfiguration<TsPropertyAttribute>> configuration) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetFields(TypeExtensions.MembersFlags).Where(predicate);
            return ApplyMembersConfiguration(tc, prop, configuration);
        }

        /// <summary>
        /// Include specified fields to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="predicate">Predicate function for fields to include</param>
        /// <returns>Fluent</returns>
        public static T WithFields<T>(this T tc, Func<FieldInfo, bool> predicate) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetFields(TypeExtensions.MembersFlags).Where(predicate);
            return ApplyMembersConfiguration(tc, prop);
        }

        /// <summary>
        /// Include specified fields to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="bindingFlags">BindingFlags describing fields to include</param>
        /// <returns>Fluent</returns>
        public static T WithFields<T>(this T tc, BindingFlags bindingFlags) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetFields(bindingFlags);
            return ApplyMembersConfiguration(tc, prop);
        }

        /// <summary>
        /// Include all fields to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <returns>Fluent</returns>
        public static T WithAllFields<T>(this T tc) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetFields(TypeExtensions.MembersFlags);
            return ApplyMembersConfiguration(tc, prop);
        }

        /// <summary>
        /// Include all public fields to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <returns>Fluent</returns>
        public static T WithPublicFields<T>(this T tc) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            return ApplyMembersConfiguration(tc, prop);
        }

        /// <summary>
        /// Include specified fields to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="bindingFlags">BindingFlags describing fields to include</param>
        /// <param name="configuration">Configuration to be applied to each field</param>
        /// <returns>Fluent</returns>
        public static T WithFields<T>(this T tc, BindingFlags bindingFlags, Action<IExportConfiguration<TsPropertyAttribute>> configuration) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetFields(bindingFlags);
            return ApplyMembersConfiguration(tc, prop, configuration);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Include specified method to resulting typing. 
        /// User <see cref="Ts.Parameter{T}()"/> to mock up method parameters or specify configuration for perticular method parameter
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="method">Method to include</param>
        /// <returns>Fluent</returns>
        public static IExportConfiguration<TsFunctionAttribute> WithMethod<T, TData>(this TypeConfigurationBuilder<T> tc, Expression<Func<T, TData>> method)
        {
            var prop = LambdaHelpers.ParseMethodLambda(method);
            ITypeConfigurationBuilder tcb = tc;
            var methodConf = (IExportConfiguration<TsFunctionAttribute>)tcb.MembersConfiguration.GetOrCreate(prop, () => new MethodExportConfiguration());
            ExtractParameters(tcb, method);
            return methodConf;
        }

        /// <summary>
        /// Include specified method to resulting typing. 
        /// User <see cref="Ts.Parameter{T}()"/> to mock up method parameters or specify configuration for perticular method parameter
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="method">Method to include</param>
        /// <returns>Fluent</returns>
        public static IExportConfiguration<TsFunctionAttribute> WithMethod<T>(this TypeConfigurationBuilder<T> tc, Expression<Action<T>> method)
        {
            var prop = LambdaHelpers.ParseMethodLambda(method);
            ITypeConfigurationBuilder tcb = tc;
            var methodConf = (IExportConfiguration<TsFunctionAttribute>)tcb.MembersConfiguration.GetOrCreate(prop, () => new MethodExportConfiguration());
            ExtractParameters(tcb,method);
            return methodConf;
        }

        /// <summary>
        /// Include specified methods to resulting typing. 
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="predicate">Predicate function that should mathc for methods to include</param>
        /// <param name="configuration">Configuration to be applied to each method</param>
        /// <returns>Fluent</returns>
        public static T WithMethods<T>(this T tc, Func<MethodInfo, bool> predicate, Action<IExportConfiguration<TsFunctionAttribute>> configuration) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetMethods(TypeExtensions.MembersFlags).Where(predicate);
            return ApplyMethodsConfiguration(tc, prop, configuration);
        }

        /// <summary>
        /// Include specified methods to resulting typing. 
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="bindingFlags">BindingFlags describing methods to include</param>
        /// <param name="configuration">Configuration to be applied to each method</param>
        /// <returns>Fluent</returns>
        public static T WithMethods<T>(this T tc, BindingFlags bindingFlags, Action<IExportConfiguration<TsFunctionAttribute>> configuration) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetMethods(bindingFlags);
            return ApplyMethodsConfiguration(tc, prop, configuration);
        }

        /// <summary>
        /// Include specified methods to resulting typing. 
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="predicate">Predicate function that should mathc for methods to include</param>
        /// <returns>Fluent</returns>
        public static T WithMethods<T>(this T tc, Func<MethodInfo, bool> predicate) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetMethods(TypeExtensions.MembersFlags).Where(predicate);
            return ApplyMethodsConfiguration(tc, prop);
        }

        /// <summary>
        /// Include specified methods to resulting typing. 
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="bindingFlags">BindingFlags describing methods to include</param>
        /// <returns>Fluent</returns>
        public static T WithMethods<T>(this T tc, BindingFlags bindingFlags) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetMethods(bindingFlags);
            return ApplyMethodsConfiguration(tc, prop);
        }

        /// <summary>
        /// Include all methods to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <returns>Fluent</returns>
        public static T WithAllMethods<T>(this T tc) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetMethods(TypeExtensions.MembersFlags);
            return ApplyMethodsConfiguration(tc, prop);
        }

        /// <summary>
        /// Include all methods to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <returns>Fluent</returns>
        public static T WithPublicMethods<T>(this T tc) where T : ITypeConfigurationBuilder
        {
            var prop = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            return ApplyMethodsConfiguration(tc, prop);
        }
        #endregion

        #region Interfaces

        /// <summary>
        /// Includes specified type to resulting typing exported as interface
        /// </summary>
        /// <typeparam name="T">Type to include</typeparam>
        /// <param name="builder">Configuration builder</param>
        /// <returns>Fluent</returns>
        public static InterfaceConfigurationBuilder<T> ExportAsInterface<T>(this ConfigurationBuilder builder)
        {
            return (InterfaceConfigurationBuilder<T>)builder.TypeConfigurationBuilders.GetOrCreate(typeof(T), () => new InterfaceConfigurationBuilder<T>());
        }

        /// <summary>
        /// Includes specified types to resulting typing exported as interfaces
        /// </summary>
        /// <param name="builder">Configuration builder</param>
        /// <param name="types">Types to include</param>
        /// <returns>Fluent</returns>
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

        /// <summary>
        /// Includes specified types to resulting typing exported as interfaces
        /// </summary>
        /// <param name="builder">Configuration builder</param>
        /// <param name="types">Types to include</param>
        /// <param name="configuration">Configuration to be applied to each type</param>
        /// <returns>Fluent</returns>
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

        /// <summary>
        /// Includes specified type to resulting typing exported as TypeScript class
        /// </summary>
        /// <typeparam name="T">Type to include</typeparam>
        /// <param name="builder">Configuration builder</param>
        /// <returns>Fluent</returns>
        public static ClassConfigurationBuilder<T> ExportAsClass<T>(this ConfigurationBuilder builder)
        {
            return (ClassConfigurationBuilder<T>)builder.TypeConfigurationBuilders.GetOrCreate(typeof(T), () => new ClassConfigurationBuilder<T>());
        }

        /// <summary>
        /// Includes specified types to resulting typing exported as TypeScript classes
        /// </summary>
        /// <param name="builder">Configuration builder</param>
        /// <param name="types">Types to include</param>
        /// <returns>Fluent</returns>
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

        /// <summary>
        /// Includes specified types to resulting typing exported as TypeScript classes
        /// </summary>
        /// <param name="builder">Configuration builder</param>
        /// <param name="types">Types to include</param>
        /// <param name="configuration">Configuration to be applied to each type</param>
        /// <returns>Fluent</returns>
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

        /// <summary>
        /// Includes specified type to resulting typing exported as TypeScript enumeration
        /// </summary>
        /// <typeparam name="T">Type to include</typeparam>
        /// <param name="builder">Configuration builder</param>
        /// <returns>Fluent</returns>
        public static EnumConfigurationBuilder<T> ExportAsEnum<T>(this ConfigurationBuilder builder)
            where T:struct 
        {
            return (EnumConfigurationBuilder<T>)builder.EnumConfigurationBuilders.GetOrCreate(typeof(T), () => new EnumConfigurationBuilder<T>());
        }

        /// <summary>
        /// Includes specified types to resulting typing exported as TypeScript enumerations
        /// </summary>
        /// <param name="builder">Configuration builder</param>
        /// <param name="types">Types to include</param>
        /// <returns>Fluent</returns>
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

        /// <summary>
        /// Includes specified types to resulting typing exported as TypeScript enumerations
        /// </summary>
        /// <param name="builder">Configuration builder</param>
        /// <param name="types">Types to include</param>
        /// <param name="configuration">Configuration to be applied to each type</param>
        /// <returns>Fluent</returns>
        public static void ExportAsEnums(this ConfigurationBuilder builder, IEnumerable<Type> types, Action<IEnumConfigurationBuidler> configuration)
        {
            foreach (var type in types)
            {
                var tp = type;
                var conf = builder.EnumConfigurationBuilders.GetOrCreate(type, () =>
                {
                    var t = typeof(EnumConfigurationBuilder<>).MakeGenericType(tp);
                    return (IEnumConfigurationBuidler)Activator.CreateInstance(t);
                });
                configuration(conf);
            }
        }

        /// <summary>
        /// Retrieves configuration builder for particular enumeration value
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="conf">Configuration builder</param>
        /// <param name="value">Enum value</param>
        /// <returns>Configuration builder</returns>
        public static EnumValueExportConfiguration Value<T>(this EnumConfigurationBuilder<T> conf, T value)
            where T:struct 
        {
            var n = Enum.GetName(typeof(T), value);
            var field = typeof (T).GetField(n);
            IEnumConfigurationBuidler ecb = conf;
            var c = ecb.ValueExportConfigurations.GetOrCreate(field, () => new EnumValueExportConfiguration());
            return c;
        }

        /// <summary>
        /// Retrieves configuration builder for particular enumeration value
        /// </summary>
        /// <param name="conf">Configuration builder</param>
        /// <param name="propertyName">String enum property name</param>
        /// <returns>Configuration builder</returns>
        public static EnumValueExportConfiguration Value(this IEnumConfigurationBuidler conf, string propertyName)
        {
            var field = conf.EnumType.GetField(propertyName);
            var c = conf.ValueExportConfigurations.GetOrCreate(field, () => new EnumValueExportConfiguration());
            return c;
        }
        #endregion

        private static void ExtractParameters(ITypeConfigurationBuilder conf, LambdaExpression methodLambda)
        {
            var mex = methodLambda.Body as MethodCallExpression;
            if (mex == null) throw new Exception("MethodCallExpression should be provided for .WithMethod call. Please use only lamba expressions in this place.");
            var mi = mex.Method;

            var methodParameters = mi.GetParameters();
            if (methodParameters.Length == 0) return;


            int i = 0;
            foreach (var expression in mex.Arguments)
            {
                ParameterInfo pi = methodParameters[i];
                i++;

                var call = expression as MethodCallExpression;
                if (call != null)
                {
                    if (call.Method.IsGenericMethod && call.Method.GetGenericMethodDefinition() == Ts.ParametrizedParameterMethod)
                    {
                        ParameterConfigurationBuilder pcb = (ParameterConfigurationBuilder)conf.ParametersConfiguration.GetOrCreate(pi, () => new ParameterConfigurationBuilder());

                        bool parsed = false;
                        var arg = call.Arguments[0] as LambdaExpression;
                        if (arg != null)
                        {
                            var delg = arg.Compile();
                            delg.DynamicInvoke(pcb);
                            parsed = true;
                        }
                        var uarg = call.Arguments[0] as UnaryExpression; // convert expression
                        if (uarg != null)
                        {
                            var operand = uarg.Operand as MethodCallExpression;
                            if (operand != null)
                            {
                                var actionArg = operand.Object as ConstantExpression;
                                if (actionArg != null)
                                {
                                    var value = actionArg.Value as MethodInfo;
                                    if (value != null)
                                    {
                                        var param = Expression.Parameter(typeof(ParameterConfigurationBuilder));
                                        var newCall = Expression.Call(value, param);
                                        var newLambda = Expression.Lambda(newCall, param);
                                        var delg = newLambda.Compile();
                                        delg.DynamicInvoke(pcb);
                                        parsed = true;
                                    }
                                }
                            }
                            if (!parsed) throw new Exception(String.Format("Sorry, but {0} is not very good idea for parameter configuration. Try using simple lambda expression.", call.Arguments[0]));
                        }
                    }
                }
            }
        }
    }
}
