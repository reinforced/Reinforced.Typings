namespace Reinforced.Typings.Xmldoc.Model
{
    internal static class DocumentationMemberExtensions
    {
        public static DocumentationMemberType MeberType(this string name)
        {
            if (string.IsNullOrEmpty(name)) return DocumentationMemberType.Unknown;
            if (name.Contains("#ctor")) return DocumentationMemberType.Constructor;
            if (name.StartsWith("M:")) return DocumentationMemberType.Method;
            if (name.StartsWith("T:")) return DocumentationMemberType.Type;
            if (name.StartsWith("P:")) return DocumentationMemberType.Property;
            if (name.StartsWith("E:")) return DocumentationMemberType.Event;
            if (name.StartsWith("N:")) return DocumentationMemberType.Namespace;
            if (name.StartsWith("F:")) return DocumentationMemberType.Field;
            
            return DocumentationMemberType.Unknown;
        }

        public static bool HasSummary(this DocumentationMember dm)
        {
            return dm.Summary != null && !string.IsNullOrEmpty(dm.Summary.Text);
        }

        public static bool HasParameters(this DocumentationMember dm)
        {
            return dm.Parameters != null && dm.Parameters.Length > 0;
        }

        public static bool HasReturns(this DocumentationMember dm)
        {
            return dm.Returns != null && !string.IsNullOrEmpty(dm.Returns.Text);
        }

    }
}