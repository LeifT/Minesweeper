using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using Minesweeper.Model;

namespace Minesweeper.ViewModel {
    public class MainViewModel : ViewModelBase {
        private ObservableCollection<Field> _fields;
        private List<Field> _mines;
        private int _height;
        private readonly int _mineCount;
        private int _selectedField;
        private int _width;

        public MainViewModel() {
            _height = 8;
            _width = 8;
            _mineCount = 8;
            _fields = new ObservableCollection<Field>();
            _mines = new List<Field>();
            InitalizeFields();
            PlaceBombs();
            PlaceCues();
        }

        public int Height {
            get { return _height; }
            set {
                if (_height == value) {
                    return;
                }

                _height = value;
                RaisePropertyChanged();
            }
        }

        public int SelectedField {
            get { return _selectedField; }
            set {
                if (_selectedField == value) {
                    return;
                }
                _selectedField = value;
                RevealFields(Fields[_selectedField]);
                RaisePropertyChanged();
            }
        }

        public int Width {
            get { return _width; }
            set {
                if (_width == value) {
                    return;
                }

                _width = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Field> Fields {
            get { return _fields; }
            set {
                _fields = value;
                RaisePropertyChanged();
            }
        }

        private void InitalizeFields() {
            for (var i = 0; i < _width*_height; i++) {
                _fields.Add(new Field(i%_width, i/_height));
            }
        }

        private void PlaceCues() {
            foreach (var mineField in _mines) {
                var mineNeigbours = GetNeightbours(mineField);

                foreach (var mineNeigbour in mineNeigbours)
                {
                    if (!mineNeigbour.IsMine)
                    {
                        mineNeigbour.Cues++;
                    }
                }
            }
        }

        private void RevealFields(Field field) {
            field.IsRevealed = true;

            if (field.Cues > 0) {
                return;
            }

            if (field.IsMine) {
                // Game over
                foreach (var mine in _mines) {
                    mine.IsRevealed = true;
                }
                
                return;
            }

            HashSet<Field> visited = new HashSet<Field>();
            Queue<Field> queue = new Queue<Field>();

            visited.Add(field);
            queue.Enqueue(field);

            while (queue.Count > 0) {
                var current = queue.Dequeue();
                current.IsRevealed = true;

                if (current.Cues != 0) {
                    continue;
                }

                foreach (var neightbour in GetNeightbours(current)) {
                    if (visited.Add(neightbour)) {
                        queue.Enqueue(neightbour);
                    }
                }
            }
        }

        private List<Field> GetNeightbours(Field field) {
            List<Field> neightoubrs = new List<Field>();

            for (var i = -1; i < 2; i++) {
                for (var j = -1; j < 2; j++) {
                    if ((i == 0) && (j == 0)) {
                        continue;
                    }

                    if ((field.X + i < 0) || (field.X + i >= _width)) {
                        continue;
                    }

                    if ((field.Y + j < 0) || (field.Y + j >= _height)) {
                        continue;
                    }

                    neightoubrs.Add(_fields[field.X + i + (field.Y + j) * _height]);
                }
            }

            return neightoubrs;
        }

        private void PlaceBombs() {
            var rnd = new Random();
            var fields = new List<int>();

            for (var i = 0; i < _fields.Count; i++) {
                fields.Add(i);
            }

            var bombsPlaced = 0;

            while (bombsPlaced < _mineCount) {
                var bombField = fields[rnd.Next(fields.Count)];
                fields.Remove(bombField);
                _fields[bombField].IsMine = true;
                _fields[bombField].Cues = -1;
                _mines.Add(_fields[bombField]);
                bombsPlaced++;
            }
        }
    }
}