using System.Windows.Threading;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.Threading
{
    /// <summary>
    /// WPF-specific implementation of <see cref="IMainThreadDispatcher"/> that uses
    /// <see cref="Dispatcher.BeginInvoke(Delegate, object[])"/> to marshal work to the UI thread.
    /// </summary>
    public sealed class WpfMainThreadDispatcher : IMainThreadDispatcher
    {
        private readonly Dispatcher _dispatcher = System.Windows.Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

        /// <summary>
        /// Posts the specified action to the WPF <see cref="Dispatcher"/> at <see cref="DispatcherPriority.DataBind"/>.
        /// </summary>
        /// <param name="action">The action to execute on the UI thread.</param>
        public void Post(
            Action action) =>
            _dispatcher.BeginInvoke(
                DispatcherPriority.DataBind,
                action
            );
    }
}