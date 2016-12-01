using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Minesweeper.Model;

namespace Minesweeper.ViewModel {
    public class MainViewModel : ViewModelBase {
        public GameBoard GameBoard { get; set; }
        public CollectionView ViewBoard { get; }

        public MainViewModel() {
            GameBoard = new GameBoard();
            ViewBoard = new CollectionView(GameBoard.Fields);
        }

        public ICommand RestartCommand => new RelayCommand(GameBoard.Restart);
        public ICommand RevealCommand => new RelayCommand<Field>(GameBoard.Reveal);
        public ICommand PlaceFlagCommand => new RelayCommand<Field>(GameBoard.SetFlagOrUnknown);
        public ICommand MultiRevealCommand => new RelayCommand<Field>(GameBoard.MultiReveal);
        public ICommand SetDifficultyCommand => new RelayCommand<GameBoard.Difficulty>(Test);

        private void Test(GameBoard.Difficulty diff) {
            GameBoard.SetDifficultyAndRestart(diff);
            ViewBoard.Refresh();
        }
    }
}