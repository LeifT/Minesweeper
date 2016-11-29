using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Minesweeper.Model;

namespace Minesweeper.ViewModel {
    public class MainViewModel : ViewModelBase {
        private readonly int _mineCount;
        private readonly List<Field> _mines;
        private ObservableCollection<Field> _fields;
        private int _fieldsRevealed;
        private bool _firstReveal;
        private int _height;
        private int _width;

        public ICommand RestartCommand => new RelayCommand(Restart);
        public ICommand RevealCommand => new RelayCommand<Field>(Reveal);
        public ICommand PlaceFlagCommand => new RelayCommand<Field>(PlaceFlag);

        public MainViewModel() {
            _height = 8;
            _width = 8;
            _mineCount = 8;
            _fields = new ObservableCollection<Field>();
            _mines = new List<Field>();

            Restart();
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

        private void Restart() {
            InitalizeFields();
            _firstReveal = false;
            _fieldsRevealed = 0;
        }

        private void InitalizeFields() {
            _fields.Clear();

            for (var i = 0; i < _width*_height; i++) {
                _fields.Add(new Field(i%_width, i/_width));
            }
        }

        private void PlaceCues() {
            foreach (var mineField in _mines) {
                var mineNeigbours = GetNeightbours(mineField);

                foreach (var mineNeigbour in mineNeigbours) {
                    if (!mineNeigbour.IsMine) {
                        mineNeigbour.Cues++;
                    }
                }
            }
        }

        private void PlaceFlag(Field field) {
            field.FlagPlaced = !field.FlagPlaced;
        }

        private void Reveal(Field field) {
            if ((Fields.Count == 0) || field.IsRevealed || field.FlagPlaced) {
                return;
            }

            if (!_firstReveal) {
                _firstReveal = true;
                PlaceBombs(field);
                PlaceCues();
            }
            RevealFields(field);
        }

        private void RevealFields(Field field) {
            if (field.Cues > 0) {
                field.IsRevealed = true;
                _fieldsRevealed++;
            }
            // Game over
            else if (field.IsMine) {
                
                foreach (var mine in _mines) {
                    mine.IsRevealed = true;
                }
                return;
            } else {
                var visited = new HashSet<Field>();
                var queue = new Queue<Field>();

                visited.Add(field);
                queue.Enqueue(field);

                while (queue.Count > 0) {
                    var current = queue.Dequeue();

                    if (!current.FlagPlaced) {
                        current.IsRevealed = true;
                        _fieldsRevealed++;
                    }

                    if (current.Cues != 0) {
                        continue;
                    }

                    foreach (var neightbour in GetNeightbours(current)) {
                        if (neightbour.IsRevealed) {
                            continue;
                        }

                        if (visited.Add(neightbour)) {
                            queue.Enqueue(neightbour);
                        }
                    }
                }
            }

            if (_fieldsRevealed == _width*_height - _mineCount) {
                // Victory
            }
        }

        private List<Field> GetNeightbours(Field field) {
            var neightoubrs = new List<Field>();

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

                    neightoubrs.Add(_fields[field.X + i + (field.Y + j)*_width]);
                }
            }

            return neightoubrs;
        }

        private void PlaceBombs(Field field) {
            _mines.Clear();

            var rnd = new Random();
            var fields = new List<int>();

            for (var i = 0; i < _fields.Count; i++) {
                fields.Add(i);
            }

            fields.Remove(_fields.IndexOf(field));

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