using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RetroGameHandler.Handlers
{
    public class NotifyHandler : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}