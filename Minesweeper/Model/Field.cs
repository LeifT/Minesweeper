namespace Minesweeper.Model {
    public class Field {
        public int X { get; set; }
        public int Y { get; set; }
        public int Cues { get; set; }
        public bool IsMine { get; set; }

        public Field(int x, int y) {
            X = x;
            Y = y;
            IsMine = false;
            Cues = 0;
        }
    }
}