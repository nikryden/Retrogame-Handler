using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroGameHandler.models
{
    internal class DataModel
    {
        private int _id;

        public int Id
        {
            get => _id;
            set
            {
                if (_id == value) return;
                _id = value;
            }
        }

        public string ScrapGuid { get; set; }
        public string ScrapEmail { get; set; }
    }
}