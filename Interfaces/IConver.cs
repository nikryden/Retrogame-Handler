using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroGameHandler.Interfaces
{
    public interface IConvert
    {
        void Convert(object obj);

        void ConvertBack(object obj);
    }
}