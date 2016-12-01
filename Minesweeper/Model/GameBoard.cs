using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Model {
    public class GameBoard {
        private readonly List<Field> _mines;
        private int _fieldsRevealed;
        private bool _isFirstFieldRevealed;
        private bool _isGameOver;
        private int _mineCount;

        public GameBoard(int width, int height, int mines) {
            Fields = new List<Field>();
            _mines = new List<Field>();

            Width = width;
            Height = height;
            _mineCount = mines;

            Restart();
        }

        #region Properties

        public List<Field> Fields { get; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        public int FlagsRemaining { get; private set; }

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

            if (_fieldsRevealed == Width*Height - _mineCount) {
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

        public void SetDifficultyAndRestart(int columns, int rows, int mines) {
            Width = columns;
            Height = rows;
            _mineCount = mines;
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
            if (Fields.Count < Width*Height) {
                for (var i = Fields.Count; i < Width*Height; i++) {
                    Fields.Add(new Field());
                }
            }

            if (Fields.Count > Width*Height) {
                for (var i = Fields.Count - 1; i >= Width*Height; i--) {
                    Fields.RemoveAt(i);
                }
            }

            for (var i = 0; i < Width*Height; i++) {
                Fields[i].Set(i%Width, i/Width);
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

                    if ((field.X + i < 0) || (field.X + i >= Width)) {
                        continue;
                    }

                    if ((field.Y + j < 0) || (field.Y + j >= Height)) {
                        continue;
                    }

                    neightoubrs.Add(Fields[field.X + i + (field.Y + j)*Width]);
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