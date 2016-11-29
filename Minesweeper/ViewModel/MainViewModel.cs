using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace Minesweeper.ViewModel {

    public class MainViewModel : ViewModelBase {
        private int[] _fields;
        private int _height;
        private int _width;
        private int _mines;

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
            _mines = 8;
            _fields = new int[_height * _width];
            PlaceBombs();
            PlaceCues();
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

        private void PlaceCues() {
            for (int i = 0; i < _width; i++) {
                for (int j = 0; j < _height; j++) {

                    // If mine, update adjacent fields with cues
                    if (_fields[i + j*_height] == -1) {
                        for (int k = -1; k < 2; k++) {
                            for (int l = -1; l < 2; l++) {
                                if (k == 0 && l == 0) {
                                    continue;
                                }

                                if (i + k < 0 || i + k >= _width) {
                                    continue;
                                }

                                if (j + l < 0 || j + l >= _height) {
                                    continue;
                                }

                                if (_fields[i + k + (j + l)*_height] == -1) {
                                    continue;
                                }

                                _fields[i + k + (j + l) * _height]++;
                            }
                        }
                    }
                }
            }
        }

        private void PlaceBombs() {
            Random rnd = new Random();
            var fields = new List<int>();

            for (int i = 0; i < _fields.Length; i++) {
                fields.Add(i);
            }

            int bombsPlaced = 0;

            while (bombsPlaced < _mines) {
                var bombField = fields[rnd.Next(fields.Count)];
                fields.Remove(bombField);
                _fields[bombField] = -1;

                bombsPlaced++;
            }
        }
    }
}