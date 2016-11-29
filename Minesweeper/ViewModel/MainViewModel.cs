using GalaSoft.MvvmLight;

namespace Minesweeper.ViewModel {

    public class MainViewModel : ViewModelBase {
        private int[] _fields;
        private int _height;
        private int _width;

        public int Height {
            get { return _height;}
            set {
                if (_height == value) {
                    return;
                }

                _height = value;
                RaisePropertyChanged();
            }
        }

        public int Width
        {
            get { return _width; }
            set
            {
                if (_width == value)
                {
                    return;
                }

                _width = value;
                RaisePropertyChanged();
            }
        }

        public MainViewModel() {
            _height = 8;
            _width = 8;
            _fields = new int[_height * _width];
        }

        public int[] Fields {
            get { return _fields; }

            set {
                _fields = value;
                RaisePropertyChanged();
            }
        }

        private void InitalizeFields() {
        }
    }
}