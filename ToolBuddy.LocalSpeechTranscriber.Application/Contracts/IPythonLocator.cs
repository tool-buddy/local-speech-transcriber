namespace ToolBuddy.LocalSpeechTranscriber.Application.Contracts
{
    public interface IPythonLocator
    {
        bool TryGetPythonPath(
            out string? path);
    }
}