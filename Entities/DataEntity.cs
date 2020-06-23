using LiteDB;
using RetroGameHandler.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RetroGameHandler.Entities
{
    internal class DataEntityX
    {
        private ObjectId _id;
        public string ScrapGuid { get; set; }
        public string ScrapEmail { get; set; }
    }
}