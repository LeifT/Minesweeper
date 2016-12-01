using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Minesweeper.Model;

namespace Minesweeper.ViewModel {
    public class MainViewModel : ViewModelBase {
        private GameBoard GameBoard { get; }
        public CollectionView GameBoardView { get; }
        public List<Difficulty> Difficulties { get; }
        private Difficulty _currentDifficulty;

        public ICommand RestartCommand => new RelayCommand(GameBoard.Restart);
        public ICommand RevealCommand => new RelayCommand<Field>(GameBoard.Reveal);
        public ICommand PlaceFlagCommand => new RelayCommand<Field>(GameBoard.SetFlagOrUnknown);
        public ICommand MultiRevealCommand => new RelayCommand<Field>(GameBoard.MultiReveal);
        public ICommand SetDifficultyCommand => new RelayCommand<Difficulty>(SetDifficultyAndRestart);

        public Difficulty CurrentDifficulty {
            get { return _currentDifficulty; }
            set {
                _currentDifficulty = value;
                RaisePropertyChanged();
            }
        }

        public MainViewModel() {
            Difficulties = new List<Difficulty> {
                new Difficulty("Beginner", 8, 8, 10),
                new Difficulty("Intermediate", 16, 16, 40),
                new Difficulty("Expert", 30, 16, 99)
            };

            _currentDifficulty = Difficulties[0];
            GameBoard = new GameBoard(_currentDifficulty.Width, _currentDifficulty.Height,_currentDifficulty.Mines);
            GameBoardView = new CollectionView(GameBoard.Fields);
        }

        private void SetDifficultyAndRestart(Difficulty difficulty) {
            CurrentDifficulty = difficulty;
            GameBoard.SetDifficultyAndRestart(CurrentDifficulty.Width, CurrentDifficulty.Height, CurrentDifficulty.Mines);
            GameBoardView.Refresh();
        }
    }
}