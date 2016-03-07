using Reinforced.Typings.Exceptions;

namespace Reinforced.Typings.Cli
{
    /// <summary>
    /// Class that formats messages in appropriate way to be shown at 
    /// VisualStudio's errors list
    /// 
    /// Explanation is taken from here:
    /// http://blogs.msdn.com/b/msbuild/archive/2006/11/03/msbuild-visual-studio-aware-error-messages-and-message-formats.aspx
    /// </summary>
    class VisualStudioFriendlyErrorMessage
    {
        public string Origin { get { return "Reinforced.Typings"; } }

        public string Subcategory { get; set; }

        public VisualStudioFriendlyMessageType Type { get; set; }

        public int Code { get; set; }

        public string CodeName { get { return string.Format("RT{0:0000}", Code); } }

        public string ErrorText { get; set; }

        public VisualStudioFriendlyErrorMessage(int code, string errorText, VisualStudioFriendlyMessageType type, string subcategory = "")
        {
            Code = code;
            ErrorText = errorText;
            Subcategory = subcategory;
            Type = type;
        }

        public override string ToString()
        {
            return string.Format("{0} : {1} {2} {3}: {4}", Origin, Subcategory, Type == VisualStudioFriendlyMessageType.Error ? "error" : "warning", CodeName, ErrorText);
        }

        public static VisualStudioFriendlyErrorMessage Create(RtWarning warning)
        {
            return new VisualStudioFriendlyErrorMessage(warning.Code,warning.Text,VisualStudioFriendlyMessageType.Warning,warning.Subcategory);
        }

        public static VisualStudioFriendlyErrorMessage Create(RtException error)
        {
            return new VisualStudioFriendlyErrorMessage(error.Code, error.Message, VisualStudioFriendlyMessageType.Error, error.Subcategory);
        }
    }
}