using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Minesweeper.Properties;

namespace Minesweeper.ViewModel{

    public class HighScoreViewModel : ViewModelBase {
        
        public ObservableCollection<Model.HighScoreViewModel.HighScore> HighScores { get; }

        public HighScoreViewModel() {
            HighScores = new ObservableCollection<Model.HighScoreViewModel.HighScore>();

            foreach (SettingsProperty sp in Highscores.Default.Properties) {
                var name = sp.Name;
                HighScores.Add(new Model.HighScoreViewModel.HighScore(name));
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