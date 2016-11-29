using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Minesweeper.Model;

namespace Minesweeper.ViewModel {
    public class MainViewModel : ViewModelBase {
        public enum Difficulty {
            Beginner,
            Intermediate,
            Expert
        }

        private int _mineCount;
        private readonly List<Field> _mines;
        private ObservableCollection<Field> _fields;
        private int _fieldsRevealed;
        private bool _isFirstFieldRevealed;
        private int _height;
        private bool _isMineHit;
        private int _width;
        private int _flagsRemaining;

        public MainViewModel() {
            _fields = new ObservableCollection<Field>();
            _mines = new List<Field>();

            _width = 8;
            _height = 8;
            _mineCount = 10;

            InitalizeFields();
            _isFirstFieldRevealed = false;
            _isMineHit = false;
            _fieldsRevealed = 0;
            _flagsRemaining = _mineCount;
        }

        public ICommand RestartCommand => new RelayCommand(Restart);
        public ICommand RevealCommand => new RelayCommand<Field>(Reveal);
        public ICommand PlaceFlagCommand => new RelayCommand<Field>(PlaceFlag);
        public ICommand MultiRevealCommand => new RelayCommand<Field>(MultiReveal);
        public ICommand SetDifficultyCommand => new RelayCommand<Difficulty>(SetDifficulty);

        private void MultiReveal(Field field) {
            if (field.Cues == 0 || !field.IsRevealed) {
                return;
            }

            int neigthbouringFlags = 0;

            foreach (var neightbour in GetNeightbours(field)) {
                if (neightbour.IsFlagPlaced) {
                    neigthbouringFlags++;
                }
            }

            if (neigthbouringFlags >= field.Cues) {
                foreach (var neightbour in GetNeightbours(field)) {
                    if (neightbour.IsRevealed || neightbour.IsFlagPlaced) {
                        continue;
                    }
                    RevealFields(neightbour);
                }
            }
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

        public int FlagsRemaining {
            get { return _flagsRemaining; }
            set {
                if (_flagsRemaining == value) {
                    return;
                }

                _flagsRemaining = value;
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

        private void SetDifficulty(Difficulty difficulty) {
            switch (difficulty) {
                case Difficulty.Beginner:
                    Width = 8;
                    Height = 8;
                    _mineCount = 10;
                    break;
                case Difficulty.Intermediate:
                    Width = 16;
                    Height = 16;
                    _mineCount = 40;
                    break;
                case Difficulty.Expert:
                    Width = 30;
                    Height = 16;
                    _mineCount = 99;
                    break;
            }
            Restart();
        }

        private void Restart() {
            InitalizeFields();
            _isFirstFieldRevealed = false;
            _isMineHit = false;
            _fieldsRevealed = 0;
            FlagsRemaining = _mineCount;
        }

        private void InitalizeFields() {
            if (_fields.Count < _width*_height) {
                for (var i = _fields.Count; i < _width*_height; i++) {
                    _fields.Add(new Field());
                }
            }

            if (_fields.Count > _width * _height) {
                for (var i = _fields.Count - 1; i >= _width * _height; i--) {
                    _fields.RemoveAt(i);
                }
            }

            for (var i = 0; i < _width * _height; i++) {
                _fields[i].Set(i % _width, i / _width);
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
            if (field.IsRevealed || _isMineHit) {
                return;
            }

            field.IsFlagPlaced = !field.IsFlagPlaced;

            if (field.IsFlagPlaced) {
                FlagsRemaining--;
            } else {
                FlagsRemaining++;
            }
        }

        private void Reveal(Field field) {
            if ((Fields.Count == 0) || field.IsRevealed || field.IsFlagPlaced || _isMineHit) {
                return;
            }

            if (!_isFirstFieldRevealed) {
                _isFirstFieldRevealed = true;
                PlaceBombs(field);
                PlaceCues();
            }
            RevealFields(field);
        }

        

        private void RevealFields(Field field) {
            if (field.IsMine) {
                field.IsRevealed = true;
                GameOver();
                return;
            }

            if (field.Cues > 0) {
                field.IsRevealed = true;
                _fieldsRevealed++;
            } else {
                RevealEmptyFields(field);
            }

            if (_fieldsRevealed == _width*_height - _mineCount) {
                // Victory
            }
        }

        private void RevealEmptyFields(Field field) {
            var visited = new HashSet<Field>();
            var queue = new Queue<Field>();

            visited.Add(field);
            queue.Enqueue(field);

            while (queue.Count > 0) {
                var current = queue.Dequeue();

                if (!current.IsFlagPlaced) {
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

        private void GameOver() {
            _isMineHit = true;

            foreach (var mine in _mines) {
                mine.IsRevealed = true;
            }

            foreach (var field in _fields) {
                if (field.IsFlagPlaced && !field.IsMine) {
                    field.IsFlagMissPlaced = true;
                }
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