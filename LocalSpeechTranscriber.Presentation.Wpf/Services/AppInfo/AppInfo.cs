using System.Reflection;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.AppInfo
{
    public sealed class AppInfo : IAppInfo
    {
        public string ProductName { get; } = RetrieveProductName();

        private static string RetrieveProductName()
        {
            const string fallbackName = "Local Speech Transcriber";

            Assembly assembly =
                Assembly.GetEntryAssembly()
                ?? Assembly.GetExecutingAssembly();

            string? product =
                assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;

            return string.IsNullOrWhiteSpace(product)
                ? fallbackName
                : product;
        }
    }
}