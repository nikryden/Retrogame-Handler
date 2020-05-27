using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroGameHandler.models
{
   public class LinksModel
    {
       public List<Link> Links { get; set; }
       public List<Category> Categories { get; set; }
    }

    public class Category
    {
        public string category { get; set; }
        public string color { get; set; }
        
    }
    public class Link
    {       
        public int id { get; set; }
        public string category { get; set; }
        public string name { get; set; }
        public string short_description { get; set; }
        public string long_description { get; set; }
        public string link { get; set; }
        public int point { get; set; }
        //"point": 4

    }
}
