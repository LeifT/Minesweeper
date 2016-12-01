using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;

namespace Minesweeper.Model {
    public class GameBoard : ViewModelBase {
        public enum Difficulty {
            Beginner,
            Intermediate,
            Expert
        }

        private readonly List<Field> _mines;
        private int _fieldsRevealed;
        private int _flagsRemaining;
        private int _height;
        private bool _isFirstFieldRevealed;
        private bool _isGameOver;
        private int _mineCount;
        private int _width;

        public GameBoard() {
            Fields = new List<Field>();
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

        #region Properties

        public List<Field> Fields { get; }

        public int Height {
            get { return _height; }
            private set {
                if (_height == value) {
                    return;
                }

                _height = value;
                RaisePropertyChanged();
            }
        }

        public int Width {
            get { return _width; }
            private set {
                if (_width == value) {
                    return;
                }

                _width = value;
                RaisePropertyChanged();
            }
        }

        public int FlagsRemaining {
            get { return _flagsRemaining; }
            private set {
                if (_flagsRemaining == value) {
                    return;
                }

                _flagsRemaining = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Public Methods

        public void SetFlagOrUnknown(Field field) {
            if (_isGameOver) {
                return;
            }

            switch (field.State) {
                case Field.States.Default:
                    field.State = Field.States.Flag;
                    FlagsRemaining--;
                    break;
                case Field.States.Flag:
                    field.State = Field.States.Unknown;
                    FlagsRemaining++;
                    break;
                case Field.States.Unknown:
                    field.State = Field.States.Default;
                    break;
            }
        }

        public void Reveal(Field field) {
            if ((Fields.Count == 0) || (field.State != Field.States.Default) || _isGameOver) {
                return;
            }

            if (!_isFirstFieldRevealed) {
                _isFirstFieldRevealed = true;
                PlaceMines(field);
            }

            RevealFields(field);

            if (_fieldsRevealed == _width*_height - _mineCount) {
                // Victory
            }
        }
        
        public void MultiReveal(Field field) {
            if (!Field.States.Cues.HasFlag(field.State)) {
                return;
            }

            var neighboursWithFlag = GetNeighbours(field).Count(neighbour => neighbour.State == Field.States.Flag);

            if (1 << neighboursWithFlag < (int) field.State) {
                return;
            }

            foreach (var neightbour in GetNeighbours(field)) {
                if (neightbour.State != Field.States.Default) {
                    continue;
                }

                RevealFields(neightbour);
            }
        }

        public void SetDifficultyAndRestart(Difficulty difficulty) {
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

        public void Restart() {
            InitalizeFields();
            _isFirstFieldRevealed = false;
            _isGameOver = false;
            _fieldsRevealed = 0;
            FlagsRemaining = _mineCount;
        }

        #endregion

        #region Private Methods

        private void InitalizeFields() {
            if (Fields.Count < _width*_height) {
                for (var i = Fields.Count; i < _width*_height; i++) {
                    Fields.Add(new Field());
                }
            }

            if (Fields.Count > _width*_height) {
                for (var i = Fields.Count - 1; i >= _width*_height; i--) {
                    Fields.RemoveAt(i);
                }
            }

            for (var i = 0; i < _width*_height; i++) {
                Fields[i].Set(i%_width, i/_width);
            }
        }

        private void RevealFields(Field field) {
            var visited = new HashSet<Field>();
            var queue = new Queue<Field>();

            visited.Add(field);
            queue.Enqueue(field);

            while (queue.Count > 0) {
                var current = queue.Dequeue();

                if (_mines.Contains(current)) {
                    field.State = Field.States.Mine;
                    GameOver();
                    return;
                }

                if (current.State != Field.States.Flag) {
                    var neighbouringMines = GetNeighbours(current).Count(neighbour => _mines.Contains(neighbour));
                    current.State = (Field.States) (1 << neighbouringMines);
                    _fieldsRevealed++;
                }

                if (current.State != Field.States.Blank) {
                    continue;
                }

                foreach (var neighbour in GetNeighbours(current)) {
                    if (Field.States.Cues.HasFlag(neighbour.State)) {
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

            foreach (var field in Fields) {
                if ((field.State == Field.States.Flag) && !_mines.Contains(field)) {
                    field.State = Field.States.WrongFlag;
                }
            }

            foreach (var field in _mines) {
                if (field.State == Field.States.Default) {
                    field.State = Field.States.Mine;
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

                    neightoubrs.Add(Fields[field.X + i + (field.Y + j)*_width]);
                }
            }
            return neightoubrs;
        }

        private void PlaceMines(Field field) {
            var random = new Random();
            var fields = new List<int>();

            _mines.Clear();

            for (var i = 0; i < Fields.Count; i++) {
                fields.Add(i);
            }

            fields.Remove(Fields.IndexOf(field));
            var minesPlaced = 0;

            while (minesPlaced < _mineCount) {
                var mineField = fields[random.Next(fields.Count)];
                _mines.Add(Fields[mineField]);
                fields.Remove(mineField);
                minesPlaced++;
            }
        }

        #endregion
    }
}