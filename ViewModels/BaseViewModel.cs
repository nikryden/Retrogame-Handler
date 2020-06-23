using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RetroGameHandler.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private bool _dirty = false;

        public bool Dirty
        {
            get => _dirty;
            set
            {
                _dirty = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (propertyName != "Dirty") Dirty = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static implicit operator BaseViewModel(Views.RetroResorcesView v)
        {
            throw new NotImplementedException();
        }
    }
}