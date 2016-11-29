using System.ComponentModel;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;
using Minesweeper.Annotations;

namespace Minesweeper.Model {
    public class Field : ViewModelBase
    {
        private bool _isRevealed;
        public int X { get; set; }
        public int Y { get; set; }
        public int Cues { get; set; }
        public bool IsMine { get; set; }

        public bool IsRevealed {
            get { return _isRevealed; }
            set {
                _isRevealed = value; 
                RaisePropertyChanged();
            }
        }

        public Field(int x, int y) {
            X = x;
            Y = y;
            IsMine = false;
            Cues = 0;
            _isRevealed = false;
        }

        //public event PropertyChangedEventHandler PropertyChanged;

        //[NotifyPropertyChangedInvocator]
        //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}