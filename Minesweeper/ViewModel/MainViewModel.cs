using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Minesweeper.Model;

namespace Minesweeper.ViewModel {
    public class MainViewModel : ViewModelBase {
        private int _width;
        private int _height;
        private GameBoard GameBoard { get; set; }
        public CollectionView ViewBoard { get; }

        public int Width {
            get { return _width; }
            set {
                _width = value;
                RaisePropertyChanged();
            }
        }

        public int Height {
            get { return _height; }
            set {
                _height = value;
                RaisePropertyChanged();
            }
        }

        public MainViewModel() {
            GameBoard = new GameBoard();
            ViewBoard = new CollectionView(GameBoard.Fields);
        }

        public ICommand RestartCommand => new RelayCommand(GameBoard.Restart);
        public ICommand RevealCommand => new RelayCommand<Field>(GameBoard.Reveal);
        public ICommand PlaceFlagCommand => new RelayCommand<Field>(GameBoard.SetFlagOrUnknown);
        public ICommand MultiRevealCommand => new RelayCommand<Field>(GameBoard.MultiReveal);
        public ICommand SetDifficultyCommand => new RelayCommand<Difficulty>(SetDifficultyAndRestart);

        private void SetDifficultyAndRestart(Difficulty diff) {
            int mines = 0;

            switch (diff) {
                case Difficulty.Beginner:
                    Width = 8;
                    Height = 8;
                    mines = 10;
                    break;
                case Difficulty.Intermediate:
                    Width = 16;
                    Height = 16;
                    mines = 40;
                    break;
                case Difficulty.Expert:
                    Width = 30;
                    Height = 16;
                    mines = 99;
                    break;
            }

            GameBoard.SetDifficultyAndRestart(Width, Height, mines);
            ViewBoard.Refresh();
        }

        public enum Difficulty {
            Beginner,
            Intermediate,
            Expert
        }
    }
}