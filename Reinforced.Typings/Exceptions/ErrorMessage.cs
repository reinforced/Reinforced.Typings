namespace Reinforced.Typings.Exceptions
{
    /// <summary>
    /// Data object for RT error message
    /// </summary>
    public class ErrorMessage
    {
        /// <summary>
        /// Error code
        /// </summary>
        public int Code { get; private set; }

        /// <summary>
        /// Message test
        /// </summary>
        public string MessageText { get; private set; }

        /// <summary>
        /// Error message subcategory (for Visual Studio)
        /// </summary>
        public string Subcategory { get; private set; }

        public ErrorMessage(int code, string messageText, string subcategory = "")
        {
            Code = code;
            MessageText = messageText;
            Subcategory = subcategory;
        }

        /// <summary>
        /// Throws error message as exception
        /// </summary>
        /// <param name="formatParameters">Format arguments</param>
        /// <exception cref="RtException">RtException corresponding to error message will be thrown in all cases</exception>
        public void Throw(params object[] formatParameters)
        {
            throw new RtException(string.Format(MessageText, formatParameters), Code, Subcategory);
        }

        /// <summary>
        /// Converts error message to RtWarning to be processed further
        /// </summary>
        /// <param name="formatParameters"></param>
        /// <returns></returns>
        public RtWarning Warn(params object[] formatParameters)
        {
            return new RtWarning(Code, text: string.Format(MessageText, formatParameters), subcategory: Subcategory);
        }
    }
}
