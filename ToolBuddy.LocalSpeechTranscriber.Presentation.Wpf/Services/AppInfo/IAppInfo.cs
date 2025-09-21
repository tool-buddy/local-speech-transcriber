namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.AppInfo
{
    /// <summary>
    /// Provides information about the current application.
    /// </summary>
    public interface IAppInfo
    {
        /// <summary>
        /// Gets the product name.
        /// </summary>
        string ProductName { get; }
    }
}