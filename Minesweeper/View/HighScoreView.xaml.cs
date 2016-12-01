using System.Windows;

namespace Minesweeper.View
{
    /// <summary>
    /// Description for HighScoreView.
    /// </summary>
    public partial class HighScoreView : Window
    {
        /// <summary>
        /// Initializes a new instance of the HighScoreView class.
        /// </summary>
        public HighScoreView()
        {
            InitializeComponent();
        }

        private void CloseWindow(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}