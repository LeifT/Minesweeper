using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
            }
        }

        public void Set(int x, int y, States state) {
            X = x;
            Y = y;
            State = state;
        }

        [Flags]
        public enum States {
            Blank = 1 << 0,
            One = 1 << 1, 
            Two = 1 << 2,
            Three = 1 << 3, 
            Four = 1 << 4, 
            Five = 1 << 5,  
            Six = 1 << 6,
            Seven = 1 << 7,
            Eight = 1 << 8,
            Unopened = 1 << 9,
            FlagMark = 1 << 10,
            WrongFlag = 1 << 11,
            QuestionMark= 1 << 12,
            Mine = 1 << 13,

            AllDigits = One | Two | Three | Four | Five | Six | Seven | Eight,
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}