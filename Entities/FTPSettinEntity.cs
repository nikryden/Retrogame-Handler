using LiteDB;
using RetroGameHandler.models;

namespace RetroGameHandler.Entities
{
    public class FTPSettingsEntity : EntityBase
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }
        public string FtpHost { get; set; }
        public string FtpUserName { get; set; }
        public string FtpPassword { get; set; }

        public FTPSettingsEntity(FtpSettingsModel FtpSettingsModel)
        {
            Id = FtpSettingsModel.Id ?? ObjectId.NewObjectId();
            Name = FtpSettingsModel.Name;
            FtpHost = FtpSettingsModel.FtpHost;
            FtpUserName = FtpSettingsModel.FtpUserName;
            FtpPassword = FtpSettingsModel.FtpPassword;
            FtpSettingsModel.Id = Id;
        }

        public FTPSettingsEntity(string Name, string FtpHost, string FtpUserName, string FtpPassword)
        {
            Id = ObjectId.NewObjectId();
            this.Name = Name;
            this.FtpHost = FtpHost;
            this.FtpUserName = FtpUserName;
            this.FtpPassword = FtpPassword;
        }

        [BsonCtor]
        public FTPSettingsEntity(ObjectId _id, string Name, string FtpHost, string FtpUserName, string FtpPassword)
        {
            Id = _id;
            this.Name = Name;
            this.FtpHost = FtpHost;
            this.FtpUserName = FtpUserName;
            this.FtpPassword = FtpPassword;
        }
    }
}