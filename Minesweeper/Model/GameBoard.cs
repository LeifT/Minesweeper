using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Model {
    public class GameBoard {
        private readonly List<Field> _mines;
        private int _fieldsRevealed;
        private bool _isFirstFieldRevealed;
        private int _mineCount;

        public delegate void GameOverHandler(GameOverResult gameOverResult);
        public delegate void GameStartHandler();
        public event GameOverHandler GameOver;
        public event GameStartHandler GameStart;
        
        public GameBoard(Difficulty difficulty) {
            Fields = new List<Field>();
            _mines = new List<Field>();

            Width = difficulty.Width;
            Height = difficulty.Height;
            _mineCount = difficulty.Mines;

            Restart();
        }

        #region Properties

        public List<Field> Fields { get; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        #endregion

        #region Public Methods

        public void MarkField(Field field) {
            switch (field.State) {
                case Field.States.Unopened:
                    field.State = Field.States.FlagMark;
                    break;
                case Field.States.FlagMark:
                    field.State = Field.States.QuestionMark;
                    break;
                case Field.States.QuestionMark:
                    field.State = Field.States.Unopened;
                    break;
            }
        }

        public void Reveal(Field field) {
            if (field.State != Field.States.Unopened) {
                return;
            }

            if (!_isFirstFieldRevealed) {
                _isFirstFieldRevealed = true;
                PlaceMines(field);
                GameStart?.Invoke();
            }

            RevealFields(field);
            
            if (_fieldsRevealed == Width * Height - _mineCount) {
                GameOver?.Invoke(GameOverResult.Won);
            }
        }
        
        public void MultiReveal(Field field) {
            if (!Field.States.AllDigits.HasFlag(field.State)) {
                return;
            }

            var neighboursWithFlag = GetNeighbours(field).Count(neighbour => neighbour.State == Field.States.FlagMark);

            if (1 << neighboursWithFlag < (int) field.State) {
                return;
            }

            foreach (var neightbour in GetNeighbours(field)) {
                if (neightbour.State != Field.States.Unopened) {
                    continue;
                }

                RevealFields(neightbour);
            }
            
            if (_fieldsRevealed == Width * Height - _mineCount) {
                GameOver?.Invoke(GameOverResult.Won);
            }
        }

        public void SetDifficultyAndRestart(Difficulty difficulty) {
            Width = difficulty.Width;
            Height = difficulty.Height;
            _mineCount = difficulty.Mines;
            Restart();
        }

        public void Restart() {
            InitalizeFields();
            _isFirstFieldRevealed = false;
            _fieldsRevealed = 0;
        }

        #endregion

        #region Private Methods

        private void InitalizeFields() {
            var fieldCount = Width*Height;

            for (var i = Fields.Count; i < fieldCount; i++) {
                Fields.Add(new Field());
            }

            Fields.RemoveRange(fieldCount, Fields.Count - fieldCount);

            for (var i = 0; i < fieldCount; i++) {
                Fields[i].Set(i % Width, i / Width, Field.States.Unopened);
            }
        }

        private void RevealFields(Field field) {
            if (_mines.Contains(field)) {
                field.State = Field.States.Mine;
                MineHit();
                return;
            }

            var visited = new HashSet<Field>();
            var queue = new Queue<Field>();

            visited.Add(field);
            queue.Enqueue(field);

            while (queue.Count > 0) {
                var current = queue.Dequeue();

                if (current.State != Field.States.FlagMark && current.State != Field.States.QuestionMark) {
                    var neighbouringMinesCount = GetNeighbours(current).Count(neighbour => _mines.Contains(neighbour));
                    current.State = (Field.States) (1 << neighbouringMinesCount);
                    _fieldsRevealed++;
                }

                if (current.State != Field.States.Blank) {
                    continue;
                }

                foreach (var neighbour in GetNeighbours(current)) {
                    if (Field.States.AllDigits.HasFlag(neighbour.State)) {
                        continue;
                    }

                    if (visited.Add(neighbour)) {
                        queue.Enqueue(neighbour);
                    }
                }
            }
        }

        private void MineHit() {
            foreach (var field in Fields) {
                if ((field.State == Field.States.FlagMark) && !_mines.Contains(field)) {
                    field.State = Field.States.WrongFlag;
                }
            }

            foreach (var field in _mines) {
                if (field.State == Field.States.Unopened) {
                    field.State = Field.States.Mine;
                }
            }

            GameOver?.Invoke(GameOverResult.Lost);
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

        private void PlaceMines(Field startField) {
            var random = new Random();
            var fields = new List<int>();

            _mines.Clear();

            for (var i = 0; i < Fields.Count; i++) {
                fields.Add(i);
            }

            fields.Remove(Fields.IndexOf(startField));

            for (int i = 0; i < _mineCount; i++) {
                var mineField = fields[random.Next(fields.Count)];
                _mines.Add(Fields[mineField]);
                fields.Remove(mineField);
            }
        }

        #endregion

        public enum GameOverResult {
            Lost,
            Won
        }
    }
}