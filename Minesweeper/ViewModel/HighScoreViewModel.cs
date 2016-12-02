using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Minesweeper.Annotations;
using Minesweeper.Properties;

namespace Minesweeper.ViewModel{

    public class HighScoreViewModel : ViewModelBase {
        
        public ObservableCollection<HighScore> HighScores { get; }

        public HighScoreViewModel() {
            HighScores = new ObservableCollection<HighScore>();

            foreach (SettingsProperty sp in Highscores.Default.Properties) {
                var name = sp.Name;
                //var score = int.Parse(Highscores.Default[sp.Name].ToString());
                HighScores.Add(new HighScore(name));
            }
        }

        public ICommand ResetScoresCommand => new RelayCommand(ResetScores);

        private void ResetScores() {
            for (int i = 0; i < HighScores.Count; i++) {
                HighScores[i].Score = 0;
            }

            Properties.Highscores.Default.Save();
        }

        public class HighScore : INotifyPropertyChanged {
            public HighScore(string name) {
                Name = name;
            }

            public string Name { get; }

            public int Score {
                get { return (int) Highscores.Default[Name]; }
                set {
                    if ((int) Highscores.Default[Name] == value) {
                        return;
                    }
                    Highscores.Default[Name] = value;
                    Properties.Highscores.Default.Save();
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}