using System.Reflection;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// Describes all possible JSDOC tags
    /// </summary>
    public enum DocTag
    {
        /// <summary> This member must be implemented (or overridden) by the inheritor. </summary> 
        [JsdocTag("@abstract")]
        Abstract,
        /// <summary> Specify the access level of this member (private, public, or protected). </summary> 
        [JsdocTag("@access")]
        Access,
        /// <summary> Treat a member as if it had a different name. </summary> 
        [JsdocTag("@alias")]
        Alias,
        /// <summary> Indicate that a symbol inherits from, ands adds to, a parent symbol. </summary> 
        [JsdocTag("@augments")]
        Augments,
        /// <summary> Identify the author of an item. </summary> 
        [JsdocTag("@author")]
        Author,
        /// <summary> This object uses something from another object. </summary> 
        [JsdocTag("@borrows")]
        Borrows,
        /// <summary> Document a callback function. </summary> 
        [JsdocTag("@callback")]
        Callback,
        /// <summary> This function is intended to be called with the "new" keyword. </summary> 
        [JsdocTag("@class")]
        Class,
        /// <summary> Use the following text to describe the entire class. </summary> 
        [JsdocTag("@classdesc")]
        Classdesc,
        /// <summary> Document an object as a constant. </summary> 
        [JsdocTag("@constant")]
        Constant,
        /// <summary> This function member will be the constructor for the previous class. </summary> 
        [JsdocTag("@constructs")]
        Constructs,
        /// <summary> Document some copyright information. </summary> 
        [JsdocTag("@copyright")]
        Copyright,
        /// <summary> Document the default value. </summary> 
        [JsdocTag("@default")]
        Default,
        /// <summary> Document that this is no longer the preferred way. </summary> 
        [JsdocTag("@deprecated")]
        Deprecated,
        /// <summary> Describe a symbol. </summary> 
        [JsdocTag("@description")]
        Description,
        /// <summary> Document a collection of related properties. </summary> 
        [JsdocTag("@enum")]
        Enum,
        /// <summary> Document an event. </summary> 
        [JsdocTag("@event")]
        Event,
        /// <summary> Provide an example of how to use a documented item. </summary> 
        [JsdocTag("@example")]
        Example,
        /// <summary> Identify the member that is exported by a JavaScript module. </summary> 
        [JsdocTag("@exports")]
        Exports,
        /// <summary> Identifies an external class, namespace, or module. </summary> 
        [JsdocTag("@external")]
        External,
        /// <summary> Describe a file. </summary> 
        [JsdocTag("@file")]
        File,
        /// <summary> Describe the events this method may fire. </summary> 
        [JsdocTag("@fires")]
        Fires,
        /// <summary> Describe a function or method. </summary> 
        [JsdocTag("@function")]
        Function,
        /// <summary> Document a global object. </summary> 
        [JsdocTag("@global")]
        Global,
        /// <summary> Omit a symbol from the documentation. </summary> 
        [JsdocTag("@ignore")]
        Ignore,
        /// <summary> This symbol implements an interface. </summary> 
        [JsdocTag("@implements")]
        Implements,
        /// <summary> Indicate that a symbol should inherit its parent's documentation. </summary> 
        [JsdocTag("@inheritdoc")]
        Inheritdoc,
        /// <summary> Document an inner object. </summary> 
        [JsdocTag("@inner")]
        Inner,
        /// <summary> Document an instance member. </summary> 
        [JsdocTag("@instance")]
        Instance,
        /// <summary> This symbol is an interface that others can implement. </summary> 
        [JsdocTag("@interface")]
        Interface,
        /// <summary> What kind of symbol is this? </summary> 
        [JsdocTag("@kind")]
        Kind,
        /// <summary> Document properties on an object literal as if they belonged to a symbol with a given name. </summary> 
        [JsdocTag("@lends")]
        Lends,
        /// <summary> Identify the license that applies to this code. </summary> 
        [JsdocTag("@license")]
        License,
        /// <summary> List the events that a symbol listens for. </summary> 
        [JsdocTag("@listens")]
        Listens,
        /// <summary> Document a member. </summary> 
        [JsdocTag("@member")]
        Member,
        /// <summary> This symbol belongs to a parent symbol. </summary> 
        [JsdocTag("@memberof")]
        Memberof,
        /// <summary> This object mixes in all the members from another object. </summary> 
        [JsdocTag("@mixes")]
        Mixes,
        /// <summary> Document a mixin object. </summary> 
        [JsdocTag("@mixin")]
        Mixin,
        /// <summary> Document a JavaScript module. </summary> 
        [JsdocTag("@module")]
        Module,
        /// <summary> Document the name of an object. </summary> 
        [JsdocTag("@name")]
        Name,
        /// <summary> Document a namespace object. </summary> 
        [JsdocTag("@namespace")]
        Namespace,
        /// <summary> Indicate that a symbol overrides its parent. </summary> 
        [JsdocTag("@override")]
        Override,
        /// <summary> Document the parameter to a function. </summary> 
        [JsdocTag("@param")]
        Param,
        /// <summary> This symbol is meant to be private. </summary> 
        [JsdocTag("@private")]
        Private,
        /// <summary> Document a property of an object. </summary> 
        [JsdocTag("@property")]
        Property,
        /// <summary> This symbol is meant to be protected. </summary> 
        [JsdocTag("@protected")]
        Protected,
        /// <summary> This symbol is meant to be public. </summary> 
        [JsdocTag("@public")]
        Public,
        /// <summary> This symbol is meant to be read-only. </summary> 
        [JsdocTag("@readonly")]
        Readonly,
        /// <summary> This file requires a JavaScript module. </summary> 
        [JsdocTag("@requires")]
        Requires,
        /// <summary> Document the return value of a function. </summary> 
        [JsdocTag("@returns")]
        Returns,
        /// <summary> Refer to some other documentation for more information. </summary> 
        [JsdocTag("@see")]
        See,
        /// <summary> When was this feature added? </summary> 
        [JsdocTag("@since")]
        Since,
        /// <summary> Document a static member. </summary> 
        [JsdocTag("@static")]
        Static,
        /// <summary> A shorter version of the full description. </summary> 
        [JsdocTag("@summary")]
        Summary,
        /// <summary> What does the 'this' keyword refer to here? </summary> 
        [JsdocTag("@this")]
        This,
        /// <summary> Describe what errors could be thrown. </summary> 
        [JsdocTag("@throws")]
        Throws,
        /// <summary> Document tasks to be completed. </summary> 
        [JsdocTag("@todo")]
        Todo,
        /// <summary> Insert a link to an included tutorial file. </summary> 
        [JsdocTag("@tutorial")]
        Tutorial,
        /// <summary> Document the type of an object. </summary> 
        [JsdocTag("@type")]
        Type,
        /// <summary> Document a custom type. </summary> 
        [JsdocTag("@typedef")]
        Typedef,
        /// <summary> Distinguish different objects with the same name. </summary> 
        [JsdocTag("@variation")]
        Variation,
        /// <summary>
        /// Documents the version number of an item
        /// </summary>
        [JsdocTag("@version")]
        Version,

    }

    public static class DocTagExtensions
    {
        public static string Tagname(this DocTag tag)
        {
            var member = typeof (DocTag).GetField(tag.ToString());
            return member.GetCustomAttribute<JsdocTagAttribute>().RawTagName;
        }
    }
}
