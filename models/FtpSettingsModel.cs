using LiteDB;
using RetroGameHandler.Entities;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RetroGameHandler.models
{
    public class FtpSettingsModel : INotifyPropertyChanged
    {
        public FtpSettingsModel()
        {
        }

        public FtpSettingsModel(string Name, string FtpHost, string FtpUserName, string FtpPassword)
        {
            this.Name = Name;
            this.FtpHost = FtpHost;
            this.FtpUserName = FtpUserName;
            this.FtpPassword = FtpPassword;
        }

        public FtpSettingsModel(FTPSettingsEntity FTPSettinEntity)
        {
            if (FTPSettinEntity == null) return;
            Id = FTPSettinEntity.Id;
            Name = FTPSettinEntity.Name;
            FtpHost = FTPSettinEntity.FtpHost;
            FtpUserName = FTPSettinEntity.FtpUserName;
            FtpPassword = FTPSettinEntity.FtpPassword;
        }

        private ObjectId _id;
        private string _name;
        private string _ftpHost;
        private string _ftpUserName;
        private string _ftpPassword;

        public ObjectId Id
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
            get => _name ?? "";
            set
            {
                if (_name == value) return;
                _name = value;
                NotifyPropertyChanged();
            }
        }

        public string FtpHost
        {
            get => _ftpHost ?? "";
            set
            {
                if (_ftpHost == value) return;
                _ftpHost = value;
                NotifyPropertyChanged();
            }
        }

        public string FtpUserName
        {
            get => _ftpUserName ?? "";
            set
            {
                if (_ftpUserName == value) return;
                _ftpUserName = value;
                NotifyPropertyChanged();
            }
        }

        public string FtpPassword
        {
            get => _ftpPassword ?? "";
            set
            {
                if (_ftpPassword == value) return;
                _ftpPassword = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}