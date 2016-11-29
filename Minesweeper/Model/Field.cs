using System.ComponentModel;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;
using Minesweeper.Annotations;

namespace Minesweeper.Model {
    public class Field : ViewModelBase
    {
        private bool _isRevealed;
        private bool _isMine;
        private int _cues;
        public int X { get; set; }
        public int Y { get; set; }

        public int Cues {
            get { return _cues; }
            set {
                _cues = value;
                RaisePropertyChanged();
            }
        }

        public bool IsMine {
            get { return _isMine; }
            set {
                _isMine = value; 
                RaisePropertyChanged();
            }
        }

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