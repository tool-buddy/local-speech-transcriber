using System.Windows;
using ToolBuddy.LocalSpeechTranscriber.ViewModels;

namespace ToolBuddy.LocalSpeechTranscriber
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