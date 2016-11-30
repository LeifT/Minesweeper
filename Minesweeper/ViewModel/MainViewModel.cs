using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Minesweeper.Model;
using static Minesweeper.Model.Field;

namespace Minesweeper.ViewModel {
    public class MainViewModel : ViewModelBase {
        public enum Difficulty {
            Beginner,
            Intermediate,
            Expert
        }

        private readonly List<Field> _mines;
        private ObservableCollection<Field> _fields;
        private int _fieldsRevealed;
        private int _flagsRemaining;
        private int _height;
        private bool _isFirstFieldRevealed;
        private bool _isGameOver;
        private int _mineCount;
        private int _width;

        public MainViewModel() {
            _fields = new ObservableCollection<Field>();
            _mines = new List<Field>();

            _width = 8;
            _height = 8;
            _mineCount = 10;

            InitalizeFields();
            _isFirstFieldRevealed = false;
            _isGameOver = false;
            _fieldsRevealed = 0;
            _flagsRemaining = _mineCount;
        }

        public ICommand RestartCommand => new RelayCommand(Restart);
        public ICommand RevealCommand => new RelayCommand<Field>(Reveal);
        public ICommand PlaceFlagCommand => new RelayCommand<Field>(PlaceFlag);
        public ICommand MultiRevealCommand => new RelayCommand<Field>(MultiReveal);
        public ICommand SetDifficultyCommand => new RelayCommand<Difficulty>(SetDifficulty);

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

        private void MultiReveal(Field field) {
            if (!States.Cues.HasFlag(field.State)) {
                return;
            }

            var neightboursWithFlag = GetNeighbours(field).Count(neightbour => neightbour.State == States.Flag);

            if (1 << neightboursWithFlag < (int) field.State) {
                return;
            }

            foreach (var neightbour in GetNeighbours(field)) {
                if (neightbour.State != States.Default) {
                    continue;
                }

                RevealFields(neightbour);
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
            _isGameOver = false;
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

            for (var i = 0; i < _width*_height; i++) {
                _fields[i].Set(i%_width, i/_width);
            }
        }

        private void PlaceFlag(Field field) {
            if (_isGameOver) {
                return;
            }

            switch (field.State) {
                case States.Default:
                    field.State = States.Flag;
                    FlagsRemaining--;
                    break;
                case States.Flag:
                    field.State = States.Unknown;
                    FlagsRemaining++;
                    break;
                case States.Unknown:
                    field.State = States.Default;
                    break;
            }
        }

        private void Reveal(Field field) {
            if ((Fields.Count == 0) || (field.State != States.Default) || _isGameOver) {
                return;
            }

            if (!_isFirstFieldRevealed) {
                _isFirstFieldRevealed = true;
                PlaceMines(field);
            }

            if (_mines.Contains(field)) {
                field.State = States.Mine;
                GameOver();
                return;
            }

            RevealFields(field);

            if (_fieldsRevealed == _width * _height - _mineCount) {
                // Victory
            }
        }

        private void RevealFields(Field field) {
            var visited = new HashSet<Field>();
            var queue = new Queue<Field>();

            visited.Add(field);
            queue.Enqueue(field);

            while (queue.Count > 0) {
                var current = queue.Dequeue();

                if (current.State != States.Flag) {
                    var neighbouringMines = GetNeighbours(current).Count(neighbour => _mines.Contains(neighbour));
                    current.State = (States) (1 << neighbouringMines);
                    _fieldsRevealed++;
                }

                if (current.State != States.Blank) {
                    continue;
                }

                foreach (var neighbour in GetNeighbours(current)) {
                    if (States.Cues.HasFlag(neighbour.State)) {
                        continue;
                    }

                    if (visited.Add(neighbour)) {
                        queue.Enqueue(neighbour);
                    }
                }
            }
        }

        private void GameOver() {
            _isGameOver = true;

            foreach (var field in _fields) {
                if ((field.State == States.Flag) && !_mines.Contains(field)) {
                    field.State = States.WrongFlag;
                }
            }

            foreach (var field in _mines) {
                if (field.State == States.Default) {
                    field.State = States.Mine;
                }
            }
        }

        private List<Field> GetNeighbours(Field field) {
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

        private void PlaceMines(Field field) {
            var random = new Random();
            var fields = new List<int>();

            _mines.Clear();

            for (var i = 0; i < _fields.Count; i++) {
                fields.Add(i);
            }

            fields.Remove(_fields.IndexOf(field));
            var minesPlaced = 0;

            while (minesPlaced < _mineCount) {
                var mineField = fields[random.Next(fields.Count)];
                fields.Remove(mineField);
                _mines.Add(_fields[mineField]);
                minesPlaced++;
            }
        }
    }
}