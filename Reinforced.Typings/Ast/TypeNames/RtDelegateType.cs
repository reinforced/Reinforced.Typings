using System;
using System.Collections.Generic;
using System.Linq;

namespace Reinforced.Typings.Ast.TypeNames
{
    /// <summary>
    /// AST node for delegate type
    /// </summary>
    public sealed class RtDelegateType : RtTypeName
    {
        private readonly RtArgument[] _arguments;

        /// <summary>
        /// Consumed arguments
        /// </summary>
        public RtArgument[] Arguments
        {
            get { return _arguments; }
        }

        /// <summary>
        /// Returning result
        /// </summary>
        public RtTypeName Result { get; private set; }

        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        /// <param name="arguments">Delegate parameters</param>
        /// <param name="result">Delegate result type</param>
        public RtDelegateType(RtArgument[] arguments, RtTypeName result)
        {
            _arguments = arguments;
            Result = result;
        }

        /// <inheritdoc />
        public override IEnumerable<RtNode> Children
        {
            get
            {
                foreach (var rtArgument in Arguments)
                {
                    yield return rtArgument;
                }
                yield return Result;
            }
        }

        /// <inheritdoc />
        public override void Accept(IRtVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override void Accept<T>(IRtVisitor<T> visitor)
        {
            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return String.Format("({0}) => {1}", String.Join(", ", Arguments.AsEnumerable()), Result);
        }

        private bool Equals(RtDelegateType other)
        {
            return Equals(_arguments, other._arguments) && Equals(Result, other.Result);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is RtDelegateType other && Equals(other);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((_arguments != null ? _arguments.GetHashCode() : 0) * 397) ^ (Result != null ? Result.GetHashCode() : 0);
            }
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Reinforced.Typings.Ast.TypeNames.RtDelegateType" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(RtDelegateType left, RtDelegateType right)
        {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Reinforced.Typings.Ast.TypeNames.RtDelegateType" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(RtDelegateType left, RtDelegateType right)
        {
            return !Equals(left, right);
        }
    }
}
