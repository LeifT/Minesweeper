using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Minesweeper.Model;

namespace Minesweeper.ViewModel {
    public class MainViewModel : ViewModelBase {
        private GameBoard GameBoard { get; }
        public CollectionView GameBoardView { get; }
        public List<Difficulty> Difficulties { get; }
        private Difficulty _currentDifficulty;
        private int _minesRemaining;

        public int Time {
            get { return _time; }
            private set {
                _time = value; 
                RaisePropertyChanged();
            }
        }

        public ICommand RestartCommand => new RelayCommand(Restart);
        public ICommand RevealCommand => new RelayCommand<Field>(GameBoard.Reveal);
        public ICommand PlaceFlagCommand => new RelayCommand<Field>(SetFlagOrUnknown);
        public ICommand MultiRevealCommand => new RelayCommand<Field>(GameBoard.MultiReveal);
        public ICommand SetDifficultyCommand => new RelayCommand<Difficulty>(SetDifficultyAndRestart);

        private DispatcherTimer dispatcherTimer;
        private int _time;

        public Difficulty CurrentDifficulty {
            get { return _currentDifficulty; }
            private set {
                _currentDifficulty = value;
                RaisePropertyChanged();
            }
        }

        public int MinesRemaining {
            get { return _minesRemaining; }
            private set {
                _minesRemaining = value;
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
            _minesRemaining = CurrentDifficulty.Mines;

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherDispatcherTimerTick;
            dispatcherTimer.Interval = new TimeSpan(0,0,1);
            dispatcherTimer.Start();
        }

        private void DispatcherDispatcherTimerTick(object sender, EventArgs e) {
            Time++;
        }

        private void Restart() {
            MinesRemaining = CurrentDifficulty.Mines;
            GameBoard.Restart();
        }

        private void SetFlagOrUnknown(Field field) {
            if (field.State == Field.States.Default) {
                MinesRemaining--;
            } else if(field.State == Field.States.Flag) {
                MinesRemaining++;
            }

            GameBoard.SetFlagOrUnknown(field);
        }

        private void SetDifficultyAndRestart(Difficulty difficulty) {
            CurrentDifficulty = difficulty;
            MinesRemaining = CurrentDifficulty.Mines;
            GameBoard.SetDifficultyAndRestart(CurrentDifficulty.Width, CurrentDifficulty.Height, CurrentDifficulty.Mines);
            GameBoardView.Refresh();
        }
    }
}