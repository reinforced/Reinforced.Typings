using System;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Ast.Dependency;
using Reinforced.Typings.Ast.TypeNames;
#pragma warning disable 1591
namespace Reinforced.Typings.Visitors
{
    public abstract class VisitorBase : IRtVisitor
    {
        public void Visit(RtNode node)
        {
            if (node == null) return;
            if (node is RtField) { Visit((RtField)node); return; }
            if (node is RtInterface) { Visit((RtInterface)node); return; }
            if (node is RtFuncion) { Visit((RtFuncion)node); return; }
            if (node is RtArgument) { Visit((RtArgument)node); return; }
            if (node is RtClass) { Visit((RtClass)node); return; }
            if (node is RtIdentifier) { Visit((RtIdentifier)node); return; }
            if (node is RtDelegateType) { Visit((RtDelegateType)node); return; }
            if (node is RtSimpleTypeName) { Visit((RtSimpleTypeName)node); return; }
            if (node is RtRaw) { Visit((RtRaw)node); return; }
            if (node is RtJsdocNode) { Visit((RtJsdocNode)node); return; }
            if (node is RtNamespace) { Visit((RtNamespace)node); return; }
            if (node is RtEnumValue) { Visit((RtEnumValue)node); return; }
            if (node is RtEnum) { Visit((RtEnum)node); return; }
            if (node is RtDictionaryType) { Visit((RtDictionaryType)node); return; }
            if (node is RtArrayType) { Visit((RtArrayType)node); return; }
            if (node is RtConstructor) { Visit((RtConstructor)node); return; }
            if (node is RtImport) { Visit((RtImport)node); return; }
            if (node is RtDecorator) { Visit((RtDecorator)node); return; }
            if (node is RtReference) { Visit((RtReference)node); return; }
            if (node is RtTuple) { Visit((RtTuple)node); return; }
            if (node is RtContainer) { Visit((RtContainer)node); return; }

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
        public abstract void Visit(RtNamespace node);
        public abstract void Visit(RtEnumValue node);
        public abstract void Visit(RtEnum node);
        public abstract void Visit(RtDictionaryType node);
        public abstract void Visit(RtArrayType node);
        public abstract void Visit(RtConstructor node);
        public abstract void Visit(RtImport node);
        public abstract void Visit(RtDecorator node);
        public abstract void Visit(RtReference node);
        public abstract void Visit(RtTuple node);
        public abstract void Visit(RtContainer node);
        public abstract void VisitFile(ExportedFile file);
    }
}
