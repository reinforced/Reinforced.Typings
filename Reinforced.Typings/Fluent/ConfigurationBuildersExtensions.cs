using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Exceptions;
using Reinforced.Typings.Fluent.Generic;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Extensions for configuration builders
    /// </summary>
    public static class ConfigurationBuildersExtensions
    {
        private static T ApplyMembersConfiguration<T>(T tc, IEnumerable<MemberInfo> prop,
            Action<PropertyExportConfigurationBuilder> configuration = null)
            where T : ITypeConfigurationBuilder
        {
            foreach (var propertyInfo in prop)
            {
                var conf =
                    (PropertyExportConfigurationBuilder)
                    tc.MembersConfiguration.GetOrCreate(propertyInfo, () => new PropertyExportConfigurationBuilder(propertyInfo));
                if (configuration == null) continue;
                try
                {
                    configuration(conf);
                }
                catch (Exception ex)
                {
                    ErrorMessages.RTE0006_FluentSingleError.Throw(ex.Message, "property",
                        string.Format("{0}.{1}", propertyInfo.DeclaringType.FullName, propertyInfo.Name));
                }
            }
            return tc;
        }

        private static T ApplyMethodsConfiguration<T>(T tc, IEnumerable<MethodInfo> methds,
            Action<MethodConfigurationBuilder> configuration = null) where T : ITypeConfigurationBuilder
        {
            foreach (var methodInfo in methds)
            {
                var conf =
                    (MethodConfigurationBuilder)
                    tc.MembersConfiguration.GetOrCreate(methodInfo, () => new MethodConfigurationBuilder(methodInfo));
                if (configuration == null) continue;
                try
                {
                    configuration(conf);
                }
                catch (Exception ex)
                {
                    ErrorMessages.RTE0006_FluentSingleError.Throw(ex.Message, "method",
                        string.Format("{0}.{1}(...)", methodInfo.DeclaringType.FullName, methodInfo.Name));
                }
            }
            return tc;
        }

        private static void ExtractParameters(ITypeConfigurationBuilder conf, LambdaExpression methodLambda)
        {
            var mex = methodLambda.Body as MethodCallExpression;
            if (mex == null) ErrorMessages.RTE0008_FluentWithMethodError.Throw();
            var mi = mex.Method;

            var methodParameters = mi.GetParameters();
            if (methodParameters.Length == 0) return;


            var i = 0;
            foreach (var expression in mex.Arguments)
            {
                var pi = methodParameters[i];
                i++;

                var call = expression as MethodCallExpression;
                if (call != null)
                {
                    if (call.Method.IsGenericMethod &&
                        call.Method.GetGenericMethodDefinition() == Ts.ParametrizedParameterMethod)
                    {
                        var pcb =
                            (ParameterConfigurationBuilder)
                            conf.ParametersConfiguration.GetOrCreate(pi, () => new ParameterConfigurationBuilder(pi));

                        var parsed = false;
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
                            if (!parsed)
                                ErrorMessages.RTE0009_FluentWithMethodCouldNotParse.Throw(call.Arguments[0]);
                        }
                    }
                }
            }
        }

        #region Decorators

        /// <summary>
        /// Adds decorator to member
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conf">Member configurator</param>
        /// <param name="decorator">Decorator to add (everything that must follow after "@")</param>
        /// <param name="order">Order of appearence</param>
        /// <returns>Fluent</returns>
        public static T Decorator<T>(this T conf, string decorator, double order = 0) where T : IDecoratorsAggregator
        {
            conf.Decorators.Add(new TsDecoratorAttribute(decorator, order));
            return conf;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Include specified property to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="property">Property to include</param>
        /// <returns>Fluent</returns>
        public static PropertyExportConfigurationBuilder WithProperty<T, TData>(this TypeConfigurationBuilder<T> tc,
            Expression<Func<T, TData>> property)
        {
            var prop = LambdaHelpers.ParsePropertyLambda(property);
            ITypeConfigurationBuilder tcb = tc;
            return
                (PropertyExportConfigurationBuilder)
                tcb.MembersConfiguration.GetOrCreate(prop, () => new PropertyExportConfigurationBuilder(prop));
        }

        /// <summary>
        ///     Include specified property to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="property">Property to include</param>
        /// <param name="configuration">Configuration to be applied to selected property</param>
        /// <returns>Fluent</returns>
        public static InterfaceConfigurationBuilder<T> WithProperty<T, TData>(this InterfaceConfigurationBuilder<T> tc,
            Expression<Func<T, TData>> property, Action<PropertyExportConfigurationBuilder> configuration)
        {
            return tc.WithProperties(new[] { LambdaHelpers.ParsePropertyLambda(property) }, configuration);
        }

        /// <summary>
        ///     Include specified property to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="property">Property to include</param>
        /// <param name="configuration">Configuration to be applied to selected property</param>
        /// <returns>Fluent</returns>
        public static ClassConfigurationBuilder<T> WithProperty<T, TData>(this ClassConfigurationBuilder<T> tc,
            Expression<Func<T, TData>> property, Action<PropertyExportConfigurationBuilder> configuration)
        {
            return tc.WithProperties(new[] { LambdaHelpers.ParsePropertyLambda(property) }, configuration);
        }

        /// <summary>
        ///     Include specified properties to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="properties">Properties to include</param>
        /// <param name="configuration">Configuration to be applied to each property</param>
        /// <returns>Fluent</returns>
        public static T WithProperties<T>(this T tc, IEnumerable<PropertyInfo> properties,
            Action<PropertyExportConfigurationBuilder> configuration = null) where T : ITypeConfigurationBuilder
        {
            return ApplyMembersConfiguration(tc, properties, configuration);
        }

        /// <summary>
        ///     Include specified properties to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="predicate">Predicate function for properties to include</param>
        /// <param name="configuration">Configuration to be applied to each property</param>
        /// <returns>Fluent</returns>
        public static T WithProperties<T>(this T tc, Func<PropertyInfo, bool> predicate,
            Action<PropertyExportConfigurationBuilder> configuration = null) where T : ITypeConfigurationBuilder
        {
            var prop = tc.Type.GetProperties(TypeExtensions.MembersFlags).Where(predicate);
            return tc.WithProperties(prop, configuration);
        }

        /// <summary>
        ///     Include specified properties to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="bindingFlags">BindingFlags describing properties to include</param>
        /// <param name="configuration">Configuration to be applied to each property</param>
        /// <returns>Fluent</returns>
        public static T WithProperties<T>(this T tc, BindingFlags bindingFlags,
            Action<PropertyExportConfigurationBuilder> configuration = null)
            where T : ITypeConfigurationBuilder
        {
            var prop = tc.Type.GetProperties(bindingFlags);
            return tc.WithProperties(prop, configuration);
        }

        /// <summary>
        ///     Include all properties to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="configuration">Configuration to be applied to each property</param>
        /// <returns>Fluent</returns>
        public static T WithAllProperties<T>(this T tc, Action<PropertyExportConfigurationBuilder> configuration = null)
            where T : ITypeConfigurationBuilder
        {
            var prop = tc.Type.GetProperties(TypeExtensions.MembersFlags);
            return tc.WithProperties(prop, configuration);
        }

        /// <summary>
        ///     Include all public properties to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="configuration">Configuration to be applied to each property</param>
        /// <returns>Fluent</returns>
        public static T WithPublicProperties<T>(this T tc,
            Action<PropertyExportConfigurationBuilder> configuration = null)
            where T : ITypeConfigurationBuilder
        {
            var prop =
                tc.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static |
                                      BindingFlags.DeclaredOnly);
            return tc.WithProperties(prop, configuration);
        }

        #endregion

        #region Fields

        /// <summary>
        ///     Include specified field to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="field">Field to include</param>
        /// <returns>Fluent</returns>
        public static PropertyExportConfigurationBuilder WithField<T, TData>(this TypeConfigurationBuilder<T> tc,
            Expression<Func<T, TData>> field)
        {
            var prop = LambdaHelpers.ParseFieldLambda(field);
            ITypeConfigurationBuilder tcb = tc;
            return
                (PropertyExportConfigurationBuilder)
                tcb.MembersConfiguration.GetOrCreate(prop, () => new PropertyExportConfigurationBuilder(prop));
        }

        /// <summary>
        ///     Include specified property to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="property">Property to include</param>
        /// <param name="configuration">Configuration to be applied to selected property</param>
        /// <returns>Fluent</returns>
        public static InterfaceConfigurationBuilder<T> WithField<T, TData>(this InterfaceConfigurationBuilder<T> tc,
            Expression<Func<T, TData>> property, Action<PropertyExportConfigurationBuilder> configuration)
        {
            ApplyMembersConfiguration(tc, new[] { LambdaHelpers.ParseFieldLambda(property) }, configuration);
            return tc;
        }

        /// <summary>
        ///     Include specified property to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="property">Property to include</param>
        /// <param name="configuration">Configuration to be applied to selected property</param>
        /// <returns>Fluent</returns>
        public static ClassConfigurationBuilder<T> WithField<T, TData>(this ClassConfigurationBuilder<T> tc,
            Expression<Func<T, TData>> property, Action<PropertyExportConfigurationBuilder> configuration)
        {
            return tc.WithFields(new[] { LambdaHelpers.ParseFieldLambda(property) }, configuration);
        }

        /// <summary>
        ///     Include specified fields to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="fields">Fields to include</param>
        /// <param name="configuration">Configuration to be applied to each field</param>
        /// <returns>Fluent</returns>
        public static T WithFields<T>(this T tc, IEnumerable<FieldInfo> fields,
            Action<PropertyExportConfigurationBuilder> configuration = null) where T : ITypeConfigurationBuilder
        {
            return ApplyMembersConfiguration(tc, fields, configuration);
        }

        /// <summary>
        ///     Include specified fields to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="predicate">Predicate function that should mathc for fields to include</param>
        /// <param name="configuration">Configuration to be applied to each field</param>
        /// <returns>Fluent</returns>
        public static T WithFields<T>(this T tc, Func<FieldInfo, bool> predicate,
            Action<PropertyExportConfigurationBuilder> configuration = null) where T : ITypeConfigurationBuilder
        {
            var prop = tc.Type.GetFields(TypeExtensions.MembersFlags).Where(predicate);
            return tc.WithFields(prop, configuration);
        }

        /// <summary>
        ///     Include all fields to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="configuration">Configuration to be applied to each field</param>
        /// <returns>Fluent</returns>
        public static T WithAllFields<T>(this T tc, Action<PropertyExportConfigurationBuilder> configuration = null)
            where T : ITypeConfigurationBuilder
        {
            var prop = tc.Type.GetFields(TypeExtensions.MembersFlags);
            return tc.WithFields(prop, configuration);
        }

        /// <summary>
        ///     Include all public fields to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="configuration">Configuration to be applied to each field</param>
        /// <returns>Fluent</returns>
        public static T WithPublicFields<T>(this T tc, Action<PropertyExportConfigurationBuilder> configuration = null)
            where T : ITypeConfigurationBuilder
        {
            var prop =
                tc.Type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
                                  BindingFlags.DeclaredOnly);
            return tc.WithFields(prop, configuration);
        }

        /// <summary>
        ///     Include specified fields to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="bindingFlags">BindingFlags describing fields to include</param>
        /// <param name="configuration">Configuration to be applied to each field</param>
        /// <returns>Fluent</returns>
        public static T WithFields<T>(this T tc, BindingFlags bindingFlags,
            Action<PropertyExportConfigurationBuilder> configuration = null) where T : ITypeConfigurationBuilder
        {
            var prop = tc.Type.GetFields(bindingFlags);
            return tc.WithFields(prop, configuration);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Include specified method to resulting typing.
        ///     User <see cref="Ts.Parameter{T}()" /> to mock up method parameters or specify configuration for perticular method
        ///     parameter
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="method">Method to include</param>
        /// <returns>Fluent</returns>
        public static MethodConfigurationBuilder WithMethod<T, TData>(this TypeConfigurationBuilder<T> tc,
            Expression<Func<T, TData>> method)
        {
            var prop = LambdaHelpers.ParseMethodLambda(method);
            ITypeConfigurationBuilder tcb = tc;
            var methodConf =
                (MethodConfigurationBuilder)
                tcb.MembersConfiguration.GetOrCreate(prop, () => new MethodConfigurationBuilder(prop));
            ExtractParameters(tcb, method);
            return methodConf;
        }

        /// <summary>
        ///     Include specified method to resulting typing.
        ///     User <see cref="Ts.Parameter{T}()" /> to mock up method parameters or specify configuration for perticular method
        ///     parameter
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="method">Method to include</param>
        /// <param name="configuration">configuration to be applied to method</param>
        /// <returns>Fluent</returns>
        public static InterfaceConfigurationBuilder<T> WithMethod<T, TData>(this InterfaceConfigurationBuilder<T> tc,
            Expression<Func<T, TData>> method, Action<MethodConfigurationBuilder> configuration)
        {
            tc.WithMethods(new[] { LambdaHelpers.ParseMethodLambda(method) }, configuration);
            ITypeConfigurationBuilder tcb = tc;
            ExtractParameters(tcb, method);
            return tc;
        }

        /// <summary>
        ///     Include specified method to resulting typing.
        ///     User <see cref="Ts.Parameter{T}()" /> to mock up method parameters or specify configuration for perticular method
        ///     parameter
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="method">Method to include</param>
        /// <param name="configuration">Configuration to be applied to method</param>
        /// <returns>Fluent</returns>
        public static ClassConfigurationBuilder<T> WithMethod<T, TData>(this ClassConfigurationBuilder<T> tc,
            Expression<Func<T, TData>> method, Action<MethodConfigurationBuilder> configuration)
        {
            tc.WithMethods(new[] { LambdaHelpers.ParseMethodLambda(method) }, configuration);
            ITypeConfigurationBuilder tcb = tc;
            ExtractParameters(tcb, method);
            return tc;
        }

        /// <summary>
        ///     Include specified method to resulting typing.
        ///     User <see cref="Ts.Parameter{T}()" /> to mock up method parameters or specify configuration for perticular method
        ///     parameter
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="method">Method to include</param>
        /// <returns>Fluent</returns>
        public static MethodConfigurationBuilder WithMethod<T>(this TypeConfigurationBuilder<T> tc,
            Expression<Action<T>> method)
        {
            var prop = LambdaHelpers.ParseMethodLambda(method);
            ITypeConfigurationBuilder tcb = tc;
            var methodConf =
                (MethodConfigurationBuilder)
                tcb.MembersConfiguration.GetOrCreate(prop, () => new MethodConfigurationBuilder(prop));
            ExtractParameters(tcb, method);
            return methodConf;
        }

        /// <summary>
        ///     Include specified method to resulting typing.
        ///     User <see cref="Ts.Parameter{T}()" /> to mock up method parameters or specify configuration for perticular method
        ///     parameter
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="method">Method to include</param>
        /// <param name="configuration">Configuration to be applied to method</param>
        /// <returns>Fluent</returns>
        public static InterfaceConfigurationBuilder<T> WithMethod<T>(this InterfaceConfigurationBuilder<T> tc,
            Expression<Action<T>> method, Action<MethodConfigurationBuilder> configuration)
        {
            tc.WithMethods(new[] { LambdaHelpers.ParseMethodLambda(method) }, configuration);
            ITypeConfigurationBuilder tcb = tc;
            ExtractParameters(tcb, method);
            return tc;
        }

        /// <summary>
        ///     Include specified method to resulting typing.
        ///     User <see cref="Ts.Parameter{T}()" /> to mock up method parameters or specify configuration for perticular method
        ///     parameter
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="method">Method to include</param>
        /// <param name="configuration">Configuration to be applied to method</param>
        /// <returns>Fluent</returns>
        public static ClassConfigurationBuilder<T> WithMethod<T>(this ClassConfigurationBuilder<T> tc,
            Expression<Action<T>> method, Action<MethodConfigurationBuilder> configuration)
        {
            tc.WithMethods(new[] { LambdaHelpers.ParseMethodLambda(method) }, configuration);
            ITypeConfigurationBuilder tcb = tc;
            ExtractParameters(tcb, method);
            return tc;
        }

        /// <summary>
        ///     Include specified methods to resulting typing.
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="predicate">Predicate function that should mathc for methods to include</param>
        /// <param name="configuration">Configuration to be applied to each method</param>
        /// <returns>Fluent</returns>
        public static T WithMethods<T>(this T tc, Func<MethodInfo, bool> predicate,
            Action<MethodConfigurationBuilder> configuration = null) where T : ITypeConfigurationBuilder
        {
            var prop = tc.Type.GetMethods(TypeExtensions.MembersFlags).Where(predicate);
            return tc.WithMethods(prop, configuration);
        }

        /// <summary>
        ///     Include specified methods to resulting typing.
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="bindingFlags">BindingFlags describing methods to include</param>
        /// <param name="configuration">Configuration to be applied to each method</param>
        /// <returns>Fluent</returns>
        public static T WithMethods<T>(this T tc, BindingFlags bindingFlags,
            Action<MethodConfigurationBuilder> configuration = null) where T : ITypeConfigurationBuilder
        {
            var prop = tc.Type.GetMethods(bindingFlags);
            return tc.WithMethods(prop, configuration);
        }

        /// <summary>
        ///     Include specified methods to resulting typing.
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="methods">Methods to include</param>
        /// <param name="configuration">Configuration to be applied to each method</param>
        /// <returns>Fluent</returns>
        public static T WithMethods<T>(this T tc, IEnumerable<MethodInfo> methods,
            Action<MethodConfigurationBuilder> configuration = null) where T : ITypeConfigurationBuilder
        {
            return ApplyMethodsConfiguration(tc, methods, configuration);
        }

        /// <summary>
        ///     Include all methods to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="configuration">Configuration to be applied to each method</param>
        /// <returns>Fluent</returns>
        public static T WithAllMethods<T>(this T tc, Action<MethodConfigurationBuilder> configuration = null)
            where T : ITypeConfigurationBuilder
        {
            var prop = tc.Type.GetMethods(TypeExtensions.MembersFlags);
            return tc.WithMethods(prop, configuration);
        }

        /// <summary>
        ///     Include all methods to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="configuration">Configuration to be applied to each method</param>
        /// <returns>Fluent</returns>
        public static T WithPublicMethods<T>(this T tc, Action<MethodConfigurationBuilder> configuration = null)
            where T : ITypeConfigurationBuilder
        {
            var prop =
                tc.Type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
                                   BindingFlags.DeclaredOnly);
            return tc.WithMethods(prop, configuration);
        }

        #endregion

        #region Interfaces

        /// <summary>
        ///     Includes specified type to resulting typing exported as interface
        /// </summary>
        /// <typeparam name="T">Type to include</typeparam>
        /// <param name="builder">Configuration builder</param>
        /// <returns>Fluent</returns>
        public static InterfaceConfigurationBuilder<T> ExportAsInterface<T>(this ConfigurationBuilder builder)
        {
            return
                (InterfaceConfigurationBuilder<T>)
                builder.TypeConfigurationBuilders.GetOrCreate(typeof(T),
                    () => new InterfaceConfigurationBuilder<T>());
        }

        /// <summary>
        ///     Includes specified types to resulting typing exported as interfaces
        /// </summary>
        /// <param name="builder">Configuration builder</param>
        /// <param name="types">Types to include</param>
        /// <param name="configuration">Configuration to be applied to each type</param>
        /// <returns>Fluent</returns>
        public static void ExportAsInterfaces(this ConfigurationBuilder builder, IEnumerable<Type> types,
            Action<IInterfaceConfigurationBuilder> configuration = null)
        {
            foreach (var type in types)
            {
                var tp = type;
                var conf = (IInterfaceConfigurationBuilder)builder.TypeConfigurationBuilders.GetOrCreate(type, () =>
               {
                   Type t = null;
                   if (!tp.IsGenericType)
                   {
                       t = typeof(InterfaceConfigurationBuilder<>).MakeGenericType(tp);
                       return (ITypeConfigurationBuilder)Activator.CreateInstance(t, true);
                   }
                   t = typeof(GenericInterfaceConfigurationBuilder);
                   return (ITypeConfigurationBuilder)Activator.CreateInstance(t, tp);
               });
                if (configuration != null)
                {
                    try
                    {
                        configuration(conf);
                    }
                    catch (Exception ex)
                    {
                        ErrorMessages.RTE0006_FluentSingleError.Throw(ex.Message, "type", type.FullName);
                    }
                }
            }
        }

        #endregion

        #region Classes

        /// <summary>
        ///     Includes specified type to resulting typing exported as TypeScript class
        /// </summary>
        /// <typeparam name="T">Type to include</typeparam>
        /// <param name="builder">Configuration builder</param>
        /// <returns>Fluent</returns>
        public static ClassConfigurationBuilder<T> ExportAsClass<T>(this ConfigurationBuilder builder)
        {
            return
                (ClassConfigurationBuilder<T>)
                builder.TypeConfigurationBuilders.GetOrCreate(typeof(T), () => new ClassConfigurationBuilder<T>());
        }

        /// <summary>
        ///     Includes specified types to resulting typing exported as TypeScript classes
        /// </summary>
        /// <param name="builder">Configuration builder</param>
        /// <param name="types">Types to include</param>
        /// <param name="configuration">Configuration to be applied to each type</param>
        /// <returns>Fluent</returns>
        public static void ExportAsClasses(this ConfigurationBuilder builder, IEnumerable<Type> types,
            Action<IClassConfigurationBuilder> configuration = null)
        {
            foreach (var type in types)
            {
                var tp = type;
                var conf = (IClassConfigurationBuilder)builder.TypeConfigurationBuilders.GetOrCreate(type, () =>
               {
                   Type t = null;
                   if (!tp.IsGenericType)
                   {
                       t = typeof(ClassConfigurationBuilder<>).MakeGenericType(tp);
                       return (IClassConfigurationBuilder)Activator.CreateInstance(t, true);
                   }
                   t = typeof(GenericClassConfigurationBuilder);
                   return (ITypeConfigurationBuilder)Activator.CreateInstance(t, tp);
               });
                if (configuration != null)
                {
                    try
                    {
                        configuration(conf);
                    }
                    catch (Exception ex)
                    {
                        ErrorMessages.RTE0006_FluentSingleError.Throw(ex.Message, "type", type.FullName);
                    }
                }
            }
        }

        #endregion

        #region Enums

        /// <summary>
        ///     Includes specified type to resulting typing exported as TypeScript enumeration
        /// </summary>
        /// <typeparam name="T">Type to include</typeparam>
        /// <param name="builder">Configuration builder</param>
        /// <returns>Fluent</returns>
        public static EnumConfigurationBuilder<T> ExportAsEnum<T>(this ConfigurationBuilder builder)
            where T : struct
        {
            return
                (EnumConfigurationBuilder<T>)
                builder.EnumConfigurationBuilders.GetOrCreate(typeof(T), () => new EnumConfigurationBuilder<T>());
        }

        /// <summary>
        ///     Includes specified types to resulting typing exported as TypeScript enumerations
        /// </summary>
        /// <param name="builder">Configuration builder</param>
        /// <param name="types">Types to include</param>
        /// <param name="configuration">Configuration to be applied to each type</param>
        /// <returns>Fluent</returns>
        public static void ExportAsEnums(this ConfigurationBuilder builder, IEnumerable<Type> types,
            Action<IEnumConfigurationBuidler> configuration = null)
        {
            foreach (var type in types)
            {
                var tp = type;
                var conf = builder.EnumConfigurationBuilders.GetOrCreate(type, () =>
                {
                    var t = typeof(EnumConfigurationBuilder<>).MakeGenericType(tp);
                    return (IEnumConfigurationBuidler)Activator.CreateInstance(t, true);
                });
                if (configuration != null)
                {
                    try
                    {
                        configuration(conf);
                    }
                    catch (Exception ex)
                    {
                        ErrorMessages.RTE0006_FluentSingleError.Throw(ex.Message, "enum", type.FullName);
                    }
                }
            }
        }

        /// <summary>
        /// Turns enum to constant enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conf">Enum configurator</param>
        /// <param name="isConst">When true, "const enum" will be generated. Regular enum otherwise</param>
        /// <returns>Fluent</returns>
        public static T Const<T>(this T conf, bool isConst = true) where T : IEnumConfigurationBuidler
        {
            conf.AttributePrototype.IsConst = isConst;
            return conf;
        }

        /// <summary>
        ///     Retrieves configuration builder for particular enumeration value
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="conf">Configuration builder</param>
        /// <param name="value">Enum value</param>
        /// <returns>Configuration builder</returns>
        public static EnumValueExportConfiguration Value<T>(this EnumConfigurationBuilder<T> conf, T value)
            where T : struct
        {
            var n = Enum.GetName(typeof(T), value);
            var field = typeof(T).GetField(n);
            IEnumConfigurationBuidler ecb = conf;
            var c = ecb.ValueExportConfigurations.GetOrCreate(field, () => new EnumValueExportConfiguration(field));
            return c;
        }

        /// <summary>
        ///     Retrieves configuration builder for particular enumeration value
        /// </summary>
        /// <param name="conf">Configuration builder</param>
        /// <param name="propertyName">String enum property name</param>
        /// <returns>Configuration builder</returns>
        public static EnumValueExportConfiguration Value(this IEnumConfigurationBuidler conf, string propertyName)
        {
            var field = conf.EnumType.GetField(propertyName);
            var c = conf.ValueExportConfigurations.GetOrCreate(field, () => new EnumValueExportConfiguration(field));
            return c;
        }

        #endregion
    }
}