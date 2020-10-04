using System;
using System.Collections.Generic;

namespace RetroGameHandler.Handlers
{
    public static partial class SSHHandler
    {
        public class Diskinfo
        {
            public Diskinfo(List<string> row)
            {
                Name = row[0];
                //if (!string.IsNullOrWhiteSpace(row[1])) Size = Convert.ToUInt64(row[1].Trim());
                Type = row[2];
                MoutPoint = row[3];
                SIZE = row[4];
                USED = row[5];
                USEPERC = row[6];
                AVAIL = row[7];
                FSTYPE = row[8];
                LABEL = row[9];
            }

            public bool Equals(Diskinfo Diskinfo)
            {
                var inT = Diskinfo.GetType();
                var tp = this.GetType().GetProperties();
                foreach (var p in tp)
                {
                    var iP = inT.GetProperty(p.Name);
                    if (p.GetValue(this).ToString() != iP.GetValue(Diskinfo).ToString())
                    {
                        return false;
                    }
                }
                return true;
            }

            public string MoutPoint { get; }
            public string Name { get; }
            public string SIZE { get; }
            public string Type { get; }
            public string USED { get; }
            public string USEPERC { get; }

            public uint percent
            {
                get
                {
                    return Convert.ToUInt32(USEPERC.Replace("%", "").Trim());
                }
            }

            public string AVAIL { get; }
            public string FSTYPE { get; }
            public string LABEL { get; }
        }
    }
}