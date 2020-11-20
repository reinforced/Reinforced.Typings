using System;
using System.Globalization;
using System.Reflection;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Xmldoc.Model;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    ///     Default code generator for properties
    /// </summary>
    public class PropertyCodeGenerator : TsCodeGeneratorBase<MemberInfo, RtField>
    {
        private bool HasToBeNullable(TsPropertyAttribute tp, Type propType)
        {
            var hasNullable = tp != null && tp.NilForceNullable.HasValue;
            if (hasNullable)
            {
                return tp.NilForceNullable.Value;
            }

            if (Context.Global.AutoOptionalProperties)
            {
                return propType.IsNullable();
            }

            return false;
        }

        /// <summary>
        ///     Main code generator method. This method should write corresponding TypeScript code for element (1st argument) to
        ///     WriterWrapper (3rd argument) using TypeResolver if necessary
        /// </summary>
        /// <param name="element">Element code to be generated to output</param>
        /// <param name="result">Resulting node</param>
        /// <param name="resolver">Type resolver</param>
        public override RtField GenerateNode(MemberInfo element, RtField result, TypeResolver resolver)
        {
            if (Context.CurrentBlueprint.IsIgnored(element)) return null;
            result.IsStatic = element.IsStatic();
            result.Order = Context.CurrentBlueprint.GetOrder(element);

            var doc = Context.Documentation.GetDocumentationMember(element);
            if (doc != null)
            {
                RtJsdocNode jsdoc = new RtJsdocNode();
                if (doc.HasInheritDoc()) jsdoc.AddTag(DocTag.Inheritdoc);
                if (doc.HasSummary()) jsdoc.Description = doc.Summary.Text;
                result.Documentation = jsdoc;
            }

            var t = GetType(element);
            RtTypeName type = null;
            var propName = new RtIdentifier(element.Name);
            bool isNameOverridden = false;
            var tp = Context.CurrentBlueprint.ForMember<TsPropertyAttribute>(element);

            if (tp != null)
            {
                if (tp.StrongType != null)
                {
                    type = resolver.ResolveTypeName(tp.StrongType);
                }
                else if (!string.IsNullOrEmpty(tp.Type))
                {
                    type = new RtSimpleTypeName(tp.Type);
                }

                type = tp.TypeInferers.Infer(element, resolver) ?? type;

                if (!string.IsNullOrEmpty(tp.Name))
                {
                    propName.IdentifierName = tp.Name;
                    isNameOverridden = true;
                }
            }
            
            if (!Context.SpecialCase)
            {
                propName.IsNullable = HasToBeNullable(tp, t) 
                                      || Context.Global.AutoOptionalProperties && element.IsReferenceForcedNullable();
            }

            if (type == null) type = resolver.ResolveTypeName(t);
            
            if (!isNameOverridden)
            {
                if (element is PropertyInfo)
                {
                    propName.IdentifierName =
                        Context.ConditionallyConvertPropertyNameToCamelCase(propName.IdentifierName);
                }
                propName.IdentifierName = Context.CurrentBlueprint.CamelCaseFromAttribute(element, propName.IdentifierName);
                propName.IdentifierName = Context.CurrentBlueprint.PascalCaseFromAttribute(element, propName.IdentifierName);
            }

            if (this.Context.Location.CurrentClass != null)
            {
                this.FillInitialization(element, result, resolver, t, tp);
            }
            result.Identifier = propName;
            result.AccessModifier = Context.SpecialCase ? AccessModifier.Public : element.GetModifier();
            result.Type = type;
            AddDecorators(result, Context.CurrentBlueprint.DecoratorsFor(element));
            return result;
        }
        private static readonly IFormatProvider JsNumberFormat = new NumberFormatInfo() { NumberDecimalSeparator = "." };
        /// <summary>
        /// Fills in initialization expression
        /// </summary>
        /// <param name="element">Class member</param>
        /// <param name="result">Resulting AST</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="memberType">Field/property type</param>
        /// <param name="attr">Attribute</param>
        protected virtual void FillInitialization(MemberInfo element, RtField result, TypeResolver resolver, Type memberType, TsPropertyAttribute attr)
        {
            bool exportConstant = true;
            if (attr != null) exportConstant = attr.Constant;
            if (element.IsStatic() && exportConstant)
            {
                if (TypeResolver.NumericTypes.Contains(memberType))
                {
                    var val = GetStaticValue(element);
                    if (val == null) result.InitializationExpression = "null";
                    else if (TypeResolver.IntegerTypes.Contains(memberType))
                    {
                        result.InitializationExpression = val.ToString();
                    }
                    else
                    {
                        double dVal = (double)val;
                        result.InitializationExpression = dVal.ToString(JsNumberFormat);
                    }
                }

                if (memberType == typeof(bool))
                {
                    var val = GetStaticValue(element);
                    if (val == null) result.InitializationExpression = "null";
                    else result.InitializationExpression = (bool)val ? "true" : "false";
                }

                if (memberType == typeof(string))
                {
                    var val = GetStaticValue(element);
                    if (val == null) result.InitializationExpression = "null";
                    else
                    {
                        var sv = string.Format("`{0}`", val.ToString().Replace("'", "\\'"));
                        result.InitializationExpression = sv;
                    }
                }

                if (memberType._IsEnum())
                {
                    var val = GetStaticValue(element);
                    if (val == null) result.InitializationExpression = "null";
                    else
                    {
                        var bp = Context.Project.Blueprint(memberType, false);
                        if (bp != null)
                        {
                            var tn = resolver.ResolveTypeName(memberType);
                            var name = Enum.GetName(memberType, val);
                            result.InitializationExpression = string.Format("{0}.{1}", tn, name);
                        }
                        else
                        {

                            var v = (int)val;
                            result.InitializationExpression = v.ToString();
                        }
                    }
                }
            }

            if (attr?.InitializerEvaluator != null)
            {
                var val = element.IsStatic() ? GetStaticValue(element) : null;
                result.InitializationExpression = attr.InitializerEvaluator(element, resolver, val);
            }
        }

        /// <summary>
        /// Retrieves static value 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected virtual object GetStaticValue(MemberInfo element)
        {
            var pi = element as PropertyInfo;
            return (pi.GetValue(null) ?? pi.GetConstantValue()) ?? pi.GetRawConstantValue();
        }

        /// <summary>
        ///     Returns type of specified property. It is useful for overloads sometimes
        /// </summary>
        /// <param name="mi">Method Info</param>
        /// <returns>Property info type</returns>
        protected virtual Type GetType(MemberInfo mi)
        {
            var pi = (PropertyInfo)mi;
            return pi.PropertyType;
        }
    }
}