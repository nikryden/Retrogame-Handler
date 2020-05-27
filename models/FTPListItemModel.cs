﻿using FluentFTP;
using RetroGameHandler.Handlers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace RetroGameHandler.models
{
    public class FtpListItemModel : FtpListItem, INotifyPropertyChanged
    {
        private BitmapImage _image;

        private new List<string> ImageFileExtensions = new List<string>() { ".png", ".jpg" };

        public FtpListItemModel()
        {
        }

        public FtpListItemModel(FtpListItem data)
        {
            var dt = data.GetType();
            var df = dt.GetFields();
            var tt = this.GetType();
            foreach (var field in df)
            {
                var tf = tt.GetField(field.Name);
                if (tf == null) continue;
                tf.SetValue(this, tf.GetValue(data));
            }
            var dp = dt.GetProperties();
            foreach (var prop in dp)
            {
                var tp = tt.GetField(prop.Name);
                if (tp == null) continue;
                tp.SetValue(this, tp.GetValue(data));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int ChildCount { get => Items.Count; }
        public bool HasChild { get => Items.Any(); }

        public BitmapImage Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsDirectory { get => mType == FtpFileSystemObjectType.Directory; }

        public bool IsDirectoryOrLink { get => mType == FtpFileSystemObjectType.Directory || mType == FtpFileSystemObjectType.Link; }

        public bool IsFile { get => mType == FtpFileSystemObjectType.File; }

        public bool IsImage
        {
            get
            {
                return (IsFile && ImageFileExtensions.Any(e => Name.ToLower().EndsWith(e.ToLower())));
            }
        }

        public bool IsLink { get => mType == FtpFileSystemObjectType.Link; }
        public ObservableCollection<FtpListItemModel> Items { get; set; } = new ObservableCollection<FtpListItemModel>();

        public FtpListItemModel FindItemByPath(string path)
        {
            if (mFullName == path) return this;
            if (Items.Count == 0) return null;
            foreach (var item in Items)
            {
                if (item == null) continue;
                var pth = item.FindItemByPath(path);
                if (pth != null) return pth;
            }
            return null;
        }

        public string mFullName { get => FullName; }

        public string ParentPath
        {
            get
            {
                return mFullName.Contains("/") ? mFullName.Substring(0, mFullName.LastIndexOf("/")) : mFullName;
            }
        }

        public long mSize { get => Size; }
        public FtpFileSystemObjectSubType mSubType { get => SubType; }
        public FtpFileSystemObjectType mType { get => Type; }

        public void GetImage(bool overwrightIfExists = true)
        {
            if (IsImage && Image == null)
            {
                var pathImage = FtpHandler.Instance.DownloadFile(FullName, Name, overwrightIfExists);
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    var bmi = new BitmapImage();
                    bmi.BeginInit();
                    bmi.CacheOption = BitmapCacheOption.OnLoad;
                    bmi.UriSource = new Uri(pathImage, UriKind.Absolute);
                    Image = bmi;
                    bmi.EndInit();
                });
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FtpNameData
    {
        public FtpNameData(string name)
        {
            FtpFullName = name;
            this.Items = new ObservableCollection<FtpNameData>();
        }

        public FtpNameData FindItemByPath(string path)
        {
            if (FtpFullName == path) return this;
            if (Items.Count == 0) return null;
            foreach (var item in Items)
            {
                var pth = item.FindItemByPath(path);
                if (pth != null) return pth;
            }
            return null;
        }

        public string FtpFullName { get; set; }
        public ObservableCollection<FtpNameData> Items { get; set; }
        public string Name { get => FtpFullName.Contains("/") ? FtpFullName.Substring(FtpFullName.LastIndexOf("/") + 1) : FtpFullName; }
    }
}