using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Minesweeper.Model;

namespace Minesweeper.ViewModel {
    public class MainViewModel : ViewModelBase {
        public GameBoard GameBoard { get; set; }

        public MainViewModel() {
            GameBoard = new GameBoard();
        }

        public ICommand RestartCommand => new RelayCommand(GameBoard.Restart);
        public ICommand RevealCommand => new RelayCommand<Field>(GameBoard.Reveal);
        public ICommand PlaceFlagCommand => new RelayCommand<Field>(GameBoard.PlaceFlag);
        public ICommand MultiRevealCommand => new RelayCommand<Field>(GameBoard.MultiReveal);
        public ICommand SetDifficultyCommand => new RelayCommand<GameBoard.Difficulty>(GameBoard.SetDifficulty);
    }
}