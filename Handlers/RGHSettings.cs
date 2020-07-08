using LiteDB;
using RetroGameHandler.Entities;
using RetroGameHandler.models;
using System.Linq;

namespace RetroGameHandler.Handlers
{
    public static class RGHSettings
    {
        public static string ProgGuid { get; } = "a796d1b6844b44bfaaffe2a333199c05";
        public static string ScrapPath { get; } = "https://timeonline.se/api/";
        public static ErrorLevel LogLevel { get; set; } = ErrorLevel.DEBUG;
        public static bool AutoSave { get; set; } = true;
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
            LiteDBHelper.RGHSetData(ProgramSetting);
            ProgramSetting.Dirty = false;
        }

        public static void newFtpSetting(string name)
        {
            ProgramSetting.SelectedFtpSetting = new FtpSettingsModel(name, "", "", "");
            ProgramSetting.FtpSettingList.Add(ProgramSetting.SelectedFtpSetting);
        }

        public static void init()
        {
            ProgramSetting = LiteDBHelper.RGHGetData();
        }
    }
}