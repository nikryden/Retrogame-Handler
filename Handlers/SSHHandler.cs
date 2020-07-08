using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetroGameHandler.Handlers
{
    public static class SSHHandler
    {
        public async static Task<bool> CheckConnection()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
                    using (var client = new SshClient(RGHSett.FtpHost, RGHSett.FtpUserName, RGHSett.FtpPassword))
                    {
                        client.Connect();
                        _ = client.RunCommand("echo test");
                        client.Disconnect();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.Error(ex);
                    Console.WriteLine(ex);
                    return false;
                }
            });
        }

        public async static Task<int> GetBatteryPower()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
                    using (var client = new SshClient(RGHSett.FtpHost, RGHSett.FtpUserName, RGHSett.FtpPassword))
                    {
                        client.Connect();
                        SshCommand pr = client.CreateCommand($"head /sys/class/power_supply/battery/capacity");
                        pr.Execute();
                        return Convert.ToInt32(pr.Result.Replace(@"\n", ""));
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.Error(ex);
                    Console.WriteLine(ex);
                    return default;
                }
            });
        }

        public static void GetConsoleInfo()
        {
            var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
            using (var client = new SshClient(RGHSett.FtpHost, RGHSett.FtpUserName, RGHSett.FtpPassword))
            {
                client.Connect();

                //head -n1 /etc/issue Show distri­bution
                //date
                //uptime
                //uname -a
                //cat /proc/cpuinfo

                var list = new List<string>() { "KNAME", "SIZE", "TYPE", "MOUNTPOINT" };
                var resultList = new Dictionary<string, List<string>>();
                foreach (var name in list)
                {
                    SshCommand sc = client.CreateCommand($"lsblk --all -b --output {name}");
                    sc.Execute();
                    resultList.Add(name,
                        sc.Result.Split(new[] { "\r\n", "\r", "\n" },
                        StringSplitOptions.None).ToList());
                }

                string[] ol = { "", "", "", "" };

                var listDiskInfo = new List<Diskinfo>();

                for (int i = 1; i < resultList["KNAME"].Count; i++)
                {
                    if (resultList["TYPE"][i] == "loop") continue;
                    ol[0] = resultList["KNAME"][i];
                    ol[1] = resultList["SIZE"][i];
                    ol[2] = resultList["TYPE"][i];
                    ol[3] = resultList["MOUNTPOINT"][i];
                    if (ol.All(s => string.IsNullOrWhiteSpace(s))) continue;
                    listDiskInfo.Add(new Diskinfo(ol.ToList()));
                }
            }
        }

        public async static Task<string> HostName()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
                    using (var client = new SshClient(RGHSett.FtpHost, RGHSett.FtpUserName, RGHSett.FtpPassword))
                    {
                        client.Connect();
                        SshCommand pr = client.CreateCommand($"hostname");
                        pr.Execute();
                        return pr.Result;
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.Error(ex);
                    Console.WriteLine(ex);
                    return default;
                }
            });
        }

        public async static Task<bool> IsUsbPowerOnline()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
                    using (var client = new SshClient(RGHSett.FtpHost, RGHSett.FtpUserName, RGHSett.FtpPassword))
                    {
                        client.Connect();
                        SshCommand pr = client.CreateCommand($"head /sys/class/power_supply/usb/online");
                        pr.Execute();
                        var res = Convert.ToInt16(pr.Result.Replace(@"\n", ""));
                        return Convert.ToBoolean(res);
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.Error(ex);
                    Console.WriteLine(ex);
                    return default;
                }
            });
        }

        public async static Task<string> OsReleaseInfo()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
                    using (var client = new SshClient(RGHSett.FtpHost, RGHSett.FtpUserName, RGHSett.FtpPassword))
                    {
                        client.Connect();
                        SshCommand pr = client.CreateCommand($"cat /etc/os-release");
                        pr.Execute();
                        return pr.Result;
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.Error(ex);
                    Console.WriteLine(ex);
                    return default;
                }
            });
        }

        private class ConsoleInfo
        {
            public Diskinfo Diskinfo { get; }
            public string Name { get; }
        }

        private class Diskinfo
        {
            public Diskinfo(List<string> row)
            {
                Name = row[0];
                if (!string.IsNullOrWhiteSpace(row[1])) Size = Convert.ToUInt64(row[1].Trim());
                Type = row[2];
                MoutPoint = row[3];
            }

            public string MoutPoint { get; }
            public string Name { get; }
            public ulong Size { get; } = 0;
            public string Type { get; }
        }
    }
}