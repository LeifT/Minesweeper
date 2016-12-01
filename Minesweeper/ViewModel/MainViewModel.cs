using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Minesweeper.Model;

namespace Minesweeper.ViewModel {
    public class MainViewModel : ViewModelBase {
        private int _width;
        private int _height;
        private GameBoard GameBoard { get; }
        public CollectionView ViewBoard { get; }
        public List<Difficulty> Difficulties { get; }

        public ICommand RestartCommand => new RelayCommand(GameBoard.Restart);
        public ICommand RevealCommand => new RelayCommand<Field>(GameBoard.Reveal);
        public ICommand PlaceFlagCommand => new RelayCommand<Field>(GameBoard.SetFlagOrUnknown);
        public ICommand MultiRevealCommand => new RelayCommand<Field>(GameBoard.MultiReveal);
        public ICommand SetDifficultyCommand => new RelayCommand<Difficulty>(SetDifficultyAndRestart);

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
            Difficulties = new List<Difficulty> {
                new Difficulty("Beginner", 8, 8, 10),
                new Difficulty("Intermediate", 16, 16, 40),
                new Difficulty("Expert", 30, 16, 99)
            };

            GameBoard = new GameBoard();
            ViewBoard = new CollectionView(GameBoard.Fields);
        }

        private void SetDifficultyAndRestart(Difficulty diff) {
            Width = diff.Width;
            Height = diff.Height;

            GameBoard.SetDifficultyAndRestart(Width, Height, diff.Mines);
            ViewBoard.Refresh();
        }
    }
}