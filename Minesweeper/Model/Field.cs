using GalaSoft.MvvmLight;

namespace Minesweeper.Model {
    public class Field : ViewModelBase {
        private bool _isRevealed;
        private bool _isMine;
        private bool _isFlagPlaced;
        private bool _isFlagMissPlaced;
        private int _cues;
        public int X { get; set; }
        public int Y { get; set; }

        public bool IsFlagPlaced
        {
            get { return _isFlagPlaced; }
            set
            {
                if (_isFlagPlaced == value) {
                    return;
                }
                
                _isFlagPlaced = value;
                RaisePropertyChanged();
            }
        }

        public bool IsFlagMissPlaced {
            get { return _isFlagMissPlaced; }
            set {
                if (_isFlagMissPlaced == value) {
                    return;
                }

                _isFlagMissPlaced = value;
                RaisePropertyChanged();
            }
        }

        public int Cues {
            get { return _cues; }
            set {
                if (_cues == value) {
                    return;
                }

                _cues = value;
                RaisePropertyChanged();
            }
        }

        public bool IsMine {
            get { return _isMine; }
            set {
                if (_isMine == value) {
                    return;
                }

                _isMine = value; 
                RaisePropertyChanged();
            }
        }

        public bool IsRevealed {
            get { return _isRevealed; }
            set {
                if (_isRevealed == value) {
                    return;
                }

                _isRevealed = value; 
                RaisePropertyChanged();
            }
        }

        public void Set(int x, int y) {
            X = x;
            Y = y;
            IsMine = false;
            Cues = 0;
            IsRevealed = false;
            IsFlagMissPlaced = false;
            IsFlagPlaced = false;
        }
    }
}