using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Ast;

namespace Reinforced.Typings.Visitors
{
    public abstract class VisitorBase : IRtVisitor
    {
        public void Visit(RtNode node)
        {
            if (node == null) return;
            if (node is RtField) Visit((RtField)node);
            if (node is RtInterface) Visit((RtInterface)node);
            if (node is RtFuncion) Visit((RtFuncion)node);
            if (node is RtArgument) Visit((RtArgument)node);
            if (node is RtClass) Visit((RtClass)node);
            if (node is RtIdentifier) Visit((RtIdentifier)node);
            if (node is RtDelegateType) Visit((RtDelegateType)node);
            if (node is RtSimpleTypeName) Visit((RtSimpleTypeName)node);
            if (node is RtRaw) Visit((RtRaw)node);
            if (node is RtJsdocNode) Visit((RtJsdocNode)node);
            if (node is RtModule) Visit((RtModule)node);
            if (node is RtEnumValue) Visit((RtEnumValue)node);
            if (node is RtEnum) Visit((RtEnum)node);
            if (node is RtDictionaryType) Visit((RtDictionaryType)node);
            if (node is RtArrayType) Visit((RtArrayType)node);
            if (node is RtConstructor) Visit((RtConstructor)node);
            throw new Exception("Unknown node passed");
        }

        public abstract void Visit(RtField node);
        public abstract void Visit(RtInterface node);
        public abstract void Visit(RtFuncion node);
        public abstract void Visit(RtArgument node);
        public abstract void Visit(RtClass node);
        public abstract void Visit(RtIdentifier node);
        public abstract void Visit(RtDelegateType node);
        public abstract void Visit(RtSimpleTypeName node);
        public abstract void Visit(RtRaw node);
        public abstract void Visit(RtJsdocNode node);
        public abstract void Visit(RtModule node);
        public abstract void Visit(RtEnumValue node);
        public abstract void Visit(RtEnum node);
        public abstract void Visit(RtDictionaryType node);
        public abstract void Visit(RtArrayType node);
        public abstract void Visit(RtConstructor node);
    }
}
