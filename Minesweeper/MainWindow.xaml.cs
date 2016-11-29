using System.Windows;
using Minesweeper.ViewModel;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        private void Beginner(object sender, RoutedEventArgs e) {
            MinHeight = 360;
            MinWidth = 280;
        }

        private void Intermediate(object sender, RoutedEventArgs e) {
            MinHeight = 616;
            MinWidth = 536;
        }

        private void Expert(object sender, RoutedEventArgs e) {
            MinHeight = 616;
            MinWidth = 984;
        }
    }
}