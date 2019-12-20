using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Reinforced.Typings.Exceptions;
// ReSharper disable CheckNamespace
// ReSharper disable UnusedTypeParameter
// ReSharper disable PossibleNullReferenceException
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Generic fluent configuration builder
    /// </summary>
    public interface ITypedExportBuilder<T>
    {

    }

    /// <summary>
    /// Fluent export configuration builder for type
    /// </summary>
    public abstract class TypeExportBuilder
    {
        internal TypeBlueprint Blueprint { get; private set; }
        internal TypeExportBuilder(TypeBlueprint blueprint)
        {
            Blueprint = blueprint;
            blueprint.IsExportedExplicitly = true;
        }

        /// <summary>
        /// Gets type that is being configured for export
        /// </summary>
        public Type Type
        {
            get { return Blueprint.Type; }
        }

        protected internal void ApplyMembersConfiguration<T>(IEnumerable<MemberInfo> members, Action<T> configuration = null) where T : MemberExportBuilder
        {
            Blueprint.NotifyFlattenTouched();
            foreach (var member in members)
            {
                var conf = (T)typeof(T).InstanceInternal(Blueprint, member);
                if (configuration == null) continue;
                try
                {
                    configuration(conf);
                }
                catch (Exception ex)
                {
                    ErrorMessages.RTE0006_FluentSingleError.Throw(ex.Message, "property",
                        string.Format("{0}.{1}", member.DeclaringType.FullName, member.Name));
                }
            }
        }


        protected internal void ApplyMethodsConfiguration(IEnumerable<MethodInfo> methds,

            Action<MethodExportBuilder> configuration = null)
        {

            Blueprint.NotifyFlattenTouched();
            foreach (var methodInfo in methds)
            {
                var conf = new MethodExportBuilder(Blueprint, methodInfo);
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
        }

        internal void ExtractParameters(LambdaExpression methodLambda)
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
                        var pcb = new ParameterExportBuilder(Blueprint, pi);

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
                                        var param = Expression.Parameter(typeof(ParameterExportBuilder));
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member