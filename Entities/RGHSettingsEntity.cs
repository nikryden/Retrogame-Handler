using LiteDB;
using RetroGameHandler.models;
using System.Collections.Generic;

namespace RetroGameHandler.Entities
{
    public class RGHSettingsEntity : EntityBase
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public bool ShowInActivities { get; set; } = true;
        public FTPSettingsEntity SelectedFtpSetting { get; set; }
        public IList<FTPSettingsEntity> FtpSettingList { get; set; } = new List<FTPSettingsEntity>();

        public RGHSettingsEntity()
        {
        }

        public RGHSettingsEntity(bool ShowInActivities, FTPSettingsEntity SelectedFtpSetting, IList<FTPSettingsEntity> FtpSettingList)
        {
            Id = ObjectId.NewObjectId();
            this.ShowInActivities = ShowInActivities;
            this.SelectedFtpSetting = SelectedFtpSetting;
            this.FtpSettingList = FtpSettingList;
        }

        public RGHSettingsEntity(RGHSettingsModel RGHSettingsModel)
        {
            Id = RGHSettingsModel.Id ?? ObjectId.NewObjectId();
            ShowInActivities = RGHSettingsModel.ShowInActivities;
            SelectedFtpSetting = new FTPSettingsEntity(RGHSettingsModel.SelectedFtpSetting);
            foreach (var item in RGHSettingsModel.FtpSettingList)
            {
                FtpSettingList.Add(new FTPSettingsEntity(item));
            }
        }

        [BsonCtor]
        public RGHSettingsEntity(ObjectId _id, bool ShowInActivities, FTPSettingsEntity SelectedFtpSetting, IList<FTPSettingsEntity> FtpSettingList)
        {
            Id = _id;
            this.ShowInActivities = ShowInActivities;
            this.SelectedFtpSetting = SelectedFtpSetting;
            this.FtpSettingList = FtpSettingList;
        }
    }
}