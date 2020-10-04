using RetroGameHandler.Handlers;
using RetroGameHandler.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;

using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RetroGameHandler.models
{
    public class DownloadImageModel : INotifyPropertyChanged, IConvert
    {
        public DownloadImageModel(int Id, string Name, string GameTitle, string FullName, string Url, bool MultiImages, bool useDirectory, string overview, string releseDate, int players, string publisher, string rating)
        {
            this.Id = Id;
            this.Name = Name;
            this.GameTitle = GameTitle;
            this.FullName = FullName;
            this.MultiImages = MultiImages;
            this.IsDirectoryName = useDirectory;
            this.Overview = overview;
            this.ReleseDate = releseDate;
            Players = players;
            Publisher = publisher;
            Rating = rating;
            DownloadPath = Url;
            var tmpName = TheGamesDbHandler.CleanGameName(Name, true)
                .Replace("&amp;", string.Empty)
                .Replace("-", string.Empty)
                .Replace("/", string.Empty)
                .Replace(":", string.Empty)
                .Replace(";", string.Empty)
                .Replace("*", string.Empty)
                .Replace("_", string.Empty)
                .Replace(" ", string.Empty)
                .Replace("'", string.Empty)
                .Replace(".", string.Empty)
                .Replace(",", string.Empty)
                .Replace("%", string.Empty)
                .Replace("The", string.Empty)
                .Replace("!", string.Empty)
                .Replace("é", "e");
            var tmpGameTitle = GameTitle
                 .Replace("&amp;", string.Empty)
                .Replace("-", string.Empty)
                .Replace("/", string.Empty)
                .Replace(":", string.Empty)
                .Replace(";", string.Empty)
                .Replace("*", string.Empty)
                .Replace("_", string.Empty)
                .Replace(" ", string.Empty)
                .Replace("'", string.Empty)
                .Replace(".", string.Empty)
                .Replace(",", string.Empty)
                .Replace("%", string.Empty)
                .Replace("!", string.Empty)
                .Replace("The", string.Empty)
                .Replace("é", "e"); ;
            IsSelected = tmpName.Trim().ToLower() == tmpGameTitle.Trim().ToLower();
            //if (!IsSelected) IsSelected = tmpGameTitle.Trim().ToLower().Contains(tmpName.Trim().ToLower());
            //if (!string.IsNullOrWhiteSpace(Url))
            //{
            //    using (WebClient client = new WebClient())
            //    {
            //        ImageStream = client.DownloadData(Url);
            //    }
            //    HasImage = true;
            //}
            //else
            //{
            //    var bm = new BitmapImage(new Uri("pack://application:,,,/Images/image-not-found.png"));
            //    Stream stream = bm.StreamSource;
            //    byte[] buffer = null;
            //    if (stream != null && stream.Length > 0)
            //    {
            //        using (BinaryReader br = new BinaryReader(stream))
            //        {
            //            buffer = br.ReadBytes((Int32)stream.Length);
            //        }
            //    }
            //    ImageStream = buffer;
            //    //Image = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/image-not-found.png")));
            //    HasImage = false;
            //}
        }

        public DownloadImageModel(int Id, string Name, string GameTitle, string FullName, string overview)
        {
            this.Id = Id;
            this.Name = Name;
            this.GameTitle = GameTitle;
            this.FullName = FullName;
            this.Overview = overview;
            MultiImages = false;
            //byte[] data = client.DownloadData(url);
            using (MemoryStream mem = new MemoryStream())
            {
                FtpHandler.Instance.DownloadStream(FullName, mem);
                ImageStream = mem.ToArray();
            }
            HasImage = true;
            IsUploaded = true;
        }

        private byte[] _imageStream;
        private bool _isSelected;

        public int Id
        {
            get; set;
        }

        public string Name { get; set; }
        public string FullName { get; set; }
        public string GameTitle { get; set; }
        public string Overview { get; set; }
        public int Players { get; set; }
        public string Publisher { get; set; }
        public string Rating { get; set; }

        public string ReleseDate { get; set; }
        public string DownloadPath { get; set; }
        public bool MultiImages { get; set; }
        public bool HasImage { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsDirectoryName { get; set; }
        public bool IsUploaded { get; set; }
        public bool ShowCheckbox { get => HasImage && !IsUploaded; }

        public byte[] ImageStream
        {
            get
            {
                return _imageStream;
            }
            set
            {
                _imageStream = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("Image");
            }
        }

        public ImageBrush Image
        {
            get
            {
                if (ImageStream == null || HasImage == false) return null;
                using (MemoryStream mem = new MemoryStream(ImageStream))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = mem;

                    image.EndInit();
                    image.Freeze();
                    return new ImageBrush(image);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Convert(object obj)
        {
            throw new NotImplementedException();
        }

        public void ConvertBack(object obj)
        {
            throw new NotImplementedException();
        }
    }
}