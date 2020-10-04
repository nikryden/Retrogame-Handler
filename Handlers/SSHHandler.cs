using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetroGameHandler.Handlers
{
    public static partial class SSHHandler
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

        public async static Task<List<Diskinfo>> GetConsoleInfo()
        {
            var RGHSett = RGHSettings.ProgramSetting.SelectedFtpSetting;
            return await Task.Run(() =>
            {
                try
                {
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

                        string[] ol = { "", "", "", "", "", "", "", "", "", "" };

                        var listDiskInfo = new List<Diskinfo>();

                        for (int i = 1; i < resultList["KNAME"].Count; i++)
                        {
                            if (resultList["TYPE"][i] == "loop" || resultList["MOUNTPOINT"][i] == "") continue;
                            var mpoint = resultList["MOUNTPOINT"][i];
                            var listfindmnt = new List<string>() { "SIZE", "USED", "USE%", "AVAIL", "FSTYPE", "LABEL" };
                            //findmnt -T {mpoint} -o SIZE,USED,USE%,LABEL,AVAIL,FSTYPE -n
                            SshCommand sc2 = client.CreateCommand($"findmnt -T {mpoint} -o SIZE,USED,USE%,AVAIL,FSTYPE,LABEL -f -n");
                            sc2.Execute();
                            var dtInfo = sc2.Result.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            ol[0] = resultList["KNAME"][i];
                            ol[1] = resultList["SIZE"][i];
                            ol[2] = resultList["TYPE"][i];
                            ol[3] = string.Join("/", resultList["MOUNTPOINT"][i].Split('/').Reverse().Take(2).Reverse().ToList());
                            ol[4] = dtInfo[0];
                            ol[5] = dtInfo[1];
                            ol[6] = dtInfo[2];
                            ol[7] = dtInfo[3];
                            ol[8] = dtInfo[4];
                            ol[9] = dtInfo.Count() == 6 ? dtInfo[5] : "";
                            if (ol.All(s => string.IsNullOrWhiteSpace(s))) continue;
                            listDiskInfo.Add(new Diskinfo(ol.ToList()));
                        }
                        return listDiskInfo;
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.Error(ex);
                    return new List<Diskinfo>();
                }
            });
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
    }
}