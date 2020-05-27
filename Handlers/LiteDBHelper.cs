using LiteDB;
using RetroGameHandler.Entities;
using RetroGameHandler.models;
using System;
using System.IO;
using System.Linq;

namespace RetroGameHandler.Handlers
{
    public static class RGHSettings
    {
        public static bool AutoSave { get; set; } = true;
        private static LiteDBHelper liteDBHelper = new LiteDBHelper();
        public static RGHSettingsModel ProgramSetting { get; set; }

        public static void Save()
        {
            var ftplst = ProgramSetting.FtpSettingList.FirstOrDefault(s => s.Id == ProgramSetting.SelectedFtpSetting.Id);
            if (ftplst != null)
            {
                ftplst.Name = ProgramSetting.SelectedFtpSetting.Name;
                ftplst.FtpHost = ProgramSetting.SelectedFtpSetting.FtpHost;
                ftplst.FtpUserName = ProgramSetting.SelectedFtpSetting.FtpUserName;
                ftplst.FtpPassword = ProgramSetting.SelectedFtpSetting.FtpPassword;
            }
            liteDBHelper.SetData(ProgramSetting);
            ProgramSetting.Dirty = false;
        }

        public static void newFtpSetting(string name)
        {
            ProgramSetting.SelectedFtpSetting = new FtpSettingsModel(name, "", "", "");
            ProgramSetting.FtpSettingList.Add(ProgramSetting.SelectedFtpSetting);
        }

        public static void init()
        {
            ProgramSetting = liteDBHelper.GetData();
        }
    }

    public class LiteDBHelper : IDisposable
    {
        public void Dispose()
        {
            db.Dispose();
            db = null;
        }

        public LiteDatabase db;

        public void connect()
        {
            var path = @"Timeonline\RetroGameHandler";
            var tmpPath = Path.Combine(Path.GetTempPath(), path);
            if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);
            db = new LiteDatabase(Path.Combine(tmpPath, @"RetroGameHandler2020.db"));
        }

        public RGHSettingsModel GetData()
        {
            if (db == null) connect();
            var st = db.GetCollection<RGHSettingsEntity>(BsonAutoId.ObjectId);
            var q = st.Query().ToList();
            var rm = q.FirstOrDefault();
            return new RGHSettingsModel(rm ?? new RGHSettingsEntity());
        }

        public void SetData(RGHSettingsModel data)
        {
            if (db == null) connect();
            var col = db.GetCollection<RGHSettingsEntity>(BsonAutoId.ObjectId);
            var dt = new RGHSettingsEntity(data);
            //var cl = db.
            //if (data.Id == null) col.Insert(dt);
            col.Upsert(dt);
        }
    }
}