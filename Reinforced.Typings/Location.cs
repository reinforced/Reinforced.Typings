using System;
using System.Collections.Generic;
using Reinforced.Typings.Ast;

namespace Reinforced.Typings
{
    /// <summary>
    /// Identifies where current export is performed in terms of AST. 
    /// Context.Location could be used to conditionally add members to different places of generated source code
    /// </summary>
    public class Location
    {
        private readonly ExportContext _exContext;
        /// <summary>
        /// Current Class 
        /// </summary>
        public RtClass CurrentClass { get; private set; }

        /// <summary>
        /// Current Interface
        /// </summary>
        public RtInterface CurrentInterface { get; private set; }

        /// <summary>
        /// Current Enum
        /// </summary>
        public RtEnum CurrentEnum { get; private set; }

        /// <summary>
        /// Current Module
        /// </summary>
        public RtNamespace CurrentNamespace { get; private set; }

        /// <summary>
        /// References currently exported type
        /// </summary>
        public Type CurrentType
        {
            get
            {
                if (_typesStack.Count == 0) return null;
                return _typesStack.Peek().Type;
            }
        }

        /// <summary>
        /// Sets current location
        /// </summary>
        /// <param name="location"></param>
        public void SetLocation(RtNode location)
        {
            if (location is RtClass) CurrentClass = (RtClass)location;
            if (location is RtInterface) CurrentInterface = (RtInterface)location;
            if (location is RtEnum) CurrentEnum = (RtEnum)location;
            if (location is RtNamespace) CurrentNamespace = (RtNamespace)location;
        }

        /// <summary>
        /// Sets current location
        /// </summary>
        /// <param name="location"></param>
        public void ResetLocation(RtNode location)
        {
            if (location is RtClass) CurrentClass = null;
            if (location is RtInterface) CurrentInterface = null;
            if (location is RtEnum) CurrentEnum = null;
            if (location is RtNamespace) CurrentNamespace = null;
        }

        internal Stack<TypeBlueprint> _typesStack = new Stack<TypeBlueprint>();

        internal Location(ExportContext exContext)
        {
            _exContext = exContext;
        }

        /// <summary>
        /// Sets currently exported type
        /// </summary>
        /// <param name="t"></param>
        public void SetCurrentType(Type t)
        {
            _typesStack.Push(_exContext.Project.Blueprint(t));
        }

        /// <summary>
        /// Resets currently exported type
        /// </summary>
        public void ResetCurrentType()
        {
            if (_typesStack.Count > 0) _typesStack.Pop();
        }
    }
}
