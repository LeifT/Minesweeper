using GalaSoft.MvvmLight;

namespace Minesweeper.Model {
    public class Field : ViewModelBase {
        private bool _isRevealed;
        private bool _isMine;
        private bool _flagPlaced;
        private int _cues;
        public int X { get; set; }
        public int Y { get; set; }

        public bool FlagPlaced
        {
            get { return _flagPlaced; }
            set
            {
                if (_flagPlaced == value) {
                    return;
                }
                
                _flagPlaced = value;
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



        public Field(int x, int y) {
            X = x;
            Y = y;
            _isMine = false;
            _cues = 0;
            _isRevealed = false;
        }
    }
}