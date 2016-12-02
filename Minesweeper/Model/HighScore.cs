using System.ComponentModel;
using System.Runtime.CompilerServices;
using Minesweeper.Annotations;
using Minesweeper.Properties;

namespace Minesweeper.Model {
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
                Highscores.Default.Save();
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