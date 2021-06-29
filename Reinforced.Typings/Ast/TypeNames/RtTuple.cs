using System.Collections.Generic;

namespace Reinforced.Typings.Ast.TypeNames
{
    /// <summary>
    /// AST node for TypeScript tuple type
    /// </summary>
    public class RtTuple : RtTypeName
    {
        /// <summary>
        /// Constructs new RtTuple
        /// </summary>
        public RtTuple()
        {
            TupleTypes = new List<RtTypeName>();
        }

        /// <summary>
        /// Constructs new RtTuple with specified type paranmeters
        /// </summary>
        /// <param name="tupleTypes">Types for tuple</param>
        public RtTuple(IEnumerable<RtTypeName> tupleTypes)
        {
            TupleTypes = new List<RtTypeName>(tupleTypes);
        }

        /// <summary>
        /// Constructs new RtTuple with specified type paranmeters
        /// </summary>
        /// <param name="tupleTypes">Types for tuple</param>
        public RtTuple(params RtTypeName[] tupleTypes)
        {
            TupleTypes = new List<RtTypeName>(tupleTypes);
        }

        /// <summary>
        /// All types that must participate tuple
        /// </summary>
        public List<RtTypeName> TupleTypes { get; private set; }

        /// <inheritdoc />
        public override IEnumerable<RtNode> Children
        {
            get
            {
                foreach (var rtTypeName in TupleTypes)
                {
                    yield return rtTypeName;
                }
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
            return $"[{string.Join(", ", TupleTypes)}]";
        }

        protected bool Equals(RtTuple other)
        {
            return Equals(TupleTypes, other.TupleTypes);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RtTuple) obj);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return (TupleTypes != null ? TupleTypes.GetHashCode() : 0);
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Reinforced.Typings.Ast.TypeNames.RtTuple" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(RtTuple left, RtTuple right)
        {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Reinforced.Typings.Ast.TypeNames.RtTuple" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(RtTuple left, RtTuple right)
        {
            return !Equals(left, right);
        }
    }
}
