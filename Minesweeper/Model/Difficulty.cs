namespace Minesweeper.Model {
    public class Difficulty {
        public string Name { get; set; }
        public int Width { get; }
        public int Height { get; }
        public int Mines { get; }

        public Difficulty(string name, int width, int height, int mines) {
            Name = name;
            Width = width;
            Height = height;
            Mines = mines;
        }
    }
}