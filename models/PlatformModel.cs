using RetroGameHandler.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RetroGameHandler.models
{
    public class PlatformModel : ICloneable, INotifyPropertyChanged
    {
        public PlatformModel()
        {
        }

        public PlatformModel(PlatformEntity platformEntity, BitmapImage bitmapImage = null)
        {
            foreach (var item in platformEntity.GetType().GetProperties())
            {
                var itm = this.GetType().GetProperty(item.Name);
                if (itm != null) itm.SetValue(this, item.GetValue(platformEntity));
            }
            if (bitmapImage != null) Image = new ImageBrush(bitmapImage);
        }

        private int _id = 1;
        private string _name;
        private string _alias;
        private string _icon;
        private string _console;
        private string _controller;
        private string _developer;
        private string _overview;
        private ImageBrush _image;
        private List<string> _extensions;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                NotifyPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        public string Alias
        {
            get => _alias;
            set
            {
                _alias = value;
                NotifyPropertyChanged();
            }
        }

        public string Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                NotifyPropertyChanged();
            }
        }

        public string Console
        {
            get => _console;
            set
            {
                _console = value;
                NotifyPropertyChanged();
            }
        }

        public string Controller
        {
            get => _controller;
            set
            {
                _controller = value;
                NotifyPropertyChanged();
            }
        }

        public string Developer
        {
            get => _developer;
            set
            {
                _developer = value;
                NotifyPropertyChanged();
            }
        }

        public string Overview
        {
            get => _overview;
            set
            {
                _overview = value;
                NotifyPropertyChanged();
            }
        }

        public ImageBrush Image
        {
            get => _image;
            private set
            {
                _image = value;

                NotifyPropertyChanged();
            }
        }

        public List<string> Extensions
        {
            get => _extensions;
            private set
            {
                _extensions = value;

                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}