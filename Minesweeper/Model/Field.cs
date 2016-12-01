using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;
using Minesweeper.Annotations;

namespace Minesweeper.Model {
    public class Field : INotifyPropertyChanged {
        private States _state;

        public int X { get; set; }
        public int Y { get; set; }

        public States State {
            get { return _state; }
            set {
                _state = value;
                OnPropertyChanged(nameof(State));
                //RaisePropertyChanged();
            }
        }

        public void Set(int x, int y) {
            X = x;
            Y = y;
            State = States.Default;
        }

        [Flags]
        public enum States {
            Blank     = 1 << 0,
            One       = 1 << 1, 
            Two       = 1 << 2,
            Three     = 1 << 3, 
            Four      = 1 << 4, 
            Five      = 1 << 5,  
            Six       = 1 << 6,
            Seven     = 1 << 7,
            Eight     = 1 << 8,
            Default   = 1 << 9,
            Flag      = 1 << 10,
            WrongFlag = 1 << 11,
            Unknown   = 1 << 12,
            Mine      = 1 << 13,

            Cues = One | Two | Three | Four | Five | Six | Seven | Eight,
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}