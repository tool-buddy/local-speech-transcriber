namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.Abstractions
{
    /// <summary>
    /// Provides information about the current application.
    /// </summary>
    public interface IAppInfoProvider
    {
        /// <summary>
        /// Gets the product name.
        /// </summary>
        string ProductName { get; }
    }
}