using System.Windows;

namespace Minesweeper.View {

    public partial class HighScoreView : Window {

        public HighScoreView() {
            InitializeComponent();
        }

        private void CloseWindow(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}