using System.Windows.Threading;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.Threading
{
    public sealed class WpfMainThreadDispatcher : IMainThreadDispatcher
    {
        private readonly Dispatcher _dispatcher = System.Windows.Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

        public void Post(
            Action action) =>
            _dispatcher.BeginInvoke(
                DispatcherPriority.DataBind,
                action
            );
    }
}