using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Generators;
using Reinforced.Typings.Samples.Difficult.CodeGenerators.Models;

namespace Reinforced.Typings.Samples.Difficult.CodeGenerators.ReinforcedTypings
{
    public class ClassEnumCodeGenerator : TsCodeGeneratorBase<Type, RtClass>
    {
        public override RtClass GenerateNode(Type element, RtClass node, TypeResolver resolver)
        {
            node = new RtClass() { Name = element.GetName() };
            foreach (var vField in element.GetFields())
            {
                var attr = vField.GetCustomAttribute(typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                if (attr != null)
                {
                    node.Members.Add(new RtRaw(String.Format("{0} = \"{1}\";", vField.Name, attr.Value)));
                }
            }

            return node;
        }
    }

    public class EnumMemberAttribute : Attribute
    {
        public string Value { get; set; }
    }

    public enum MyCoolEnum
    {
        [EnumMember(Value = "Value1")]
        One,
        [EnumMember(Value = "Value1")]
        Two,
        [EnumMember(Value = "Value1")]
        Three
    }
}