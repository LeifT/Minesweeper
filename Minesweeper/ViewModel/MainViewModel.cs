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
        private readonly DispatcherTimer _dispatcherTimer;
        private Difficulty _currentDifficulty;
        private int _minesRemaining;
        private int _secondsFromGameStarted;

        public MainViewModel() {
            Difficulties = new List<Difficulty> {
                new Difficulty("Beginner", 8, 8, 10),
                new Difficulty("Intermediate", 16, 16, 40),
                new Difficulty("Expert", 30, 16, 99)
            };
            
            _currentDifficulty = Difficulties[0];
            _minesRemaining = CurrentDifficulty.Mines;

            GameBoard = new GameBoard(_currentDifficulty.Width, _currentDifficulty.Height, _currentDifficulty.Mines);
            GameBoardView = new CollectionView(GameBoard.Fields);

            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += DispatcherDispatcherTimerTick;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

            GameBoard.GameOver += GameOver;
            GameBoard.GameStart += GameStart;
        }

        public ICommand NewGameCommand => new RelayCommand(() => SetDifficultyAndRestart(CurrentDifficulty));
        public ICommand RevealCommand => new RelayCommand<Field>(GameBoard.Reveal);
        public ICommand PlaceFlagCommand => new RelayCommand<Field>(SetFlagOrUnknown);
        public ICommand MultiRevealCommand => new RelayCommand<Field>(GameBoard.MultiReveal);
        public ICommand SetDifficultyCommand => new RelayCommand<Difficulty>(SetDifficultyAndRestart);

        ~MainViewModel() {
            GameBoard.GameOver -= GameOver;
            GameBoard.GameStart -= GameStart;
        }

        #region Properties

        private GameBoard GameBoard { get; }
        public CollectionView GameBoardView { get; }
        public List<Difficulty> Difficulties { get; }

        public int SecondsFromGameStarted {
            get { return _secondsFromGameStarted; }
            private set {
                _secondsFromGameStarted = value;
                RaisePropertyChanged();
            }
        }

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
                if (_minesRemaining == value) {
                    return;
                }

                _minesRemaining = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Private Methods

        private void GameStart() {
            _dispatcherTimer.Start();
        }

        private void GameOver(bool mineHit) {
            _dispatcherTimer.Stop();

            if (!mineHit) {
                
            }
        }

        private void DispatcherDispatcherTimerTick(object sender, EventArgs e) {
            SecondsFromGameStarted++;
        }

        private void SetFlagOrUnknown(Field field) {
            if (field.State == Field.States.Default) {
                MinesRemaining--;
            } else if (field.State == Field.States.Flag) {
                MinesRemaining++;
            }

            GameBoard.SetFlagOrUnknown(field);
        }

        private void SetDifficultyAndRestart(Difficulty difficulty) {
            _dispatcherTimer.Stop();

            SecondsFromGameStarted = 0;
            CurrentDifficulty = difficulty;
            MinesRemaining = CurrentDifficulty.Mines;

            GameBoard.SetDifficultyAndRestart(CurrentDifficulty.Width, CurrentDifficulty.Height, CurrentDifficulty.Mines);
            GameBoardView.Refresh();
        }

        #endregion
    }
}