using Reinforced.Typings.Ast.Dependency;
using Reinforced.Typings.Ast.TypeNames;

#pragma warning disable 1591
namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// Base interface for visitor traversing simple TypeScript AST tree
    /// </summary>
    /// <typeparam name="T">Node traverse result</typeparam>
    public interface IRtVisitor<out T>
    {
        T Visit(RtNode node);
        T Visit(RtField node);
        T Visit(RtInterface node);
        T Visit(RtFunction node);
        T Visit(RtArgument node);
        T Visit(RtClass node);
        T Visit(RtIdentifier node);
        T Visit(RtDelegateType node);
        T Visit(RtSimpleTypeName node);
        T Visit(RtRaw node);
        T Visit(RtJsdocNode node);
        T Visit(RtNamespace node);
        T Visit(RtEnumValue node);
        T Visit(RtEnum node);
        T Visit(RtDictionaryType node);
        T Visit(RtArrayType node);
        T Visit(RtConstructor node);
        T Visit(RtImport node);
        T Visit(RtDecorator node);
        T Visit(RtReference node);
        T Visit(RtTuple node);
    }

    /// <summary>
    /// Base interface for void visitor traversing simple TypeScript AST tree
    /// </summary>
    public interface IRtVisitor
    {
        void Visit(RtNode node);
        void Visit(RtField node);
        void Visit(RtInterface node);
        void Visit(RtFunction node);
        void Visit(RtArgument node);
        void Visit(RtClass node);
        void Visit(RtIdentifier node);
        void Visit(RtDelegateType node);
        void Visit(RtSimpleTypeName node);
        void Visit(RtRaw node);
        void Visit(RtJsdocNode node);
        void Visit(RtNamespace node);
        void Visit(RtEnumValue node);
        void Visit(RtEnum node);
        void Visit(RtDictionaryType node);
        void Visit(RtArrayType node);
        void Visit(RtConstructor node);
        void Visit(RtImport node);
        void Visit(RtDecorator node);
        void Visit(RtReference node);
        void Visit(RtTuple node);
    }
}
