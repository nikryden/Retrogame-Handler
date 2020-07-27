using FluentFTP;
using RetroGameHandler.Handlers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using SevenZipExtractor;
using System.Windows.Forms;

namespace RetroGameHandler.models
{
    public class FtpListItemModel : FtpListItem, INotifyPropertyChanged
    {
        private BitmapImage _image;

        private new List<string> ImageFileExtensions = new List<string>() { ".png", ".jpg", ".bmp" };

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
        public string LastSearchParam { get; set; }

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

        public bool IsOpk
        {
            get
            {
                return (IsFile && Path.GetExtension(Name) == ".opk");
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

        public async Task<opkInfo> GetOpkInfo()
        {
            return await Task.Run(() =>
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    var opk = FtpHandler.Instance.DownloadStream(FullName, ms);
                    ms.Position = 0;
                    using (ArchiveFile archiveFile = new ArchiveFile(ms, SevenZipFormat.SquashFS))
                    {
                        var ls = archiveFile.Entries.FirstOrDefault(e => e.FileName.EndsWith("gcw0.desktop"));
                        if (ls != null)
                        {
                            var str = ls.ToString();
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                ls.Extract(memoryStream);
                                using (StreamReader reader = new StreamReader(memoryStream))
                                {
                                    reader.BaseStream.Position = 0;
                                    var txt = reader.ReadToEnd();
                                    var texts = txt.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                                    var opkinf = new opkInfo();
                                    var opkType = opkinf.GetType();
                                    for (int i = 1; i < texts.Length; i++)
                                    {
                                        var sp = texts[i].Split('=');
                                        var prop = opkType.GetProperty(sp[0].Trim());
                                        if (prop == null) continue;
                                        prop.SetValue(opkinf, sp[1].Trim());
                                    }
                                    var img = archiveFile.Entries.FirstOrDefault(e => e.FileName == $"{opkinf.Icon}.png");
                                    if (img != null)
                                    {
                                        using (MemoryStream msimg = new MemoryStream())
                                        {
                                            img.Extract(msimg);
                                            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                            {
                                                var bmi = new BitmapImage();
                                                bmi.BeginInit();
                                                bmi.CacheOption = BitmapCacheOption.OnLoad;
                                                bmi.StreamSource = msimg;
                                                opkinf.Image = bmi;
                                                bmi.EndInit();
                                            });
                                        }
                                    }
                                    return opkinf;
                                }
                            }
                        }
                    }

                    return null;
                }
            });
        }

        public async Task RenameFile(string toFileName)
        {
            var fullName = FullName.Replace(Name, toFileName);
            await FtpHandler.Instance.RenameFile(this.FullName, fullName, new System.Threading.CancellationToken());
            FullName = fullName;
            Name = toFileName;
        }

        public void GetImage(bool overwrightIfExists = true)
        {
            if (IsImage && Image == null)
            {
                //var pathImage = FtpHandler.Instance.DownloadFile(FullName, Name, overwrightIfExists);

                using (MemoryStream mem = new MemoryStream())
                {
                    FtpHandler.Instance.DownloadStream(FullName, mem);

                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                    {
                        Image = TheGamesDbHandler.LoadImage(mem);
                    });
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task GetChild(bool refreshData = false)
        {
            await Task.Run(() =>
            {
                if (!IsDirectoryOrLink) return;
                if (Items.Any(i => i == null) || refreshData) App.Current.Dispatcher.Invoke((Action)delegate { Items.Clear(); });
                var ftpItems = FtpHandler.Instance.List(mFullName);
                foreach (var f in ftpItems)
                {
                    var ftp = new FtpListItemModel(f);
                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                    {
                        if (!Items.Any(p => p.FullName == ftp.FullName)) Items.Add(ftp);
                    });

                    foreach (var item in Items)
                    {
                        if (item.Type == FtpFileSystemObjectType.File) continue;
                        App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                        {
                            item.Items.Add(null);
                        });
                    }
                }
            });
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

    public class opkInfo
    {
        public opkInfo()
        {
        }

        public string Name { get; set; }
        public string Comment { get; set; }
        public string Exec { get; set; }
        public string Terminal { get; set; }
        public string Type { get; set; }
        public string StartupNotify { get; set; }
        public string Icon { get; set; }
        public string Categories { get; set; }
        public BitmapImage Image { get; set; }
    }
}