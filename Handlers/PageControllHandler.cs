using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RetroGameHandler.Handlers
{
    public class PageControllHandler
    {
        private MainButton Next = new MainButton(ButtonTypes.Next);
        private MainButton Previous = new MainButton(ButtonTypes.Previous);
        private MainButton Close = new MainButton(ButtonTypes.Close);

        public PageControllHandler()
        {
            Next.Click += Next_Click;
            Previous.Click += Previous_Click;
            Close.Click += Close_Click;
        }

        private void Close_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Previous_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Next_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }

    public class MainButton : INotifyPropertyChanged
    {
        public event EventHandler<EventArgs> Click;

        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;

        public MainButton(ButtonTypes buttonType)
        {
            _buttonType = buttonType;
            Name = $"{buttonType}";
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public ButtonTypes _buttonType;

        public void Button_Click()
        {
            Click?.Invoke(this, new EventArgs());
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public enum ButtonTypes
    {
        Next,
        Previous,
        Close
    }
}