using System.Windows;
using System.Windows.Threading;

namespace ToolBuddy.LocalSpeechTranscriber.Services.Threading
{
    public sealed class WpfMainThreadDispatcher : IMainThreadDispatcher
    {
        private readonly Dispatcher _dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

        public void Post(Action action) =>
            _dispatcher.BeginInvoke(DispatcherPriority.DataBind, action);
    }
}
