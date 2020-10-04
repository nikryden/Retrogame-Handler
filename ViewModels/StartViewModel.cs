using FluentFTP;
using RetroGameHandler.Handlers;
using RetroGameHandler.Helpers;
using RetroGameHandler.models;
using RetroGameHandler.UserControllImages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;

namespace RetroGameHandler.ViewModels
{
    public class StartViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private int _batteryPower;
        private IUserControllImage _batteryStatusImage;
        private ObservableCollection<FtpNameData> _ftpList;
        private ObservableCollection<FtpListItemModel> _ftpListItems;
        private bool _isconnected;
        private bool _powerConnected;
        private string _releaseInfo;
        private System.Timers.Timer _updatetimer = new System.Timers.Timer();
        private System.Timers.Timer _updatetimer2 = new System.Timers.Timer();

        private bool _working = false;
        private string _dots = "";

        public StartViewModel()
        {
            //FtpHandler.Instance.PropertyChanged += FtpPropertyChanged;
            _updatetimer.Elapsed += TimerUppdated;
            _updatetimer2.Elapsed += updateDots;
            BatteryStatusImage = ControllImages.CImages.Images["BatteryCharging"];
        }

        private void updateDots(object sender, ElapsedEventArgs e)
        {
            _dotCount++;
            _dots = "";
            Dots = _dots.PadRight(_dotCount, '.');
            if (_dotCount == 10) _dotCount = 1;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private int _dotCount = 0;

        public string Dots
        {
            get { return _dots; }
            set
            {
                _dots = value;
                NotifyPropertyChanged();
            }
        }

        public int BatteryPower
        {
            get => _batteryPower;
            set
            {
                _batteryPower = value;
                var spl = Math.Ceiling((decimal)value / 20) - 1;
                if (!_powerConnected &&
                    ControllImages.CImages.Images.ContainsKey($"Battery{spl}")) BatteryStatusImage =
                        ControllImages.CImages.Images[$"Battery{spl}"];
                if (!_powerConnected) BatteryIconHelper.ChangeIcon((int)spl, $"{value}%");
                NotifyPropertyChanged();
            }
        }

        public IUserControllImage BatteryStatusImage
        {
            get
            {
                return _batteryStatusImage;
            }
            private set
            {
                _batteryStatusImage = value;
                NotifyPropertyChanged();
            }
        }

        public async Task<FtpListItemModel> FindItemByPathAsync(string path)
        {
            return await Task.Run(() =>
            {
                if (FtpListItems == null) return null;
                foreach (var item in FtpListItems)
                {
                    var ftpListItemModel = item.FindItemByPath(path);
                    if (ftpListItemModel != null) return ftpListItemModel;
                }
                return null;
            });
        }

        public ObservableCollection<FtpListItemModel> FtpListItems
        {
            get => _ftpListItems;
            set
            {
                _ftpListItems = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsConnected
        {
            get => _isconnected;
            set
            {
                _isconnected = value;
                NotifyPropertyChanged();
                if (_isconnected)
                {
                    _updatetimer.Interval = 1;

                    Working = true;

                    _updatetimer.Start();
                }
                else
                {
                    BatteryIconHelper.ChangeIcon(-1, $"");
                    _updatetimer.Stop();
                    FtpListItems = null;
                    BatteryPower = -1;
                    PowerConnected = false;
                    ReleaseInfo = "";
                    Working = false;
                }
            }
        }

        public bool PowerConnected
        {
            get => _powerConnected;
            set
            {
                _powerConnected = value;
                if (_powerConnected && ControllImages.CImages.Images.ContainsKey("BatteryCharging")) BatteryStatusImage = ControllImages.CImages.Images["BatteryCharging"];
                if (_powerConnected) BatteryIconHelper.ChangeIcon(5, $"Charging");
                NotifyPropertyChanged();
            }
        }

        private BindingList<SSHHandler.Diskinfo> _diskinfos = new BindingList<SSHHandler.Diskinfo>();

        public BindingList<SSHHandler.Diskinfo> DiskInfos
        {
            get => _diskinfos;
            set
            {
                var isNotEqual = false;
                if (_diskinfos.Count == 0)
                {
                    _diskinfos = value;
                    NotifyPropertyChanged();
                }
                else
                {
                    if (_diskinfos.Count == value.Count)
                    {
                        for (int i = 0; i < _diskinfos.Count; i++)
                        {
                            if (!_diskinfos[i].Equals(value[i]))
                            {
                                isNotEqual = true;
                                break;
                            }
                        }
                    }
                    else { isNotEqual = true; }

                    if (isNotEqual)
                    {
                        _diskinfos = value;
                        NotifyPropertyChanged();
                    }
                }

                //if (_powerConnected && ControllImages.CImages.Images.ContainsKey("BatteryCharging")) BatteryStatusImage = ControllImages.CImages.Images["BatteryCharging"];
                //if (_powerConnected) BatteryIconHelper.ChangeIcon(5, $"Charging");
            }
        }

        public string ReleaseInfo
        {
            get => _releaseInfo;
            set
            {
                _releaseInfo = value;
                NotifyPropertyChanged();
            }
        }

        public bool Working
        {
            get => _working;
            set
            {
                _working = value;
                if (_working)
                {
                    _updatetimer2.Start();
                    _dotCount = 0;
                }
                else _updatetimer2.Stop();
                NotifyPropertyChanged();
            }
        }

        public async Task GetFtpChildItems(FtpListItemModel model = null, bool doNext = true)
        {
            if (model == null) return;

            await GetFtpListItems(model.Items, model.FullName);
        }

        public async Task GetFtpListItems()
        {
            FtpListItems = new ObservableCollection<FtpListItemModel>();
            await GetFtpListItems(FtpListItems);
        }

        public async Task GetFtpListItems(ObservableCollection<FtpListItemModel> ListItem, string path = null, bool refreshData = false)
        {
            await Task.Run(() =>
            {
                if (ListItem.Any(i => i == null) || refreshData) App.Current.Dispatcher.Invoke((Action)delegate { ListItem.Clear(); });
                var ftpItems = FtpHandler.Instance.List(path);
                foreach (var f in ftpItems)
                {
                    var ftp = new FtpListItemModel(f);
                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                    {
                        if (!ListItem.Any(p => p.FullName == ftp.FullName)) ListItem.Add(ftp);
                    });

                    foreach (var item in ListItem)
                    {
                        if (item.Type == FtpFileSystemObjectType.File) continue;
                        App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                        {
                            if (item.Items.Count == 0) item.Items.Add(null);
                        });
                    }
                }
            });
        }

        private async void FtpPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("IsConnected");
            if (IsConnected)
            {
                ReleaseInfo = await SSHHandler.OsReleaseInfo();
                _updatetimer.Start();
            }
            else _updatetimer.Stop();
        }

        private new void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task SetBatteryPower()
        {
            var bt = await SSHHandler.GetBatteryPower();
            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                BatteryPower = bt;
            });
        }

        private async Task SetDiskInfo()
        {
            var bt = await SSHHandler.GetConsoleInfo();
            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                DiskInfos = new BindingList<SSHHandler.Diskinfo>(bt);
                //BatteryPower = bt;
            });
        }

        private async Task SetIsUsbPowerOnline()
        {
            var bt = await SSHHandler.IsUsbPowerOnline();
            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                PowerConnected = bt;
            });
        }

        private async Task SetOsReleaseInfo()
        {
            var bt = await SSHHandler.OsReleaseInfo();
            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                ReleaseInfo = bt;
            });
        }

        private async void TimerUppdated(object sender, ElapsedEventArgs e)
        {
            _updatetimer.Stop();
            if (IsConnected)
            {
                if (!await FtpHandler.Instance.IsValidConnection())
                {
                    IsConnected = false;
                    Working = false;
                    return;
                }
                var listTask = new List<Task>() { SetBatteryPower(), SetIsUsbPowerOnline(), SetDiskInfo() };
                if (string.IsNullOrWhiteSpace(ReleaseInfo))
                {
                    listTask.Add(SetOsReleaseInfo());
                }
                try
                {
                    await Task.WhenAll(listTask);
                }
                catch (Exception ex)
                {
                    ErrorHandler.Error(ex);
                }

                _updatetimer.Interval = 3000;
            }
            Working = false;
            _updatetimer.Start();
        }
    }
}