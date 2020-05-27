using LiteDB;
using RetroGameHandler.Entities;
using RetroGameHandler.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RetroGameHandler.models
{
    public class RGHSettingsModel : BaseViewModel, INotifyPropertyChanged
    {
        private bool _showInActivities = true;
        private ObjectId _id;
        private FtpSettingsModel _selectedFtpSetting;
        private ObservableCollection<FtpSettingsModel> _ftpSettingList = new ObservableCollection<FtpSettingsModel>();

        public ObjectId Id
        {
            get => _id;
            set
            {
                if (_id == value) return;
                _id = value;
                NotifyPropertyChanged();
            }
        }

        public bool ShowInActivities
        {
            get => _showInActivities;
            set
            {
                if (_showInActivities == value) return;
                _showInActivities = value;
                NotifyPropertyChanged();
            }
        }

        public FtpSettingsModel SelectedFtpSetting
        {
            get => _selectedFtpSetting;
            set
            {
                if (_selectedFtpSetting == value) return;
                _selectedFtpSetting = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<FtpSettingsModel> FtpSettingList
        {
            get => _ftpSettingList;
            set
            {
                if (_ftpSettingList == value) return;
                _ftpSettingList = value;
                NotifyPropertyChanged();
            }
        }

        public RGHSettingsModel(RGHSettingsEntity RGHSettingsEntity)
        {
            Id = RGHSettingsEntity.Id;
            ShowInActivities = RGHSettingsEntity.ShowInActivities;
            SelectedFtpSetting = new FtpSettingsModel(RGHSettingsEntity.SelectedFtpSetting);
            if (RGHSettingsEntity.FtpSettingList != null)
                foreach (var item in RGHSettingsEntity.FtpSettingList)
                {
                    if (item.Id == SelectedFtpSetting.Id) FtpSettingList.Add(SelectedFtpSetting);
                    else FtpSettingList.Add(new FtpSettingsModel(item));
                }
            SelectedFtpSetting.PropertyChanged += (s, e) =>
            {
                Dirty = true;
            };
            Dirty = false;
        }

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

        public RGHSettingsModel()
        {
        }

        public RGHSettingsModel(bool ShowInActivities, FtpSettingsModel SelectedFtpSetting, IList<FtpSettingsModel> FtpSettingList)
        {
            this.ShowInActivities = ShowInActivities;
            this.SelectedFtpSetting = SelectedFtpSetting;
            this.FtpSettingList = new ObservableCollection<FtpSettingsModel>(FtpSettingList);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (propertyName != "Dirty") Dirty = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}