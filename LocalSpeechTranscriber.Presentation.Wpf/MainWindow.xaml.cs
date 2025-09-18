using System.Windows;
using ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.ViewModels;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow(
            MainViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}