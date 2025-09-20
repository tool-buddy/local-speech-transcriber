namespace ToolBuddy.LocalSpeechTranscriber.Application.Contracts
{
    /// <summary>
    /// Provides discovery of a suitable Python executable on the host machine.
    /// </summary>
    public interface IPythonLocator
    {
        /// <summary>
        /// Attempts to locate the absolute path to a Python interpreter.
        /// </summary>
        /// <param name="path">When this method returns, contains the full path to the Python executable if found; otherwise, null.</param>
        /// <returns>True if a Python executable was found; otherwise, false.</returns>
        bool TryGetPythonPath(
            out string? path);
    }
}