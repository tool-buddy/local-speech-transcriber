using System.Windows;
using ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.ViewModels;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf
{
    /// <summary>
    /// Main application window that binds to <see cref="MainViewModel"/>.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class and sets the view model as the data context.
        /// </summary>
        /// <param name="viewModel">The main view model providing UI state and commands.</param>
        public MainWindow(
            MainViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}