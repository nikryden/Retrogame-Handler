using RetroGameHandler.Handlers;
using RetroGameHandler.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace RetroGameHandler.ViewModels
{
    public class ScrapFolderViewModel : BaseViewModel, INotifyPropertyChanged
    {
        public event EventHandler PageLoadReady;

        private void PageLoadReadyChanged()
        {
            PageLoadReady?.Invoke(this, new EventArgs());
        }

        public ScrapFolderViewModel()
        {
            //PlatformModels = new OTheGamesDbHandler.PlatformModels
            DownloadList = new BindingList<DownloadImageModel>();
            DownloadList.ListChanged += (s, e) =>
            {
                NotifyPropertyChanged("Paged");
                NotifyPropertyChanged("FoundWithImages");
                NotifyPropertyChanged("IsSelected");
                NotifyPropertyChanged("MissingImages");
                NotifyPropertyChanged("FilesToScrape");
            };

            FtpHelper = FtpHandler.Instance;
            _character = new char[] { 'A' };
        }

        private PlatformModel _selectedPlatform;

        private FtpListItemModel _ftpListItem;
        private BindingList<DownloadImageModel> _downloadList = new BindingList<DownloadImageModel>();
        private FtpHelper _ftpHelper;
        private char[] _character;

        public ObservableCollection<PlatformModel> PlatformModels { get; } = TheGamesDbHandler.PlatformModels;

        public PlatformModel SelectedPlatform
        {
            get => _selectedPlatform;
            set
            {
                _selectedPlatform = value;
                NotifyPropertyChanged();
            }
        }

        public FtpListItemModel FtpListItem
        {
            get => _ftpListItem;
            set
            {
                _ftpListItem = value;
                NotifyPropertyChanged();
            }
        }

        public FtpHelper FtpHelper { get; set; }

        public BindingList<DownloadImageModel> DownloadList
        {
            get => _downloadList;
            set
            {
                _downloadList = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("Paged");
                NotifyPropertyChanged("FoundWithImages");
                NotifyPropertyChanged("IsSelected");
                NotifyPropertyChanged("MissingImages");
                NotifyPropertyChanged("FilesToScrape");
            }
        }

        public char[] Character
        {
            get => _character;
            set
            {
                _character = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("Paged");
            }
        }

        public bool HaveData(char[] Char)
        {
            return DownloadList.Any(i => !string.IsNullOrWhiteSpace(i.Name) && (Char.Contains(i.Name[0])));
        }

        public int FilesToScrape
        {
            get
            {
                return FoundWithImages + MissingImages; //;.Any(i => !string.IsNullOrWhiteSpace(i.Name) && (Char.Contains(i.Name[0])));
            }
        }

        public int FoundWithImages
        {
            get
            {
                var grp = DownloadList.GroupBy(s => s.Name);
                var grpOUt = grp.Where(s => s.Any(d => !string.IsNullOrWhiteSpace(d.GameTitle)));
                return grpOUt.ToList().Count();//;.Any(i => !string.IsNullOrWhiteSpace(i.Name) && (Char.Contains(i.Name[0])));
            }
        }

        public int IsSelected
        {
            get
            {
                var grp = DownloadList.GroupBy(s => s.Name);
                var grpOUt = grp.Where(s => s.Any(d => d.IsSelected));
                return grpOUt.Count();//;.Any(i => !string.IsNullOrWhiteSpace(i.Name) && (Char.Contains(i.Name[0])));
            }
        }

        public int MissingImages
        {
            get
            {
                var grp = DownloadList.GroupBy(s => s.Name);
                var grpOUt = grp.Where(s => s.All(d => string.IsNullOrWhiteSpace(d.GameTitle)));
                return grpOUt.Count();
            }
        }

        public async Task<IEnumerable<DownloadImageModel>> GetPage()
        {
            return await Task.Run(() =>
            {
                var lst = new List<DownloadImageModel>();
                if (Character[0] == '?')
                {
                    var grp = DownloadList.GroupBy(s => s.Name);
                    var grpOUt = grp.Where(s => s.All(d => !string.IsNullOrWhiteSpace(d.GameTitle) && !d.IsSelected));
                    foreach (var item in grpOUt)
                    {
                        lst.AddRange(item.ToList());
                    }
                    foreach (var item in lst)
                    {
                        if (!item.HasImage && !string.IsNullOrWhiteSpace(item.DownloadPath))
                        {
                            var path = TheGamesDbHandler.BaseUrls.FirstOrDefault(s => s.Name == "small").Path;
                            path += item.DownloadPath;
                            using (WebClient client = new WebClient())
                            {
                                item.ImageStream = client.DownloadData(path);
                            }
                            item.HasImage = true;
                        }
                    }
                }
                else if (Character[0] == '!')
                {
                    var grp = DownloadList.GroupBy(s => s.Name);
                    var grpOUt = grp.Where(s => s.All(d => string.IsNullOrWhiteSpace(d.GameTitle)));
                    foreach (var item in grpOUt)
                    {
                        lst.AddRange(item.ToList());
                    }
                }
                else
                {
                    lst = DownloadList?.OrderBy(g => g.Name).Where(l => !string.IsNullOrWhiteSpace(l.Name) && (Character.Contains(l.Name[0]))).ToList() ?? new List<DownloadImageModel>();
                    foreach (var item in lst)
                    {
                        if (!item.HasImage && !string.IsNullOrWhiteSpace(item.DownloadPath))
                        {
                            var path = TheGamesDbHandler.BaseUrls.FirstOrDefault(s => s.Name == "small").Path;
                            path += item.DownloadPath;
                            using (WebClient client = new WebClient())
                            {
                                item.ImageStream = client.DownloadData(path);
                            }
                            item.HasImage = true;
                        }
                    }
                }

                return lst;
            });
        }

        public IEnumerable<DownloadImageModel> Paged
        {
            get
            {
                return Task.Run(() =>
                 {
                     var lst = DownloadList?.OrderBy(g => g.Name).Where(l => !string.IsNullOrWhiteSpace(l.Name) && (Character.Contains(l.Name[0]))) ?? new ObservableCollection<DownloadImageModel>();
                     foreach (var item in lst)
                     {
                         if (!item.HasImage && !string.IsNullOrWhiteSpace(item.DownloadPath))
                         {
                             using (WebClient client = new WebClient())
                             {
                                 item.ImageStream = client.DownloadData(item.DownloadPath);
                             }
                             item.HasImage = true;
                         }
                     }
                     PageLoadReadyChanged();
                     return lst;
                 }).Result;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}