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
        /// </summary>
        /// <returns>The product name to display in the UI.</returns>
        private static string RetrieveProductName()
        {
            Assembly assembly =
                Assembly.GetEntryAssembly()
                ?? Assembly.GetExecutingAssembly();

            string? product =
                assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;

            return String.IsNullOrWhiteSpace(product)
                ? assembly.GetName().FullName
                : product;
        }
    }
}