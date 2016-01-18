using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Ast;

namespace Reinforced.Typings
{
    /// <summary>
    /// Identifies where current export is performed in terms of AST. 
    /// Context.Location could be used to conditionally add members to different places of generated source code
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Current Class 
        /// </summary>
        public RtClass CurrentClass { get; set; }

        /// <summary>
        /// Current Interface
        /// </summary>
        public RtInterface CurrentInterface { get; set; }

        /// <summary>
        /// Current Enum
        /// </summary>
        public RtEnum CurrentEnum { get; set; }

        /// <summary>
        /// Current Module
        /// </summary>
        public RtModule CurrentModule { get; set; }

        /// <summary>
        /// Sets current location
        /// </summary>
        /// <param name="location"></param>
        public void SetLocation(RtNode location)
        {
            if (location is RtClass) CurrentClass = (RtClass) location;
            if (location is RtInterface) CurrentInterface = (RtInterface)location;
            if (location is RtEnum) CurrentEnum = (RtEnum)location;
            if (location is RtModule) CurrentModule = (RtModule)location;
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
            if (location is RtModule) CurrentModule = null;
        }
    }
}
