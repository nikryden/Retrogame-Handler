using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroGameHandler.Helpers
{
    public static class GeneralFunctions
    {
        public static bool IsFilenameInvalid(string filename)
        {
            return filename.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0;
        }
    }
}