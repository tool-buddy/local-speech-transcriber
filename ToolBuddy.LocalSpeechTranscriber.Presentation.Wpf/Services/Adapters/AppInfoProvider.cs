using System.Reflection;
using ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.Abstractions;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.Adapters
{
    /// <inheritdoc />
    public sealed class AppInfoProvider : IAppInfoProvider
    {
        /// <inheritdoc />
        public string ProductName { get; } = RetrieveProductName();

        /// <summary>
        /// Retrieves the product name from the entry or executing assembly.
        /// Falls back to a predefined name if not available.
        /// </summary>
        /// <returns>The product name to display in the UI.</returns>
        private static string RetrieveProductName()
        {
            const string fallbackName = "Local Speech Transcriber";

            Assembly assembly =
                Assembly.GetEntryAssembly()
                ?? Assembly.GetExecutingAssembly();

            string? product =
                assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;

            return String.IsNullOrWhiteSpace(product)
                ? fallbackName
                : product;
        }
    }
}