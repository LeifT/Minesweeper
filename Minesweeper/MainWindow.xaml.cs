using System.Windows;
using Minesweeper.View;
using Minesweeper.ViewModel;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow() {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        private void OpenHighScoreWindow(object sender, RoutedEventArgs e) {
            var highScoreView = new HighScoreView {
                Owner = Application.Current.MainWindow,
                ShowInTaskbar = false
            };

            highScoreView.ShowDialog();
        }
    }
}