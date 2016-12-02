using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Minesweeper.Properties;
using Minesweeper.Model;

namespace Minesweeper.ViewModel{

    public class HighScoreViewModel : ViewModelBase {
        
        public ObservableCollection<HighScore> HighScores { get; }

        public HighScoreViewModel() {
            HighScores = new ObservableCollection<HighScore>();

            foreach (SettingsProperty sp in Highscores.Default.Properties) {
                var name = sp.Name;
                HighScores.Add(new HighScore(name));
            }
        }

        public ICommand ResetScoresCommand => new RelayCommand(ResetScores);

        private void ResetScores() {
            foreach (var highScore in HighScores) {
                highScore.Score = 0;
            }

            Highscores.Default.Save();
        }
    }
}