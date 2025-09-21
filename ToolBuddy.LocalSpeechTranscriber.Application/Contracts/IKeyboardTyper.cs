namespace ToolBuddy.LocalSpeechTranscriber.Application.Contracts
{
    /// <summary>
    /// Sends text to the active input control by simulating keyboard input.
    /// </summary>
    public interface IKeyboardTyper
    {
        /// <summary>
        /// Types the specified text into the active window/control.
        /// </summary>
        /// <param name="text">The text to type as simulated keyboard input.</param>
        void TypeText(
            string text);
    }
}