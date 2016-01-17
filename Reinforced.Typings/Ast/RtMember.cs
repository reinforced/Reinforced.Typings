namespace Reinforced.Typings.Ast
{
    public abstract class RtMember : RtNode
    {
        public RtJsdocNode Documentation { get; set; }

        public AccessModifier? AccessModifier { get; set; }

        public bool IsStatic { get; set; }
    }
}
