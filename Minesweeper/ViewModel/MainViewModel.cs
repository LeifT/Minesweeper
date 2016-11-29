using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace Minesweeper.ViewModel {

    public class MainViewModel : ViewModelBase {
        private int[] _fields;
        private int _height;
        private int _width;
        private int _bombs;

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
            _bombs = 8;
            _fields = new int[_height * _width];
            PlaceBombs();
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

        private void PlaceBombs() {
            Random rnd = new Random();
            var fields = new List<int>();

            for (int i = 0; i < _fields.Length; i++) {
                fields.Add(i);
            }

            int bombsPlaced = 0;

            while (bombsPlaced < _bombs) {
                var bombField = fields[rnd.Next(fields.Count)];
                fields.Remove(bombField);
                _fields[bombField] = -1;

                bombsPlaced++;
            }
        }
    }
}