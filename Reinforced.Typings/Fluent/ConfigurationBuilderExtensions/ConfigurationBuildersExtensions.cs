using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Exceptions;
using Reinforced.Typings.Fluent.Interfaces;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Extensions for configuration builders
    /// </summary>
    public static partial class ConfigurationBuildersExtensions
    {
        private static T ApplyMembersConfiguration<T>(T tc, IEnumerable<MemberInfo> prop,
            Action<PropertyExportConfigurationBuilder> configuration = null)
            where T : ITypeConfigurationBuilder
        {
            tc.Context.Project.Blueprint(tc.Type).NotifyFlattenTouched();
            foreach (var propertyInfo in prop)
            {
                var conf = new PropertyExportConfigurationBuilder(propertyInfo, tc.Context.Project.Blueprint(tc.Type));
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
            tc.Context.Project.Blueprint(tc.Type).NotifyFlattenTouched();
            foreach (var methodInfo in methds)
            {
                var conf = new MethodConfigurationBuilder(methodInfo, tc.Context.Project.Blueprint(tc.Type));
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
                        var pcb = new ParameterConfigurationBuilder(pi, conf.Context.Project.Blueprint(conf.Type));

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

        
    }
}